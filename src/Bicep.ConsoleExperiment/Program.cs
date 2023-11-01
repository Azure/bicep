// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.ConsoleExperiment;
using Spectre.Console;
using Spectre.Console.Rendering;

public static class Example
{
    public static string Truncate(this string value, int maxLength, string truncationSuffix = "...")
    {
        if (value.Length > maxLength && maxLength > 3)
        {
            int middle = maxLength / 2;
            var trunc_str = value.Substring(0, middle) + "..." + value.Substring(value.Length - middle + 1);
            return trunc_str;
        }
        return value;
    }
    public static void Main(string[] args)
    {
        //create dummy deployment info
        var deploymentObj = new DeploymentInfo("azureDeploy2", "stephy-rg", "0.01 (AzHPCImageGallery/AlmaLinuxHPC-8.6-gen2/0.0.1) - Microsoft Azure", "Value");
    
        //create dummy deployment objects
        var d1 = new Deployments("networkInterfaceName", "Microsoft.Network/networkInterfaces", "Creating");
        var d2 = new Deployments("networkSecurityGroup", "Microsoft.Network/networkSecurityGroups", "Created");
        var d3 = new Deployments("virtualNetworkName", "Microsoft.Network/virtualNetworks", "Creating");
        var d4 = new Deployments("vnetname1/subnetname1", "Microsoft.Network/virtualNetworks/subnets", "Creating");
        var d5 = new Deployments("subnetName2netName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName", "Microsoft.Network/virtualNetworks/subnets", "Created");
        var d6 = new Deployments("subnetName3netName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetNamenetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName", "Microsoft.Network/virtualNetworks/subnets", "Creating");
        var d7 = new Deployments("subnetName4subnetName4subnetName4", "Microsoft.Network/virtualNetworks/subnets", "Created");
        var d8 = new Deployments("vmExtension2", "Microsoft.Compute/virtualMachines/extensions", "Creating");
        var d9 = new Deployments("publicIpAddressName2", "Microsoft.Compute/virtualMachines/extensions", "Created");
        var d10 = new Deployments("publicIpAddressName1", "Microsoft.Network/PublicIPAddresses", "Created");
        var d11 = new Deployments("vmName1", "Microsoft.Compute/virtualMachines", "Creating");
        var d12 = new Deployments("vmName2", "Microsoft.Compute/virtualMachines", "Creating");

        //group in a list
        var deploymentList = new List<Deployments>() { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12 }; //original
       
        var w = Console.WindowWidth;
        var h = Console.WindowHeight;
        //fill up to 800
        var truncatedList = new List<Deployments>(); //modified
        for (int i = 0; i < 60; i++)
        {
            var name = "name" + i;
            var type = "resourceType" + i;
            var status = "status" + i;
            var tempDepObj = new Deployments(name, type, status);
            truncatedList.Add(tempDepObj);
            //truncatedList[i].Name = deploymentList[i % 12].Name + "#" + i;
            //truncatedList[i].Name = Truncate(truncatedList[i].Name, w / 4); 
        }

        //group by resource type
        var resourceGroups = truncatedList.GroupBy(d => d.ResourceType);

        const int startX = 0;
        const int startY = 5;
        const int optionsPerLine = 3;
        const int spacingPerLine = 55;

        int currentSelection = 0;

        ConsoleKey key;

        Console.CursorVisible = false;

        //organized deployments list
        var rgList = new List<string>();
        rgList.Add("Name ");
        rgList.Add("Resource Type");
        rgList.Add("Status ");

        foreach (var group in resourceGroups)
        {
            foreach (var item in group)
            {
                rgList.Add(item.Name);
                rgList.Add(item.ResourceType);
                rgList.Add(item.Status);
            }
        }

        //var rgList2 = resourceGroups.SelectMany(item => item).ToList();
        //var count = 5;

        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Deploment name: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(deploymentObj.Name);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Resource group: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(deploymentObj.ResourceGroup);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Portal Link: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(deploymentObj.PortalLink);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Correlation ID: ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(deploymentObj.CorrelationID);
        Console.ResetColor();
        var viewableRange = 3 * (h - 5);
        var flag = 0;
        var tempList = new List<string>(new string[viewableRange]);
        do
        {
            if (currentSelection > viewableRange)
            {
                flag = 1;
                var count = currentSelection;
                for (int i = 0; i < viewableRange; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                    if (i == viewableRange - 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (count < rgList.Count)
                    {
                        Console.Write(rgList[count - viewableRange + i]);
                        Console.Write(new String(' ', Math.Abs(rgList[count - viewableRange + i].Length - tempList[i].Length)));
                        tempList[i] = rgList[count - viewableRange + i];
                    }
                    Console.ResetColor();
                }
            }
            else
            {
                for (int i = 0; i < viewableRange; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);
                    
                    if (i == currentSelection)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write(rgList[i]);
                    tempList[i] = rgList[i];
                    if (flag == 1)
                    {
                        Console.Write(new String(' ', Math.Abs(rgList[i].Length - tempList[i].Length)));
                    }
                    Console.ResetColor();
                }
                if (flag == 1)
                {
                    flag = 0;
                }
            }

            key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    {
                        if (currentSelection % optionsPerLine > 0)
                        {
                            currentSelection--;
                        }
                        break;
                    }
                case ConsoleKey.RightArrow:
                    {
                        if (currentSelection % optionsPerLine < optionsPerLine - 1)
                        {
                            currentSelection++;
                        }
                        break;
                    }
                case ConsoleKey.UpArrow:
                    {
                        if (currentSelection >= optionsPerLine) {
                                currentSelection -= optionsPerLine;
                        }
                        break;
                    }
                case ConsoleKey.DownArrow:
                    {
                    if (currentSelection + optionsPerLine < rgList.Count)
                    {
                        currentSelection += optionsPerLine;
                    }
                        break;
                    }
                case ConsoleKey.Escape:
                    {
                       return;
                    }
            }

        } while (key != ConsoleKey.Enter);

        Console.CursorVisible = true;

        return;
    }

    //while (true)
    //{
    //    Console.Clear();

    //    for (int i = 0; i < truncatedList.Count; i++)
    //    {
    //        Console.SetCursorPosition(cursor.Left + (i % 10), cursor.Top +  i);

    //        if (i == viewableIndex)
    //        {
    //            Console.ForegroundColor = ConsoleColor.Red;
    //        }

    //        Console.WriteLine(truncatedList[i].Name + " " + truncatedList[i].ResourceType + " " + truncatedList[i].Status);

    //        Console.ResetColor();
    //    }

    //    if (Console.KeyAvailable)
    //    {
    //        ConsoleKeyInfo key = Console.ReadKey(true);
    //        switch (key.Key)
    //        {
    //            case ConsoleKey.UpArrow:
    //                if (viewableIndex != 0)
    //                {
    //                    viewableIndex--;
    //                }
    //                break;
    //            case ConsoleKey.DownArrow:
    //                if (viewableIndex < truncatedList.Count)
    //                {
    //                    viewableIndex++;
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
}

