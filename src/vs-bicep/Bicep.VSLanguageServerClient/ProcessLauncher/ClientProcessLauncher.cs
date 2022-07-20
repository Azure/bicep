// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Bicep.VSLanguageServerClient.ProcessLauncher
{
    /// <summary>
    /// Helper class which wraps starting the process. On arm64 architectures, windows will launch AnyCPU applications in x64 emulation mode. CreateProcess needs to be used to
    /// start the process natively on an arm64 machine and this class attempts to abstract that away.
    /// </summary>
    public static class ClientProcessLauncher
    {
        public static IClientProcess CreateClientProcess(string exePath, string args, string? curDirectory, Dictionary<string, string>? envVariables)
        {
            if (NativeMethods.IsArm64())
            {
                return CreateProcessForArm64(exePath, args, curDirectory, envVariables);
            }

            return CreateProcessForAmd64(exePath, args, curDirectory, envVariables);
        }

        private static IClientProcess CreateProcessForAmd64(string exePath, string args, string? curDirectory, Dictionary<string, string>? envVariables)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                CreateNoWindow = true,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = curDirectory,
            };
            if (envVariables != null)
            {
                foreach (KeyValuePair<string, string> kvp in envVariables)
                {
                    processStartInfo.EnvironmentVariables.Add(kvp.Key, kvp.Value);
                }
            }
            Process process = new Process()
            {
                StartInfo = processStartInfo
            };

            process.Start();

            return new ClientProcessAmd64(process);

        }

        private static IClientProcess CreateProcessForArm64(string exePath, string args, string? curDirectory, Dictionary<string, string>? envVariables)
        {
            // Set the thread attribute that indictates we want to run this exe as native arm64
            using ProcThreadAttributeList attributeList = ProcThreadAttributeList.CreateArm64AttributeList();

            // Create pipes so that stdout and stderr can be redirected.
            NativeMethods.SECURITY_ATTRIBUTES? securityAttribs = new NativeMethods.SECURITY_ATTRIBUTES
            {
                bInheritHandle = true
            };

            if (!NativeMethods.CreatePipe(out SafeFileHandle stdErrRead, out SafeFileHandle stdErrWrite, securityAttribs, 0) ||
                !NativeMethods.CreatePipe(out SafeFileHandle stdOutRead, out SafeFileHandle stdOutWrite, securityAttribs, 0) ||
                !NativeMethods.CreatePipe(out SafeFileHandle stdInRead, out SafeFileHandle stdInWrite, securityAttribs, 0))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            // We create it suspended so that we can attach a Process object to it safely (No race with the app unexpectedly shutting down)
            NativeMethods.CREATEFLAGS createFlags = NativeMethods.CREATEFLAGS.EXTENDED_STARTUPINFO_PRESENT |
                                                    NativeMethods.CREATEFLAGS.CREATE_NEW_CONSOLE |
                                                    NativeMethods.CREATEFLAGS.UNICODE_ENVIRONMENT |
                                                    NativeMethods.CREATEFLAGS.SUSPENDED;

            NativeMethods.STARTUPINFOEX? startupInfoEx = new NativeMethods.STARTUPINFOEX()
            {
                AttributeList = attributeList.AttributeList
            };

            startupInfoEx.StartupInfo = new NativeMethods.STARTUPINFO
            {
                cb = Marshal.SizeOf(startupInfoEx),
                wShowWindow = (short)NativeMethods.SHOWWINDOWFLAGS.SWP_HIDEWINDOW,
                hStdOutput = stdOutWrite,
                hStdError = stdErrWrite,
                hStdInput = stdInRead,
                dwFlags = NativeMethods.STARTFLAGS.STARTF_USESHOWWINDOW | NativeMethods.STARTFLAGS.STARTF_USESTDHANDLES
            };

            NativeMethods.PROCESS_INFORMATION processInfo = new NativeMethods.PROCESS_INFORMATION();
            string cmdLine = $"\"{exePath}\" {args}";
            bool creationSucceeded = NativeMethods.CreateProcess(lpApplicationName: null,
                                                lpCommandLine: new StringBuilder(cmdLine),
                                                processAttributes: IntPtr.Zero,
                                                threadAttributes: IntPtr.Zero,
                                                bInheritHandles: true,
                                                dwCreationFlags: createFlags,
                                                lpEnvironment: GetEnvironment(envVariables),
                                                lpCurrentDirectory: curDirectory,
                                                lpStartupInfo: startupInfoEx,
                                                lpProcessInformation: processInfo);

            if (!creationSucceeded)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            Process p = Process.GetProcessById(processInfo.dwProcessId);

            // Create the wrapper which will hook into the std handles and then resume the process
            IClientProcess processWrapper = new ClientProcessArm64(p, stdOutRead, stdErrRead, stdInWrite);
            NativeMethods.ResumeProcess(p);

            // Free returned handles
            NativeMethods.CloseHandle(processInfo.hProcess);
            NativeMethods.CloseHandle(processInfo.hThread);
            return processWrapper;
        }

        private static StringBuilder? GetEnvironment(Dictionary<string, string>? envVars)
        {
            if (envVars != null)
            {
                // Build up the list of key=value/0 variables
                StringBuilder envVariables = new();

                foreach (KeyValuePair<string, string> kvp in envVars)
                {
                    envVariables.Append(kvp.Key);
                    envVariables.Append('=');
                    envVariables.Append(kvp.Value);
                    envVariables.Append('\0');
                }

                // Add in the rest of the environment, filtering out variables which match those passed in
                foreach (DictionaryEntry kvp in Environment.GetEnvironmentVariables())
                {
                    var key = kvp.Key.ToString();
                    var value = kvp.Value;
                    if (key is not null && value is not null && !envVars.ContainsKey(key))
                    {
                        envVariables.Append(key);
                        envVariables.Append('=');
                        envVariables.Append(value.ToString());
                        envVariables.Append('\0');
                    }
                }

                // Double null terminate
                envVariables.Append('\0');
                return envVariables;
            }

            return null;
        }

        /// <summary>
        /// IClientProcess implemenation for executables started on Arm64 
        /// </summary>
        private class ClientProcessArm64 : IDisposable, IClientProcess
        {
            public Process Process { get; }
            public StreamReader StandardOutput { get; }
            public StreamReader StandardError { get; }
            public StreamWriter StandardInput { get; }

            public ClientProcessArm64(Process p, SafeFileHandle stdOutHandle, SafeFileHandle stdErrHandle, SafeFileHandle stdInHandle)
            {
                Process = p;
                StandardOutput = new StreamReader(new FileStream(stdOutHandle, FileAccess.Read, 4096, false), Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096);
                StandardError = new StreamReader(new FileStream(stdErrHandle, FileAccess.Read, 4096, false), Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096);
                StandardInput = new StreamWriter(new FileStream(stdInHandle, FileAccess.Write, 4096, false), Encoding.UTF8, 4096, leaveOpen: true);
            }

            public void Dispose()
            {
                StandardOutput.Dispose();
                StandardError.Dispose();
                StandardInput.Dispose();
                Process.Dispose();
            }
        }

        /// <summary>
        /// IClientProcess implemenation for executables started on normally using the Process class
        /// </summary>
        private class ClientProcessAmd64 : IDisposable, IClientProcess
        {
            public Process Process { get; }
            public StreamReader StandardOutput => Process.StandardOutput;
            public StreamReader StandardError => Process.StandardError;
            public StreamWriter StandardInput => Process.StandardInput;

            public ClientProcessAmd64(Process p)
            {
                Process = p;
            }

            public void Dispose()
            {
                Process.Dispose();
            }
        }

        /// <summary>
        /// Wraps handling the ProcThreadAttributeList ensuring the value set on the attribute list remains valid until DeleteProcThreadAttributeList is called which
        /// should be after the call to CreateProcess. 
        /// </summary>
        private class ProcThreadAttributeList : IDisposable
        {
            public static ProcThreadAttributeList CreateArm64AttributeList()
            {
                IntPtr size = IntPtr.Zero;
                bool wasInitialized = NativeMethods.InitializeProcThreadAttributeList(IntPtr.Zero, dwAttributeCount: 1, dwFlags: 0, ref size);
                if (wasInitialized || size == IntPtr.Zero)
                {
                    Debug.Fail($"Couldn't get the size of the attribute list: error = {Marshal.GetLastWin32Error()}");
                    throw new System.ComponentModel.Win32Exception();
                }

                IntPtr attributeList = Marshal.AllocHGlobal(size);
                if (attributeList == IntPtr.Zero)
                {
                    Debug.Fail($"Couldn't reserve space for a new attribute list: error = {Marshal.GetLastWin32Error()}");
                    throw new System.ComponentModel.Win32Exception();
                }

                wasInitialized = NativeMethods.InitializeProcThreadAttributeList(attributeList, dwAttributeCount: 1, dwFlags: 0, ref size);
                if (!wasInitialized)
                {
                    Debug.Fail($"Failed to initialize the new attribute list: error = {Marshal.GetLastWin32Error()}");
                    Marshal.FreeHGlobal(attributeList);
                    throw new System.ComponentModel.Win32Exception();
                }

                // Finally write the value to the attributeList
                IntPtr lpValue = Marshal.AllocHGlobal(sizeof(short));
                Marshal.WriteInt16(lpValue, unchecked((short)NativeMethods.IMAGE_FILE_MACHINE_ARM64));
                bool success = NativeMethods.UpdateProcThreadAttribute(attributeList, 0, new IntPtr(NativeMethods.PROC_THREAD_ATTRIBUTE_MACHINE_TYPE), lpValue,
                                                                       sizeof(short), IntPtr.Zero, IntPtr.Zero);
                if (!success)
                {
                    Marshal.FreeHGlobal(attributeList);
                    throw new System.ComponentModel.Win32Exception();
                }

                return new ProcThreadAttributeList(lpValue, attributeList);
            }

            private readonly IntPtr lpValue;
            public IntPtr AttributeList { get; private set; }

            private ProcThreadAttributeList(IntPtr lpValue, IntPtr attributeList)
            {
                this.lpValue = lpValue;
                AttributeList = attributeList;
            }

            public void Dispose()
            {
                NativeMethods.DeleteProcThreadAttributeList(AttributeList);
                Marshal.FreeHGlobal(lpValue);
                Marshal.FreeHGlobal(AttributeList);
            }
        }

        /// <summary>
        /// Native methods used to start the process for Arm64
        /// </summary>
        private class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool CreateProcess(
                                string? lpApplicationName,
                                StringBuilder lpCommandLine,
                                IntPtr processAttributes,
                                IntPtr threadAttributes,
                                [In, MarshalAs(UnmanagedType.Bool)]
                            bool bInheritHandles,
                                CREATEFLAGS dwCreationFlags,
                                [In, MarshalAs(UnmanagedType.LPWStr)] StringBuilder? lpEnvironment,
                                string? lpCurrentDirectory,
                                STARTUPINFOEX lpStartupInfo,
                                PROCESS_INFORMATION lpProcessInformation);

            [DllImport("kernel32.dll", SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool InitializeProcThreadAttributeList(
                 IntPtr lpAttributeList,
                 int dwAttributeCount,
                 int dwFlags,
                 ref IntPtr lpSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UpdateProcThreadAttribute(
                 IntPtr lpAttributeList,
                 uint dwFlags,
                 IntPtr Attribute,
                 IntPtr lpValue,
                 int cbSize,
                 IntPtr lpPreviousValue,
                 IntPtr lpReturnSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern void DeleteProcThreadAttributeList(IntPtr lpAttributeList);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern int ResumeThread(IntPtr hThread);

            [DllImport("kernel32.dll")]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

            [DllImport("kernel32", CharSet = CharSet.Auto)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

            public const ushort IMAGE_FILE_MACHINE_ARM64 = unchecked(0xAA64);
            public const int PROC_THREAD_ATTRIBUTE_MACHINE_TYPE = 0x00020019;

            [DllImport("kernel32.dll", SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
            public static extern bool IsWow64Process2(IntPtr process, out ushort processMachine, out ushort nativeMachine);

            public static bool IsArm64()
            {
                // IsWow64Process2 is not available on all supported versions of Windows (namely Server 2016, windows8) so
                // protect it with a try/catch
                try
                {
                    IntPtr handle = Process.GetCurrentProcess().Handle;
                    IsWow64Process2(handle, out ushort _, out ushort nativeMachine);
                    return nativeMachine == IMAGE_FILE_MACHINE_ARM64;
                }
                catch (EntryPointNotFoundException)
                {
                    return false;
                }
            }

            public static void ResumeProcess(Process process)
            {
                foreach (ProcessThread pT in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        ResumeThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
            }

            [Flags]
            internal enum ThreadAccess : int
            {
                TERMINATE = (0x0001),
                SUSPEND_RESUME = (0x0002),
                GET_CONTEXT = (0x0008),
                SET_CONTEXT = (0x0010),
                SET_INFORMATION = (0x0020),
                QUERY_INFORMATION = (0x0040),
                SET_THREAD_TOKEN = (0x0080),
                IMPERSONATE = (0x0100),
                DIRECT_IMPERSONATION = (0x0200)
            }

            [Flags]
            internal enum CREATEFLAGS
            {
                NONE = 0,
                DEBUG_PROCESS = 1,
                DEBUG_ONLY_THIS_PROCESS = 2,
                SUSPENDED = 4,
                NEW_PROCESS_GROUP = 0x200,
                CREATE_NEW_CONSOLE = 0x00000010,
                UNICODE_ENVIRONMENT = 0x00000400,
                EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            }

            [Flags]
            internal enum STARTFLAGS
            {
                STARTF_USESHOWWINDOW = 0x00000001,
                STARTF_USESIZE = 0x00000002,
                STARTF_USEPOSITION = 0x00000004,
                STARTF_USECOUNTCHARS = 0x00000008,
                STARTF_USEFILLATTRIBUTE = 0x00000010,
                STARTF_RUNFULLSCREEN = 0x00000020,
                STARTF_FORCEONFEEDBACK = 0x00000040,
                STARTF_FORCEOFFFEEDBACK = 0x00000080,
                STARTF_USESTDHANDLES = 0x00000100,
                STARTF_USEHOTKEY = 0x00000200
            };

            [Flags]
            internal enum SHOWWINDOWFLAGS
            {
                SWP_ASYNCWINDOWPOS = 0x4000,
                SWP_DEFERERASE = 0x2000,
                SWP_DRAWFRAME = 0x0020,
                SWP_FRAMECHANGED = 0x0020,
                SWP_HIDEWINDOW = 0x0080,
                SWP_NOACTIVATE = 0x0010,
                SWP_NOCOPYBITS = 0x0100,
                SWP_NOMOVE = 0x0002,
                SWP_NOOWNERZORDER = 0x0200,
                SWP_NOREDRAW = 0x0008,
                SWP_NOREPOSITION = 0x0200,
                SWP_NOSENDCHANGING = 0x0400,
                SWP_NOSIZE = 0x0001,
                SWP_NOZORDER = 0x0004,
                SWP_SHOWWINDOW = 0x0040,
            };

            [StructLayout(LayoutKind.Sequential)]
            internal class PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public int dwProcessId;
                public int dwThreadId;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal class STARTUPINFOEX
            {
                public STARTUPINFO? StartupInfo;
                public IntPtr AttributeList;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal class SECURITY_ATTRIBUTES
            {
                public SECURITY_ATTRIBUTES()
                {
                    nLength = (uint)Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                }

                private readonly uint nLength;
                public IntPtr lpSecurityDescriptor;
                public bool bInheritHandle;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal class STARTUPINFO : IDisposable
            {
                internal int cb;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string? lpReserved;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string? lpDesktop;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string? lpTitle;
                internal int dwX;
                internal int dwY;
                internal int dwXSize;
                internal int dwYSize;
                internal int dwXCountChars;
                internal int dwYCountChars;
                internal int dwFillAttribute;
                internal STARTFLAGS dwFlags;
                internal short wShowWindow;
                internal short cbReserved2;
                internal IntPtr lpReserved2;
                internal SafeFileHandle? hStdInput;
                internal SafeFileHandle? hStdOutput;
                internal SafeFileHandle? hStdError;

                internal STARTUPINFO()
                {
                    cb = Marshal.SizeOf(this);
                    hStdInput = new SafeFileHandle(IntPtr.Zero, false);
                    hStdOutput = new SafeFileHandle(IntPtr.Zero, false);
                    hStdError = new SafeFileHandle(IntPtr.Zero, false);
                }

                public void Dispose()
                {
                    // close the handles created for child process
                    if (hStdInput != null && !hStdInput.IsInvalid)
                    {
                        hStdInput.Close();
                        hStdInput = null;
                    }

                    if (hStdOutput != null && !hStdOutput.IsInvalid)
                    {
                        hStdOutput.Close();
                        hStdOutput = null;
                    }

                    if (hStdError != null && !hStdError.IsInvalid)
                    {
                        hStdError.Close();
                        hStdError = null;
                    }
                }
            }
        }
    }
}
