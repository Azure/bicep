// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Refactor;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class TypeStringifierTests
{
    private static bool debugPrintAllSyntaxNodeTypes = false;

    [DataTestMethod]
    [DataRow(
        "type testType = int",
        "type loose = int",
        "type medium = int",
        "type strict = int")]
    [DataRow(
        "type testType = string",
        "type loose = string",
        "type medium = string",
        "type strict = string")]
    [DataRow(
        "type testType = bool",
        "type loose = bool",
        "type medium = bool",
        "type strict = bool")]
    [DataRow(
        "type testType = 'abc'?",
        "type loose = string?",
        "type medium = string?",
        "type strict = 'abc'?")]
    public void SimpleTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = 123",
        "type loose = int",
        "type medium = int",
        "type strict = 123")]
    [DataRow(
        "type testType = 'abc'",
        "type loose = string",
        "type medium = string",
        "type strict = 'abc'")]
    [DataRow(
        "type testType = true",
        "type loose = bool",
        "type medium = bool",
        "type strict = true")]
    public void LiteralTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = 123|234",
        "type loose = int",
        "type medium = 123 | 234",
        "type strict = 123 | 234")]
    [DataRow(
        "type testType = 'abc' | 'def' | null",
        "type loose = string?",
        "type medium = ('abc' | 'def')?",
        "type strict = ('abc' | 'def')?")]
    [DataRow(
        "type testType = ('abc' | 'def')[]",
        "type loose = array",
        "type medium = ('abc' | 'def')[]",
        "type strict = ('abc' | 'def')[]")]
    [DataRow(
        "type testType = { a: 'abc' | 'def' }",
        "type loose = object",
        "type medium = { a: 'abc' | 'def' }",
        "type strict = { a: 'abc' | 'def' }")]
    [DataRow("type testType = 123|null",
        "type loose = int?",
        "type medium = int?",  // I think "123?" would also be acceptable, it's not obvious which is better
        "type strict = 123?")]
    public void DontWidenLiteralTypesWithMediumWhenPartOfAUnion(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataRow(
        "type testType = ('fizz' | 42 | {an: 'object'} | null)[]",
        "type loose = array",
        // TODO: better would be: "type medium = ('fizz' | 42 | {an: 'object'} | null)[]",
        "type medium = ((object /* 'fizz' | 42 | { an: 'object' } */)?)[]",
        "type strict = ((object /* 'fizz' | 42 | { an: 'object' } */)?)[]")]
    [DataTestMethod]
    public void MixedTypeArrays(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = object",
        "type loose = object",
        "type medium = object",
        "type strict = object")]
    [DataRow(
        "type testType = {}",
        "type loose = object",
        "type medium = object",
        "type strict = { }")]
    [DataRow(
        "type testType = { empty: { } }",
        "type loose = object",
        "type medium = { empty: object }",
        "type strict = { empty: { } }")]
    [DataRow(
        "type testType = {a:123,b:'abc'}",
        "type loose = object",
        "type medium = { a: int, b: string }",
        "type strict = { a: 123, b: 'abc' }")]
    [DataRow(
        "type testType = { 'my type': 'my string' }",
        "type loose = object",
        "type medium = { 'my type': string }",
        "type strict = { 'my type': 'my string' }")]
    [DataRow(
        "type testType = { 'true': true }",
        "type loose = object",
        "type medium = { 'true': bool }",
        "type strict = { 'true': true }")]
    public void ObjectTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = 'abc' | 'def' | 'ghi'",
        "type loose = string",
        "type medium = 'abc' | 'def' | 'ghi'",
        "type strict = 'abc' | 'def' | 'ghi'")]
    [DataRow(
        "type testType = 1 | 2 | 3 | -1",
        "type loose = int",
        "type medium = -1 | 1 | 2 | 3",
        "type strict = -1 | 1 | 2 | 3")]
    [DataRow(
        "type testType = true|false",
        "type loose = bool",
        "type medium = false | true",
        "type strict = false | true")]
    [DataRow(
        "type testType = null|true|false",
        "type loose = bool?",
        "type medium = (false | true)?",
        "type strict = (false | true)?")]
    [DataRow(
        "type testType = { a: 'a'|null, b: 'a'|'b'|null, c: 'a'|'b'|'c'|null }",
        "type loose = object",
        "type medium = { a: string?, b: ('a'|'b')?, c: ('a'|'b'|'c')? }",
        "type strict = { a: 'a'?, b: ('a'|'b')?, c: ('a'|'b'|'c')? }")]
    public void UnionTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = [1, 2, 3]",
        "type loose = array",
        "type medium = int[]",
        "type strict = [1, 2, 3]")]
    [DataRow(
        "type testType = [ object, array, {}, [] ]",
        "type loose = array",
        "type medium = [ object, array, object, array ]",
        "type strict = [ object, array, {}, [] ]")]
    [DataRow(
        "type testType = [int, string]",
        "type loose = array",
        "type medium = [int, string]",
        "type strict = [int, string]")]
    [DataRow(
        "type testType = [123, 'abc' | 'def']",
        "type loose = array",
        "type medium = [int, 'abc' | 'def']",
        "type strict = [123, 'abc' | 'def']")]
    // Bicep infers a type from literals like "['abc', 'def'] as typed tuples, the user more likely wants "string[]" if all the items are of the same type
    [DataRow(
        "type testType = int[]",
        "type loose = array",
        "type medium = int[]",
        "type strict = int[]")]
    [DataRow(
        "type testType = int[][]",
        "type loose = array",
        "type medium = int[][]",
        "type strict = int[][]")]
    [DataRow(
        "type testType = [ int ]",
        "type loose = array",
        "type medium = int[]",
        "type strict = [ int ]")]
    [DataRow(
        "type testType = [ int, int, int ]",
        "type loose = array",
        "type medium = int[]",
        "type strict = [ int, int, int ]")]
    [DataRow(
        "type testType = [ int?, int?, int? ]",
        "type loose = array",
        "type medium = (int?)[]",
        "type strict = [ int?, int?, int? ]")]
    [DataRow(
        "type testType = [ int?, int, int? ]",
        "type loose = array",
        "type medium = [ int?, int, int? ]",
        "type strict = [ int?, int, int? ]")]
    [DataRow(
        "type testType = [ 'abc'|'def', 'abc'|'def' ]",
        "type loose = array",
        "type medium = ('abc'|'def')[]",
        "type strict = [ 'abc'|'def', 'abc'|'def' ]")]
    [DataRow(
        "type testType = [ 'abc'|'def', 'def'|'ghi' ]",
        "type loose = array",
        "type medium = [ 'abc'|'def', 'def'|'ghi' ]",
        "type strict = [ 'abc'|'def', 'def'|'ghi' ]")]
    public void TupleTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataRow(
        "type testType = 'abc'|string|'def'",
        "type loose = string",
        "type medium = string /* 'abc' | 'def' | string */",
        "type strict = string /* 'abc' | 'def' | string */")]
    [DataRow(
        "type testType = int|123",
        "type loose = int",
        "type medium = int /* 123 | int */",
        "type strict = int /* 123 | int */")]
    [DataRow(
        "type testType = | true | bool",
        "type loose = bool",
        "type medium = bool /* bool | true */",
        "type strict = bool /* bool | true */")]
    [DataRow(
        "type testType = | true | null | bool",
        "type loose = bool?",
        "type medium = (bool /* bool | true */)?",
        "type strict = (bool /* bool | true */)?")]
    [DataRow(
        "type testType = | true | bool?",
        "type loose = bool?",
        "type medium = (bool /* bool | true */)?",
        "type strict = (bool /* bool | true */)?")]
    [DataRow(
        "type testType = true? | bool",
        "type loose = bool?",
        "type medium = (bool /* bool | true */)?",
        "type strict = (bool /* bool | true */)?")]
    [DataRow(
        "type testType = 'abc'|null|'def'|string",
        "type loose = string?",
        "type medium = (string /* 'abc' | 'def' | string */)?",
        "type strict = (string /* 'abc' | 'def' | string */)?")]
    [DataRow(
        "type testType = null|'abc'|'def'|string",
        "type loose = string?",
        "type medium = (string /* 'abc' | 'def' | string */)?",
        "type strict = (string /* 'abc' | 'def' | string */)?")]
    [DataRow(
        "type testType = string|null|'abc'|'def'",
        "type loose = string?",
        "type medium = (string /* 'abc' | 'def' | string */)?",
        "type strict = (string /* 'abc' | 'def' | string */)?")]
    [DataRow(
        "type testType = [ 'abc'|'def'|string, 'def'|'ghi' ]",
        "type loose = array",
        "type medium = [ string /* 'abc' | 'def' | string */, 'def'|'ghi' ]",
        "type strict = [ string /* 'abc' | 'def' | string */, 'def'|'ghi' ]")]
    [DataRow(
        "type testType = int | string",
        "type loose = object /* int | string */",
        "type medium = object /* int | string */",
        "type strict = object /* int | string */")]
    [DataRow(
        "type testType = [1 | string]",
        "type loose = array",
        "type medium = (object /* 1 | string */)[]",
        "type strict = [object /* 1 | string */]")]
    [DataRow(
        "type testType = 'abc' | int | string | [string]",
        "type loose = object /* 'abc' | [string] | int | string */",
        "type medium = object /* 'abc' | [string] | int | string */",
        "type strict = object /* 'abc' | [string] | int | string */")]
    [DataRow(
        "type testType = 'abc' | int | string | [string] | null",
        "type loose = (object /* 'abc' | [string] | int | string */)?",
        "type medium = (object /* 'abc' | [string] | int | string */)?",
        "type strict = (object /* 'abc' | [string] | int | string */)?")]
    [DataTestMethod]
    public void OpenEnumTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = string[]",
        "type loose = array",
        "type medium = string[]",
        "type strict = string[]")]
    [DataRow(
        "type testType = (string?)[]",
        "type loose = array",
        "type medium = (string?)[]",
        "type strict = (string?)[]")]
    [DataRow(
        "type testType = 'abc'[]",
        "type loose = array",
        "type medium = string[]",
        "type strict = 'abc'[]")]
    [DataRow(
        "type testType = ('abc'|'def')[]",
        "type loose = array",
        "type medium = ('abc' | 'def')[]",
        "type strict = ('abc' | 'def')[]")]
    public void TypedArrays(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = array",
        "type loose = array",
        "type medium = array",
        "type strict = array")]
    public void ArrayType(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = []",
        "type loose = array",
        // Bicep infers an empty array with no items from "[]", the user more likely wants "array"
        "type medium = array",
        "type strict = []")]
    public void EmptyArray(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = {}",
        "type loose = object",
        "type medium = object",
        "type strict = { }")]
    public void EmptyObject(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = []",
        "type loose = array",
        "type medium = array",
        "type strict = []")]
    public void EmptyArrays(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = [testType?]",
        "type loose = array",
        "type medium = (object /* recursive */?)[]", // CONSIDER: question mark before the comment would be better
        "type strict = [object /* recursive */?]")]
    [DataRow(
        "type testType = [string, testType?]",
        "type loose = array",
        "type medium = [string, object /* recursive */?]",
        "type strict = [string, object /* recursive */?]")]
    [DataRow(
        "type testType = [string, testType]?",
        "type loose = array?",
        "type medium = [string, object /* recursive */]?",
        "type strict = [string, object /* recursive */]?")]
    [DataRow(
        "type testType = {t: testType?, a: [testType, testType?]?}",
        "type loose = object",
        "type medium = {a: [object /* recursive */, object /* recursive */?]?, t: object /* recursive */?}",
        "type strict = {a: [object /* recursive */, object /* recursive */?]?, t: object /* recursive */?}")]
    public void RecursiveTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = string?",
        "type loose = string?",
        "type medium = string?",
        "type strict = string?")]
    [DataRow(
        "type testType = null|true",
        "type loose = bool?",
        "type medium = bool?",
        "type strict = true?")]
    [DataRow(
        "type testType = null|true|false",
        "type loose = bool?",
        "type medium = (false | true)?",
        "type strict = (false | true)?")]
    [DataRow(
        "type testType = (null|true)|null",
        "type loose = bool?",
        "type medium = bool?",
        "type strict = true?")]
    [DataRow(
        "type testType = (null|'a')|null|'a'",
        "type loose = string?",
        "type medium = string?",
        "type strict = 'a'?")]
    [DataRow(
        "type testType = (null|'a'|'b')|null|'c'",
        "type loose = string?",
        "type medium = ('a' | 'b' | 'c')?",
        "type strict = ('a' | 'b' | 'c')?")]
    [DataRow(
        "type testType = null|(1|2)",
        "type loose = int?",
        "type medium = (1 | 2)?",
        "type strict = (1 | 2)?")]
    [DataRow(
        "type testType = null|['a', 'b']",
        "type loose = array?",
        "type medium = string[]?",
        "type strict = ['a', 'b']?")]
    [DataRow(
        "type testType = null|{a: 'a', b: 1234?}",
        "type loose = object?",
        "type medium = { a: string, b: int? }?",
        "type strict = { a: 'a', b: 1234? }?")]
    [DataRow(
        "type testType = {a: 'a', b: 1234?}?",
        "type loose = object?",
        "type medium = { a: string, b: int? }?",
        "type strict = { a: 'a', b: 1234? }?")]
    [DataRow(
        "type testType = array?",
        "type loose = array?",
        "type medium = array?",
        "type strict = array?")]
    [DataRow(
        "type testType = []?",
        "type loose = array?",
        "type medium = array?",
        "type strict = []?")]
    [DataRow(
        "type testType = object?",
        "type loose = object?",
        "type medium = object?",
        "type strict = object?")]
    [DataRow(
        "type testType = {}?",
        "type loose = object?",
        "type medium = object?",
        "type strict = {}?")]
    [DataRow(
        """
            type testType = { a: 'a' | null, b: 'a' | 'b' | null, c: 'a' | 'b' | 'c' | null }?
            """,
        "type loose = object?",
        "type medium = { a: string?, b: ('a' | 'b')?, c: ('a' | 'b' | 'c')? }?",
        "type strict = { a: 'a'?, b: ('a' | 'b')?, c: ('a' | 'b' | 'c')? }?")]
    public void NullableTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, null);
    }

    [DataTestMethod]
    //
    // storage Kind property
    //
    [DataRow(
        """
            resource testResource 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              kind: 'StorageV2'
            }
            """,
        "kind",
        "type loose = string",
        "type medium = string /* 'BlobStorage' | 'BlockBlobStorage' | 'FileStorage' | 'Storage' | 'StorageV2' | string */",
        "type strict = string /* 'BlobStorage' | 'BlockBlobStorage' | 'FileStorage' | 'Storage' | 'StorageV2' | string */",
        null,
        DisplayName = "Storage kind property (open enum)")]
    [DataRow(
        """
            var _artifactsLocation = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/azuredeploy.json'
            var _artifactsLocationSasToken = '?sas=abcd'
            var commandToExecute = 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1'

            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
                properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: {
                        fileUris: [
                            uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                        ]
                        commandToExecute: commandToExecute
                    }
                }
            }
            """,
        "fileUris",
        "type loose = array",
        "type medium = string[]",
        "type strict = ['https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/writeblob.ps1?sas=abcd']",
        null,
        DisplayName = "virtual machine extensions fileUris property")]
    //
    // "settings" property
    //
    [DataRow(
        """
            var _artifactsLocation = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/azuredeploy.json'
            var _artifactsLocationSasToken = '?sas=abcd'
            var commandToExecute = 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1'

            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
                properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: {
                        fileUris: [
                            uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                        ]
                        commandToExecute: commandToExecute
                    }
                }
            }
            """,
        "settings",
        "type loose = object?",
        "type medium = { commandToExecute: string, fileUris: string[] }?",
        "type strict = { commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1', fileUris: ['https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/writeblob.ps1?sas=abcd'] }?",
        null,
        DisplayName = "typed as Any (virtual machine extensions settings property)")]
    //
    // "properties" property
    //
    [DataRow(
        """
            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
              name: 'cse-windows/extension'
              location: 'location'
              properties: {
              }
            }
            """,
        "properties",
        "type loose = object?",
        """
            type medium = {
              autoUpgradeMinorVersion: bool?
              forceUpdateTag: string?
              instanceView: {
                name: string?
                statuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                substatuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                type: string?
                typeHandlerVersion: string?
              }?
              protectedSettings: object? /* any */
              publisher: string?
              settings: object? /* any */
              type: string?
              typeHandlerVersion: string?
            }?
            """,
        """
            type strict = {
              autoUpgradeMinorVersion: bool?
              forceUpdateTag: string?
              instanceView: {
                name: string?
                statuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                substatuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                type: string?
                typeHandlerVersion: string?
              }?
              protectedSettings: object? /* any */
              publisher: string?
              settings: object? /* any */
              type: string?
              typeHandlerVersion: string?
            }?
            """,
        "type resourceDerived = resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties",
        DisplayName = "virtual machine extensions properties property")]
    [DataRow(
        """
            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
              name: 'cse-windows/extension'
              location: 'location'
              properties: {
              }
            }
            """,
        "properties",
        "type loose = object?",
        """
            type medium = {
              autoUpgradeMinorVersion: bool?
              forceUpdateTag: string?
              instanceView: {
                name: string?
                statuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                substatuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                type: string?
                typeHandlerVersion: string?
              }?
              protectedSettings: object? /* any */
              publisher: string?
              settings: object? /* any */
              type: string?
              typeHandlerVersion: string?
            }?
            """,
        "IGNORE",
        "type resourceDerived = resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties",
        DisplayName = "if")]
    [DataRow(
        """
            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = [for (item, index) in mylist: {
                name: 'cse-windows/extension'
                location: 'location'
                properties: {
                }
              }
            ]
            """,
        "properties",
        "type loose = object?",
        """
            type medium = {
              autoUpgradeMinorVersion: bool?
              forceUpdateTag: string?
              instanceView: {
                name: string?
                statuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                substatuses: {
                  code: string?
                  displayStatus: string?
                  level: ('Error' | 'Info' | 'Warning')?
                  message: string?
                  time: string?
                }[]?
                type: string?
                typeHandlerVersion: string?
              }?
              protectedSettings: object? /* any */
              publisher: string?
              settings: object? /* any */
              type: string?
              typeHandlerVersion: string?
            }?
            """,
        "IGNORE",
        "type resourceDerived = resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties",
        DisplayName = "for")]
    [DataRow(
        """
            resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
            }

            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
              parent: virtualMachine
              name: 'name'
            }
            """,
        "",
        "type loose = object? /* Microsoft.Compute/virtualMachines/extensions@2020-12-01 */",
        "type medium = object? /* Microsoft.Compute/virtualMachines/extensions@2020-12-01 */",
        "type strict = object? /* Microsoft.Compute/virtualMachines/extensions@2020-12-01 */",
        null, // Resource-derived type for a full resource is not allowed in Bicep
        DisplayName = "virtual machine extension full resource type")]
    [DataRow(
        """
            resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
            }

            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
              parent: virtualMachine
              name: 'name'
            }
            """,
        "parent",
        "type loose = object",
        "type medium = { asserts: object, dependsOn: (object /* module[] | (resource | module) | resource[] */)[], eTag: string, extendedLocation: { name: string?, type: (string /* 'EdgeZone' | string */)? }?, identity: { type: ('None' | 'SystemAssigned' | 'SystemAssigned, UserAssigned' | 'UserAssigned')?, userAssignedIdentities: object? }?, kind: string, location: string, managedBy: string, managedByExtended: string[], name: string, plan: { name: string?, product: string?, promotionCode: string?, publisher: string? }?, properties: { additionalCapabilities: { ultraSSDEnabled: bool? }?, availabilitySet: { id: string? }?, billingProfile: { maxPrice: int? }?, diagnosticsProfile: { bootDiagnostics: { enabled: bool?, storageUri: string? }? }?, evictionPolicy: (string /* 'Deallocate' | 'Delete' | string */)?, extensionsTimeBudget: string?, hardwareProfile: { vmSize: (string /* 'Basic_A0' | 'Basic_A1' | 'Basic_A2' | 'Basic_A3' | 'Basic_A4' | 'Standard_A0' | 'Standard_A1' | 'Standard_A10' | 'Standard_A11' | 'Standard_A1_v2' | 'Standard_A2' | 'Standard_A2_v2' | 'Standard_A2m_v2' | 'Standard_A3' | 'Standard_A4' | 'Standard_A4_v2' | 'Standard_A4m_v2' | 'Standard_A5' | 'Standard_A6' | 'Standard_A7' | 'Standard_A8' | 'Standard_A8_v2' | 'Standard_A8m_v2' | 'Standard_A9' | 'Standard_B1ms' | 'Standard_B1s' | 'Standard_B2ms' | 'Standard_B2s' | 'Standard_B4ms' | 'Standard_B8ms' | 'Standard_D1' | 'Standard_D11' | 'Standard_D11_v2' | 'Standard_D12' | 'Standard_D12_v2' | 'Standard_D13' | 'Standard_D13_v2' | 'Standard_D14' | 'Standard_D14_v2' | 'Standard_D15_v2' | 'Standard_D16_v3' | 'Standard_D16s_v3' | 'Standard_D1_v2' | 'Standard_D2' | 'Standard_D2_v2' | 'Standard_D2_v3' | 'Standard_D2s_v3' | 'Standard_D3' | 'Standard_D32_v3' | 'Standard_D32s_v3' | 'Standard_D3_v2' | 'Standard_D4' | 'Standard_D4_v2' | 'Standard_D4_v3' | 'Standard_D4s_v3' | 'Standard_D5_v2' | 'Standard_D64_v3' | 'Standard_D64s_v3' | 'Standard_D8_v3' | 'Standard_D8s_v3' | 'Standard_DS1' | 'Standard_DS11' | 'Standard_DS11_v2' | 'Standard_DS12' | 'Standard_DS12_v2' | 'Standard_DS13' | 'Standard_DS13-2_v2' | 'Standard_DS13-4_v2' | 'Standard_DS13_v2' | 'Standard_DS14' | 'Standard_DS14-4_v2' | 'Standard_DS14-8_v2' | 'Standard_DS14_v2' | 'Standard_DS15_v2' | 'Standard_DS1_v2' | 'Standard_DS2' | 'Standard_DS2_v2' | 'Standard_DS3' | 'Standard_DS3_v2' | 'Standard_DS4' | 'Standard_DS4_v2' | 'Standard_DS5_v2' | 'Standard_E16_v3' | 'Standard_E16s_v3' | 'Standard_E2_v3' | 'Standard_E2s_v3' | 'Standard_E32-16_v3' | 'Standard_E32-8s_v3' | 'Standard_E32_v3' | 'Standard_E32s_v3' | 'Standard_E4_v3' | 'Standard_E4s_v3' | 'Standard_E64-16s_v3' | 'Standard_E64-32s_v3' | 'Standard_E64_v3' | 'Standard_E64s_v3' | 'Standard_E8_v3' | 'Standard_E8s_v3' | 'Standard_F1' | 'Standard_F16' | 'Standard_F16s' | 'Standard_F16s_v2' | 'Standard_F1s' | 'Standard_F2' | 'Standard_F2s' | 'Standard_F2s_v2' | 'Standard_F32s_v2' | 'Standard_F4' | 'Standard_F4s' | 'Standard_F4s_v2' | 'Standard_F64s_v2' | 'Standard_F72s_v2' | 'Standard_F8' | 'Standard_F8s' | 'Standard_F8s_v2' | 'Standard_G1' | 'Standard_G2' | 'Standard_G3' | 'Standard_G4' | 'Standard_G5' | 'Standard_GS1' | 'Standard_GS2' | 'Standard_GS3' | 'Standard_GS4' | 'Standard_GS4-4' | 'Standard_GS4-8' | 'Standard_GS5' | 'Standard_GS5-16' | 'Standard_GS5-8' | 'Standard_H16' | 'Standard_H16m' | 'Standard_H16mr' | 'Standard_H16r' | 'Standard_H8' | 'Standard_H8m' | 'Standard_L16s' | 'Standard_L32s' | 'Standard_L4s' | 'Standard_L8s' | 'Standard_M128-32ms' | 'Standard_M128-64ms' | 'Standard_M128ms' | 'Standard_M128s' | 'Standard_M64-16ms' | 'Standard_M64-32ms' | 'Standard_M64ms' | 'Standard_M64s' | 'Standard_NC12' | 'Standard_NC12s_v2' | 'Standard_NC12s_v3' | 'Standard_NC24' | 'Standard_NC24r' | 'Standard_NC24rs_v2' | 'Standard_NC24rs_v3' | 'Standard_NC24s_v2' | 'Standard_NC24s_v3' | 'Standard_NC6' | 'Standard_NC6s_v2' | 'Standard_NC6s_v3' | 'Standard_ND12s' | 'Standard_ND24rs' | 'Standard_ND24s' | 'Standard_ND6s' | 'Standard_NV12' | 'Standard_NV24' | 'Standard_NV6' | string */)? }?, host: { id: string? }?, hostGroup: { id: string? }?, licenseType: string?, networkProfile: { networkInterfaces: { id: string?, properties: { primary: bool? }? }[]? }?, osProfile: { adminPassword: string?, adminUsername: string?, allowExtensionOperations: bool?, computerName: string?, customData: string?, linuxConfiguration: { disablePasswordAuthentication: bool?, patchSettings: { patchMode: (string /* 'AutomaticByPlatform' | 'ImageDefault' | string */)? }?, provisionVMAgent: bool?, ssh: { publicKeys: { keyData: string?, path: string? }[]? }? }?, requireGuestProvisionSignal: bool?, secrets: { sourceVault: { id: string? }?, vaultCertificates: { certificateStore: string?, certificateUrl: string? }[]? }[]?, windowsConfiguration: { additionalUnattendContent: { componentName: string?, content: string?, passName: string?, settingName: ('AutoLogon' | 'FirstLogonCommands')? }[]?, enableAutomaticUpdates: bool?, patchSettings: { enableHotpatching: bool?, patchMode: (string /* 'AutomaticByOS' | 'AutomaticByPlatform' | 'Manual' | string */)? }?, provisionVMAgent: bool?, timeZone: string?, winRM: { listeners: { certificateUrl: string?, protocol: ('Http' | 'Https')? }[]? }? }? }?, platformFaultDomain: int?, priority: (string /* 'Low' | 'Regular' | 'Spot' | string */)?, proximityPlacementGroup: { id: string? }?, securityProfile: { encryptionAtHost: bool?, securityType: (string /* 'TrustedLaunch' | string */)?, uefiSettings: { secureBootEnabled: bool?, vTpmEnabled: bool? }? }?, storageProfile: { dataDisks: { caching: ('None' | 'ReadOnly' | 'ReadWrite')?, createOption: string /* 'Attach' | 'Empty' | 'FromImage' | string */, detachOption: (string /* 'ForceDetach' | string */)?, diskSizeGB: int?, image: { uri: string? }?, lun: int, managedDisk: { diskEncryptionSet: { id: string? }?, id: string?, storageAccountType: (string /* 'Premium_LRS' | 'Premium_ZRS' | 'StandardSSD_LRS' | 'StandardSSD_ZRS' | 'Standard_LRS' | 'UltraSSD_LRS' | string */)? }?, name: string?, toBeDetached: bool?, vhd: { uri: string? }?, writeAcceleratorEnabled: bool? }[]?, imageReference: { id: string?, offer: string?, publisher: string?, sku: string?, version: string? }?, osDisk: { caching: ('None' | 'ReadOnly' | 'ReadWrite')?, createOption: string /* 'Attach' | 'Empty' | 'FromImage' | string */, diffDiskSettings: { option: (string /* 'Local' | string */)?, placement: (string /* 'CacheDisk' | 'ResourceDisk' | string */)? }?, diskSizeGB: int?, encryptionSettings: { diskEncryptionKey: { secretUrl: string, sourceVault: { id: string? } }?, enabled: bool?, keyEncryptionKey: { keyUrl: string, sourceVault: { id: string? } }? }?, image: { uri: string? }?, managedDisk: { diskEncryptionSet: { id: string? }?, id: string?, storageAccountType: (string /* 'Premium_LRS' | 'Premium_ZRS' | 'StandardSSD_LRS' | 'StandardSSD_ZRS' | 'Standard_LRS' | 'UltraSSD_LRS' | string */)? }?, name: string?, osType: ('Linux' | 'Windows')?, vhd: { uri: string? }?, writeAcceleratorEnabled: bool? }? }?, virtualMachineScaleSet: { id: string? }? }?, scale: { capacity: int, maximum: int, minimum: int }, sku: { capacity: int, family: string, model: string, name: string, size: string, tier: string }, tags: object?, zones: string[]? }",
        "type strict = { asserts: object, dependsOn: (object /* module[] | (resource | module) | resource[] */)[], eTag: string, extendedLocation: { name: string?, type: (string /* 'EdgeZone' | string */)? }?, identity: { type: ('None' | 'SystemAssigned' | 'SystemAssigned, UserAssigned' | 'UserAssigned')?, userAssignedIdentities: object? }?, kind: string, location: string, managedBy: string, managedByExtended: string[], name: string, plan: { name: string?, product: string?, promotionCode: string?, publisher: string? }?, properties: { additionalCapabilities: { ultraSSDEnabled: bool? }?, availabilitySet: { id: string? }?, billingProfile: { maxPrice: int? }?, diagnosticsProfile: { bootDiagnostics: { enabled: bool?, storageUri: string? }? }?, evictionPolicy: (string /* 'Deallocate' | 'Delete' | string */)?, extensionsTimeBudget: string?, hardwareProfile: { vmSize: (string /* 'Basic_A0' | 'Basic_A1' | 'Basic_A2' | 'Basic_A3' | 'Basic_A4' | 'Standard_A0' | 'Standard_A1' | 'Standard_A10' | 'Standard_A11' | 'Standard_A1_v2' | 'Standard_A2' | 'Standard_A2_v2' | 'Standard_A2m_v2' | 'Standard_A3' | 'Standard_A4' | 'Standard_A4_v2' | 'Standard_A4m_v2' | 'Standard_A5' | 'Standard_A6' | 'Standard_A7' | 'Standard_A8' | 'Standard_A8_v2' | 'Standard_A8m_v2' | 'Standard_A9' | 'Standard_B1ms' | 'Standard_B1s' | 'Standard_B2ms' | 'Standard_B2s' | 'Standard_B4ms' | 'Standard_B8ms' | 'Standard_D1' | 'Standard_D11' | 'Standard_D11_v2' | 'Standard_D12' | 'Standard_D12_v2' | 'Standard_D13' | 'Standard_D13_v2' | 'Standard_D14' | 'Standard_D14_v2' | 'Standard_D15_v2' | 'Standard_D16_v3' | 'Standard_D16s_v3' | 'Standard_D1_v2' | 'Standard_D2' | 'Standard_D2_v2' | 'Standard_D2_v3' | 'Standard_D2s_v3' | 'Standard_D3' | 'Standard_D32_v3' | 'Standard_D32s_v3' | 'Standard_D3_v2' | 'Standard_D4' | 'Standard_D4_v2' | 'Standard_D4_v3' | 'Standard_D4s_v3' | 'Standard_D5_v2' | 'Standard_D64_v3' | 'Standard_D64s_v3' | 'Standard_D8_v3' | 'Standard_D8s_v3' | 'Standard_DS1' | 'Standard_DS11' | 'Standard_DS11_v2' | 'Standard_DS12' | 'Standard_DS12_v2' | 'Standard_DS13' | 'Standard_DS13-2_v2' | 'Standard_DS13-4_v2' | 'Standard_DS13_v2' | 'Standard_DS14' | 'Standard_DS14-4_v2' | 'Standard_DS14-8_v2' | 'Standard_DS14_v2' | 'Standard_DS15_v2' | 'Standard_DS1_v2' | 'Standard_DS2' | 'Standard_DS2_v2' | 'Standard_DS3' | 'Standard_DS3_v2' | 'Standard_DS4' | 'Standard_DS4_v2' | 'Standard_DS5_v2' | 'Standard_E16_v3' | 'Standard_E16s_v3' | 'Standard_E2_v3' | 'Standard_E2s_v3' | 'Standard_E32-16_v3' | 'Standard_E32-8s_v3' | 'Standard_E32_v3' | 'Standard_E32s_v3' | 'Standard_E4_v3' | 'Standard_E4s_v3' | 'Standard_E64-16s_v3' | 'Standard_E64-32s_v3' | 'Standard_E64_v3' | 'Standard_E64s_v3' | 'Standard_E8_v3' | 'Standard_E8s_v3' | 'Standard_F1' | 'Standard_F16' | 'Standard_F16s' | 'Standard_F16s_v2' | 'Standard_F1s' | 'Standard_F2' | 'Standard_F2s' | 'Standard_F2s_v2' | 'Standard_F32s_v2' | 'Standard_F4' | 'Standard_F4s' | 'Standard_F4s_v2' | 'Standard_F64s_v2' | 'Standard_F72s_v2' | 'Standard_F8' | 'Standard_F8s' | 'Standard_F8s_v2' | 'Standard_G1' | 'Standard_G2' | 'Standard_G3' | 'Standard_G4' | 'Standard_G5' | 'Standard_GS1' | 'Standard_GS2' | 'Standard_GS3' | 'Standard_GS4' | 'Standard_GS4-4' | 'Standard_GS4-8' | 'Standard_GS5' | 'Standard_GS5-16' | 'Standard_GS5-8' | 'Standard_H16' | 'Standard_H16m' | 'Standard_H16mr' | 'Standard_H16r' | 'Standard_H8' | 'Standard_H8m' | 'Standard_L16s' | 'Standard_L32s' | 'Standard_L4s' | 'Standard_L8s' | 'Standard_M128-32ms' | 'Standard_M128-64ms' | 'Standard_M128ms' | 'Standard_M128s' | 'Standard_M64-16ms' | 'Standard_M64-32ms' | 'Standard_M64ms' | 'Standard_M64s' | 'Standard_NC12' | 'Standard_NC12s_v2' | 'Standard_NC12s_v3' | 'Standard_NC24' | 'Standard_NC24r' | 'Standard_NC24rs_v2' | 'Standard_NC24rs_v3' | 'Standard_NC24s_v2' | 'Standard_NC24s_v3' | 'Standard_NC6' | 'Standard_NC6s_v2' | 'Standard_NC6s_v3' | 'Standard_ND12s' | 'Standard_ND24rs' | 'Standard_ND24s' | 'Standard_ND6s' | 'Standard_NV12' | 'Standard_NV24' | 'Standard_NV6' | string */)? }?, host: { id: string? }?, hostGroup: { id: string? }?, licenseType: string?, networkProfile: { networkInterfaces: { id: string?, properties: { primary: bool? }? }[]? }?, osProfile: { adminPassword: string?, adminUsername: string?, allowExtensionOperations: bool?, computerName: string?, customData: string?, linuxConfiguration: { disablePasswordAuthentication: bool?, patchSettings: { patchMode: (string /* 'AutomaticByPlatform' | 'ImageDefault' | string */)? }?, provisionVMAgent: bool?, ssh: { publicKeys: { keyData: string?, path: string? }[]? }? }?, requireGuestProvisionSignal: bool?, secrets: { sourceVault: { id: string? }?, vaultCertificates: { certificateStore: string?, certificateUrl: string? }[]? }[]?, windowsConfiguration: { additionalUnattendContent: { componentName: 'Microsoft-Windows-Shell-Setup'?, content: string?, passName: 'OobeSystem'?, settingName: ('AutoLogon' | 'FirstLogonCommands')? }[]?, enableAutomaticUpdates: bool?, patchSettings: { enableHotpatching: bool?, patchMode: (string /* 'AutomaticByOS' | 'AutomaticByPlatform' | 'Manual' | string */)? }?, provisionVMAgent: bool?, timeZone: string?, winRM: { listeners: { certificateUrl: string?, protocol: ('Http' | 'Https')? }[]? }? }? }?, platformFaultDomain: int?, priority: (string /* 'Low' | 'Regular' | 'Spot' | string */)?, proximityPlacementGroup: { id: string? }?, securityProfile: { encryptionAtHost: bool?, securityType: (string /* 'TrustedLaunch' | string */)?, uefiSettings: { secureBootEnabled: bool?, vTpmEnabled: bool? }? }?, storageProfile: { dataDisks: { caching: ('None' | 'ReadOnly' | 'ReadWrite')?, createOption: string /* 'Attach' | 'Empty' | 'FromImage' | string */, detachOption: (string /* 'ForceDetach' | string */)?, diskSizeGB: int?, image: { uri: string? }?, lun: int, managedDisk: { diskEncryptionSet: { id: string? }?, id: string?, storageAccountType: (string /* 'Premium_LRS' | 'Premium_ZRS' | 'StandardSSD_LRS' | 'StandardSSD_ZRS' | 'Standard_LRS' | 'UltraSSD_LRS' | string */)? }?, name: string?, toBeDetached: bool?, vhd: { uri: string? }?, writeAcceleratorEnabled: bool? }[]?, imageReference: { id: string?, offer: string?, publisher: string?, sku: string?, version: string? }?, osDisk: { caching: ('None' | 'ReadOnly' | 'ReadWrite')?, createOption: string /* 'Attach' | 'Empty' | 'FromImage' | string */, diffDiskSettings: { option: (string /* 'Local' | string */)?, placement: (string /* 'CacheDisk' | 'ResourceDisk' | string */)? }?, diskSizeGB: int?, encryptionSettings: { diskEncryptionKey: { secretUrl: string, sourceVault: { id: string? } }?, enabled: bool?, keyEncryptionKey: { keyUrl: string, sourceVault: { id: string? } }? }?, image: { uri: string? }?, managedDisk: { diskEncryptionSet: { id: string? }?, id: string?, storageAccountType: (string /* 'Premium_LRS' | 'Premium_ZRS' | 'StandardSSD_LRS' | 'StandardSSD_ZRS' | 'Standard_LRS' | 'UltraSSD_LRS' | string */)? }?, name: string?, osType: ('Linux' | 'Windows')?, vhd: { uri: string? }?, writeAcceleratorEnabled: bool? }? }?, virtualMachineScaleSet: { id: string? }? }?, scale: { capacity: int, maximum: int, minimum: int }, sku: { capacity: int, family: string, model: string, name: string, size: string, tier: string }, tags: object?, zones: string[]? }",
        null,
        DisplayName = "virtual machine extension's 'parent' property")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'name'
              location: location
              properties: {
                sku: {
                  name: 'Standard_Small'
                  tier: 'Standard'
                  capacity: 1
                }
                gatewayIPConfigurations: [
                  {
                    name: 'name'
                    properties: {
                      subnet: {
                        id: 'id'
                      }
                    }
                  }
                ]
              }
            }
            """,
        "[skip]properties",
        "IGNORE",
        "IGNORE",
        "IGNORE",
        "type resourceDerived = resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.properties.gatewayIPConfigurations[*].properties",
        DisplayName = "Array 1")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'n'
              properties:{
                gatewayIPConfigurations: [
                  {
                    id: 'configId'
                    properties: {
                      subnet: {
                        id: 'subnetId'
                      }
                    }
                  }
                ]
              }
            }
            """,
        "[skip]id",
        "IGNORE",
        "IGNORE",
        "IGNORE",
        null, // subnet.id is a simple string
        DisplayName = "string inside an array")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'n'
              properties:{
                gatewayIPConfigurations: [
                  {
                    id: 'configId'
                    properties: {
                      subnet: {
                        id: 'subnetId'
                      }
                    }
                  }
                ]
              }
            }
            """,
        "subnet",
        "IGNORE",
        "IGNORE",
        "IGNORE",
        "type resourceDerived = resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.properties.gatewayIPConfigurations[*].properties.subnet",
        DisplayName = "custom object inside an array")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'n'
              dependsOn: []
            }
            """,
        "dependsOn",
        "IGNORE",
        "IGNORE",
        "IGNORE",
        "type resourceDerived = resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.dependsOn",
        DisplayName = "dependsOn")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'n'
              extendedLocation: {
                type: 'ArcZone'
                name: 'name'
              }
            }
            """,
        "type",
        "type loose = string",
        "type medium = string /* 'ArcZone' | 'CustomLocation' | 'EdgeZone' | 'NotSpecified' | string */",
        "type strict = string /* 'ArcZone' | 'CustomLocation' | 'EdgeZone' | 'NotSpecified' | string */",
        null,
        DisplayName = "string enum")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              name: 'n'
              identity: {
                type: 'None'
                userAssignedIdentities: {}
              }
            }
            """,
        "userAssignedIdentities",
        "type loose = object?",
        "type medium = object?",
        "type strict = object?",
        null,
        DisplayName = "custom object type with zero writable properties (resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.identity.userAssignedIdentities)")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              properties: {
                sslPolicy: {
                  cipherSuites: [
                    // hi there
                    'TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA' // hello
                  ]
                }
              }
            }
            """,
        "cipherSuites",
        "type loose = array?",
        "type medium = (string /* 'TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_128_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_128_CBC_SHA256' | 'TLS_DHE_DSS_WITH_AES_256_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_256_CBC_SHA256' | 'TLS_DHE_RSA_WITH_AES_128_CBC_SHA' | 'TLS_DHE_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_DHE_RSA_WITH_AES_256_CBC_SHA' | 'TLS_DHE_RSA_WITH_AES_256_GCM_SHA384' | 'TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA' | 'TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256' | 'TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256' | 'TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA' | 'TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384' | 'TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384' | 'TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA' | 'TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256' | 'TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA' | 'TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384' | 'TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384' | 'TLS_RSA_WITH_3DES_EDE_CBC_SHA' | 'TLS_RSA_WITH_AES_128_CBC_SHA' | 'TLS_RSA_WITH_AES_128_CBC_SHA256' | 'TLS_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_RSA_WITH_AES_256_CBC_SHA' | 'TLS_RSA_WITH_AES_256_CBC_SHA256' | 'TLS_RSA_WITH_AES_256_GCM_SHA384' | string */)[]?",
        "type strict = (string /* 'TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_128_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_128_CBC_SHA256' | 'TLS_DHE_DSS_WITH_AES_256_CBC_SHA' | 'TLS_DHE_DSS_WITH_AES_256_CBC_SHA256' | 'TLS_DHE_RSA_WITH_AES_128_CBC_SHA' | 'TLS_DHE_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_DHE_RSA_WITH_AES_256_CBC_SHA' | 'TLS_DHE_RSA_WITH_AES_256_GCM_SHA384' | 'TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA' | 'TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256' | 'TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256' | 'TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA' | 'TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384' | 'TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384' | 'TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA' | 'TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256' | 'TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA' | 'TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384' | 'TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384' | 'TLS_RSA_WITH_3DES_EDE_CBC_SHA' | 'TLS_RSA_WITH_AES_128_CBC_SHA' | 'TLS_RSA_WITH_AES_128_CBC_SHA256' | 'TLS_RSA_WITH_AES_128_GCM_SHA256' | 'TLS_RSA_WITH_AES_256_CBC_SHA' | 'TLS_RSA_WITH_AES_256_CBC_SHA256' | 'TLS_RSA_WITH_AES_256_GCM_SHA384' | string */)[]?",
        "type resourceDerived = resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.properties.sslPolicy.cipherSuites",
        DisplayName = "typed array")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              properties: {
                urlPathMaps: [
                  {
                    properties: {
                      pathRules: [
                        {
                          name: 'name'
                          properties: {
                            paths: [
                              'path'
                            ]
                            backendAddressPool: {
                              id: 'id'
                            }
                          }
                        }
                      ]
                    }
                  }
                ]
              }
            }
            """,
        "backendAddressPool",
        "type loose = object?",
        "type medium = { id: string? }?",
        "type strict = { id: string? }?",
        "type resourceDerived = resourceInput<'Microsoft.Network/applicationGateways@2020-11-01'>.properties.urlPathMaps[*].properties.pathRules[*].properties.backendAddressPool",
        DisplayName = "nested typed arrays")]
    [DataRow(
        """
            resource testResource 'Microsoft.Network/applicationGateways@2020-11-01' = {
              properties: {
                [
                    '*': {
                      cipherSuites: [
                        a: 'b'
                      ]
                    }
                ]
              }
            }
            """,
        "cipherSuites",
        "type loose = object? /* error */",
        "type medium = object? /* error */",
        "type strict = object? /* error */",
        null,
        DisplayName = "Error")]
    public void ResourcePropertyTypesAndResourceDerivedTypes(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string expectedResourceDerivedSyntax)
    {
        RunTestFromResourceProperty(resourceDeclaration, resourcePropertyName, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, expectedResourceDerivedSyntax);
    }

    [DataTestMethod]
    [DataRow(
        """
            type t1 = { abc: int }
            type testType = t1
            """,
        "type loose = object",
        "type medium = { abc: int }", // TODO: better would be "type medium = t1" but Bicep type system doesn't currently support it
        "type strict = { abc: int }", // TODO: better would be "type strict = t1" but Bicep type system doesn't currently support it
        null
        )]
    [DataRow(
        """
            type t1 = {
                a: string
                b: string
            }
            type t2 = t1[]
            type t3 = {
                t1Property: t1
                t2Property: t2
            }
            type testType = [t3]
            """,
        "type loose = array",
        "type medium = { t1Property: { a: string, b: string }, t2Property: { a: string, b: string }[] }[]", // TODO: better would be "type medium = t3[]" but Bicep type system doesn't currently support it
        "type strict = [ { t1Property: { a: string, b: string }, t2Property: { a: string, b: string }[] } ]",  // TODO: better would be "type strict = [ t3 ]" but Bicep type system doesn't currently support it
        null)]
    [DataRow(
        """
            type t1 = { a: 'abc', b: 123 }
            type testType = { a: t1, b: [t1, t1] }
            """,
        "type loose = object", // TODO: better: "{ a: t1, b: [t1, t1] }"
        "type medium = { a: { a: string, b: int }, b: { a: string, b: int }[] }", // TODO: better: "{ a: t1, b: [t1, t1] }"
        "type strict = { a: { a: 'abc', b: 123 }, b: [{ a: 'abc', b: 123 }, { a: 'abc', b: 123 }] }",
        null)]
    public void NamedTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string? expectedResourceDerivedSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, expectedResourceDerivedSyntax);
    }

    [DataTestMethod]
    [DataRow(
        """
            type negativeIntLiteral = -10
            type negatedIntReference = -negativeIntLiteral
            type negatedBoolLiteral = !true
            type negatedBoolReference = !negatedBoolLiteral
            type testType = {
              a: negativeIntLiteral
              b: negatedIntReference
              c: negatedBoolLiteral
              d: negatedBoolReference
            }
            """,
        "type loose = object",
        "type medium = { a: int, b: int, c: bool, d: bool }",
        "type strict = { a: -10, b: 10, c: false, d: true }",
        null)]
    public void NegatedTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string? expectedResourceDerivedSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, expectedResourceDerivedSyntax);
    }

    #region Support

    private static CompilationResult Compile(string source)
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new(ResourceTypedParamsAndOutputsEnabled: true));
        return CompilationHelper.Compile(services, source);
    }

    // input is a type declaration statement for type "testType", e.g. "type testType = int"
    private static void RunTestFromTypeDeclaration(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string? expectedResourceDerivedSyntax)
    {
        var compilationResult = Compile(typeDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
        var declaredType = semanticModel.GetDeclaredType(semanticModel.Root.TypeDeclarations.Single(t => t.Name == "testType").Value);
        declaredType.Should().NotBeNull();

        RunTestHelper(null, null, declaredType!, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, expectedResourceDerivedSyntax);
    }

    // input is a resource declaration for resource "testResource" and a property name such as "properties" that is exposed anywhere on the resource
    private static void RunTestFromResourceProperty(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string expectedResourceDerivedSyntax)
    {
        var useEntireResourceType = resourcePropertyName == "";

        var compilationResult = Compile(resourceDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var resourceSyntax = semanticModel.Root.ResourceDeclarations.SingleOrDefault(r => r.Name == "testResource")?.DeclaringResource!;
        resourceSyntax.Should().NotBeNull("the resource in the test must be named 'testResource'");

        var properties = GetAllSyntaxOfType<ObjectPropertySyntax>(resourceSyntax);
        ObjectPropertySyntax? matchingPropertySyntax;
        if (useEntireResourceType)
        {
            // Use the entire resource
            matchingPropertySyntax = null;
        }
        else
        {
            int skip = 0;
            while (resourcePropertyName.StartsWith("[skip]"))
            {
                skip++;
                resourcePropertyName = resourcePropertyName.Substring("[skip]".Length);
            }
            matchingPropertySyntax = properties.Skip(skip).Where(p => p.Key is IdentifierSyntax id && id.NameEquals(resourcePropertyName))
                .Skip(skip)
                .FirstOrDefault();
            matchingPropertySyntax.Should().NotBeNull($"can't find property {resourcePropertyName} in the resource's syntax");
        }

        var inferredType = semanticModel.GetTypeInfo(useEntireResourceType ? resourceSyntax : matchingPropertySyntax!.Value);
        var declaredType = semanticModel.GetDeclaredType(useEntireResourceType ? resourceSyntax : matchingPropertySyntax!.Value);
        var matchingPropertyTypeSymbol = declaredType is AnyType || declaredType == null ? inferredType : declaredType;
        matchingPropertyTypeSymbol.Should().NotBeNull();
        var matchingPropertyTypeProperty = matchingPropertySyntax?.TryGetTypeProperty(semanticModel);

        RunTestHelper(matchingPropertySyntax, matchingPropertyTypeProperty, matchingPropertyTypeSymbol!, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax, expectedResourceDerivedSyntax);
    }

    private static void RunTestHelper(ObjectPropertySyntax? propertySyntax, TypeProperty? typeProperty, TypeSymbol typeSymbol, SemanticModel semanticModel, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax, string? expectedResourceDerivedSyntax /* null means no resource-derived type can be generated */)
    {
        var ignoreLoose = expectedLooseSyntax == "IGNORE";
        var ignoreMediumStrict = expectedMediumStrictSyntax == "IGNORE";
        var ignoreStrict = expectedStrictSyntax == "IGNORE";
        var ignoreResourceDerived = expectedResourceDerivedSyntax == "IGNORE";

        if (debugPrintAllSyntaxNodeTypes)
        {
            DebugPrintAllSyntaxNodeTypes(semanticModel);
        }

        var actualLooseTypeName = ignoreLoose ? null : TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Loose);
        var actualMediumTypeName = ignoreMediumStrict ? null : TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Medium);
        var actualStrictTypeName = ignoreStrict ? null : TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Strict);
        var actualResourceDerivedTypeName = propertySyntax is null ? null : TypeStringifier.TryGetResourceDerivedTypeName(semanticModel, propertySyntax);

        using (new AssertionScope())
        {
            if (!ignoreLoose)
            {
                Compile(expectedLooseSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected loose syntax should be error-free");
            }
            if (!ignoreMediumStrict)
            {
                Compile(expectedMediumStrictSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected medium strictness syntax should be error-free");
            }
            if (!ignoreStrict)
            {
                Compile(expectedStrictSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected strict syntax should be error-free");
            }
            if (expectedResourceDerivedSyntax is { } && !ignoreResourceDerived)
            {
                /* TODO: Blocked by https://github.com/Azure/bicep/issues/15277
                Compile(expectedResourceDerivedSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected resource-derived syntax should be error-free");
                */
            }
        }

        using (new AssertionScope())
        {
            if (!ignoreLoose)
            {
                var actualLooseSyntaxTypeStmt = $"type loose = {actualLooseTypeName}";
                actualLooseSyntaxTypeStmt.Should().EqualIgnoringBicepFormatting(expectedLooseSyntax);
                Compile(actualLooseSyntaxTypeStmt).Diagnostics.Should().NotHaveAnyDiagnostics("the generated loose type string should compile successfully");
            }

            if (!ignoreMediumStrict)
            {
                var actualMediumStrictSyntaxTypeStmt = $"type medium = {actualMediumTypeName}";
                actualMediumStrictSyntaxTypeStmt.Should().EqualIgnoringBicepFormatting(expectedMediumStrictSyntax);
                Compile(actualMediumStrictSyntaxTypeStmt).Diagnostics.Should().NotHaveAnyDiagnostics("the generated medium strictness type string should compile successfully");
            }

            if (!ignoreStrict)
            {
                var actualStrictTypeStmt = $"type strict = {actualStrictTypeName}";
                actualStrictTypeStmt.Should().EqualIgnoringBicepFormatting(expectedStrictSyntax);
                Compile(actualStrictTypeStmt).Diagnostics.Should().NotHaveAnyDiagnostics("the generated strict type string should compile successfully");
            }

            string? actualResourceDerivedSyntaxTypeStmt = null;
            if (!ignoreResourceDerived)
            {
                if (expectedResourceDerivedSyntax is { })
                {
                    actualResourceDerivedTypeName.Should().NotBeNull($"expected {expectedResourceDerivedSyntax.Replace("type resourceDerived = ", "")}");
                    if (actualResourceDerivedTypeName is { })
                    {
                        actualResourceDerivedSyntaxTypeStmt = $"type resourceDerived = {actualResourceDerivedTypeName}";
                        actualResourceDerivedSyntaxTypeStmt.Should().EqualIgnoringBicepFormatting(expectedResourceDerivedSyntax);

                        /* TODO: Blocked by https://github.com/Azure/bicep/issues/15277
                        Compile(actualResourceDerivedSyntaxTypeStmt!).Diagnostics.Should().NotHaveAnyDiagnostics("the generated resource-derived type string should compile successfully");
                        */
                    }
                }
                else
                {
                    actualResourceDerivedTypeName.Should().BeNull("expected no resource-derived type to be found");
                }
            }
        }
    }

    private static IEnumerable<TSyntax> GetAllSyntaxOfType<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        => SyntaxVisitor.GetAllSyntaxOfType<TSyntax>(syntax);

    private class SyntaxVisitor : CstVisitor
    {
        private readonly List<SyntaxBase> syntaxList = new();

        private SyntaxVisitor()
        {
        }

        public static IEnumerable<TSyntax> GetAllSyntaxOfType<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        {
            var visitor = new SyntaxVisitor();
            visitor.Visit(syntax);

            return visitor.syntaxList.OfType<TSyntax>();
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            syntaxList.Add(syntax);
            base.VisitInternal(syntax);
        }

    }

    private static void DebugPrintAllSyntaxNodeTypes(SemanticModel semanticModel)
    {
        var allSyntaxNodes = GetAllSyntaxNodesVisitor.Build(semanticModel.Root.Syntax);
        foreach (var node in allSyntaxNodes.Where(s => s is not Token && s is not IdentifierSyntax))
        {
            Trace.WriteLine($"** {node.GetDebuggerDisplay().ReplaceNewlines(" ").TruncateWithEllipses(150)}");
            Trace.WriteLine($"  ... type info: {semanticModel.GetTypeInfo(node).Name}");
            Trace.WriteLine($"  ... declared type: {semanticModel.GetDeclaredType(node)?.Name}");
        }
    }

    private class GetAllSyntaxNodesVisitor : CstVisitor
    {
        private readonly List<SyntaxBase> syntaxList = new();

        public static ImmutableArray<SyntaxBase> Build(SyntaxBase syntax)
        {
            var visitor = new GetAllSyntaxNodesVisitor();
            visitor.Visit(syntax);

            return [.. visitor.syntaxList];
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            syntaxList.Add(syntax);
            base.VisitInternal(syntax);
        }
    }

    #endregion Support
}

