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
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Refactor;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = array",
        "type loose = array",
        "type medium = array",
        "type strict = array")]
    public void ArrayType(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = {}",
        "type loose = object",
        "type medium = object",
        "type strict = { }")]
    public void EmptyObject(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = []",
        "type loose = array",
        "type medium = array",
        "type strict = []")]
    public void EmptyArrays(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type testType = string?",
        "type loose = string?",
        "type medium = string?",
        "type strict = string?")]
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
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        "type loose = object",
        "type medium = { commandToExecute: string, fileUris: string[] }",
        "type strict = { commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1', fileUris: ['https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/writeblob.ps1?sas=abcd'] }",
        DisplayName = "virtual machine extensions settings property")]
    //
    // "properties" property
    //
    [DataRow(
        """
            var isWindowsOS = true
            var provisionExtensions = true
            param _artifactsLocation string
            @secure()
            param _artifactsLocationSasToken string

            resource testResource 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
              name: 'cse-windows/extension'
              location: 'location'
              properties: {
                publisher: 'Microsoft.Compute'
                type: 'CyustomScriptExtension'
                typeHandlerVersion: '1.8'
                autoUpgradeMinorVersion: true
                settings: {
                  fileUris: [
                    uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                  ]
                  commandToExecute: 'commandToExecute'
                }
              }
            }
            """,
        "properties",
        "type loose = object",
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
            }
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
            }
            """,
        DisplayName = "virtual machine extensions properties")]
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
        "type loose = object? /* Microsoft.Compute/virtualMachines */",
        "type medium = object? /* Microsoft.Compute/virtualMachines */",
        "type strict = object? /* Microsoft.Compute/virtualMachines */",
        DisplayName = "virtual machine entire object (via 'parent')")]
    public void ResourcePropertyTypes(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromResourceProperty(resourceDeclaration, resourcePropertyName, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        """
            type t1 = { abc: int }
            type testType = t1
            """,
        "type loose = object",
        "type medium = { abc: int }", // TODO: better would be "type medium = t1" but Bicep type system doesn't currently support it
        "type strict = { abc: int }" // TODO: better would be "type strict = t1" but Bicep type system doesn't currently support it
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
        "type strict = [ { t1Property: { a: string, b: string }, t2Property: { a: string, b: string }[] } ]"  // TODO: better would be "type strict = [ t3 ]" but Bicep type system doesn't currently support it
        )]
    [DataRow(
        """
            type t1 = { a: 'abc', b: 123 }
            type testType = { a: t1, b: [t1, t1] }
            """,
        "type loose = object", // TODO: better: "{ a: t1, b: [t1, t1] }"
        "type medium = { a: { a: string, b: int }, b: { a: string, b: int }[] }", // TODO: better: "{ a: t1, b: [t1, t1] }"
        "type strict = { a: { a: 'abc', b: 123 }, b: [{ a: 'abc', b: 123 }, { a: 'abc', b: 123 }] }")]
    public void NamedTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
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
        "type strict = { a: -10, b: 10, c: false, d: true }")]
    public void NegatedTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    #region Support

    // input is a type declaration statement for type "testType", e.g. "type testType = int"
    private static void RunTestFromTypeDeclaration(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        var compilationResult = CompilationHelper.Compile(typeDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
        var declaredType = semanticModel.GetDeclaredType(semanticModel.Root.TypeDeclarations.Single(t => t.Name == "testType").Value);
        declaredType.Should().NotBeNull();

        RunTestHelper(null, declaredType!, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    // input is a resource declaration for resource "testResource" and a property name such as "properties" that is exposed anywhere on the resource
    private static void RunTestFromResourceProperty(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        var compilationResult = CompilationHelper.Compile(resourceDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var resourceSyntax = semanticModel.Root.ResourceDeclarations.Single(r => r.Name == "testResource").DeclaringResource;

        var properties = GetAllSyntaxOfType<ObjectPropertySyntax>(resourceSyntax);
        var matchingProperty = properties.Single(p => p.Key is IdentifierSyntax id && id.NameEquals(resourcePropertyName));

        var inferredType = semanticModel.GetTypeInfo(matchingProperty.Value);
        var declaredType = semanticModel.GetDeclaredType(matchingProperty);
        var matchingPropertyType = declaredType is AnyType || declaredType == null ? inferredType : declaredType;
        matchingPropertyType.Should().NotBeNull();

        RunTestHelper(null, matchingPropertyType!, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    private static void RunTestHelper(TypeProperty? typeProperty, TypeSymbol typeSymbol, SemanticModel semanticModel, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        if (debugPrintAllSyntaxNodeTypes)
        {
            DebugPrintAllSyntaxNodeTypes(semanticModel);
        }

        var looseSyntax = TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Loose);
        var mediumStrictSyntax = TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Medium);
        var strictSyntax = TypeStringifier.Stringify(typeSymbol, typeProperty, TypeStringifier.Strictness.Strict);

        using (new AssertionScope())
        {
            CompilationHelper.Compile(expectedLooseSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected loose syntax should be error-free");
            CompilationHelper.Compile(expectedMediumStrictSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected medium strictness syntax should be error-free");
            CompilationHelper.Compile(expectedStrictSyntax).Diagnostics.Should().NotHaveAnyDiagnostics("expected strict syntax should be error-free");
        }

        using (new AssertionScope())
        {
            var actualLooseSyntaxType = $"type loose = {looseSyntax}";
            actualLooseSyntaxType.Should().EqualIgnoringBicepFormatting(expectedLooseSyntax);

            string actualMediumLooseSyntaxType = $"type medium = {mediumStrictSyntax}";
            actualMediumLooseSyntaxType.Should().EqualIgnoringBicepFormatting(expectedMediumStrictSyntax);

            string actualStrictSyntaxType = $"type strict = {strictSyntax}";
            actualStrictSyntaxType.Should().EqualIgnoringBicepFormatting(expectedStrictSyntax);

            CompilationHelper.Compile(actualLooseSyntaxType).Diagnostics.Should().NotHaveAnyDiagnostics("the generated loose type string should compile successfully");
            CompilationHelper.Compile(actualMediumLooseSyntaxType).Diagnostics.Should().NotHaveAnyDiagnostics("the generated medium strictness type string should compile successfully");
            CompilationHelper.Compile(actualStrictSyntaxType).Diagnostics.Should().NotHaveAnyDiagnostics("the generated loose strict string should compile successfully");

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

            return [..visitor.syntaxList];
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            syntaxList.Add(syntax);
            base.VisitInternal(syntax);
        }
    }

    #endregion Support
}

