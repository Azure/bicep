// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberResourcesRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void ParameterNameInFormattedMessage()
        {
            var ruleToTest = new MaxNumberResourcesRule();
            ruleToTest.GetMessage(1).Should().Be("Too many resources. Number of resources is limited to 1.");
        }

        private void CompileAndTest(string text, params string[] unusedParams)
        {
            AssertLinterRuleDiagnostics(MaxNumberResourcesRule.Code, text, diags =>
                {
                    if (unusedParams.Any())
                    {
                        diags.Should().HaveCount(unusedParams.Count());

                        var rule = new MaxNumberResourcesRule();
                        string[] expectedMessages = unusedParams.Select(p => rule.GetMessage(MaxNumberResourcesRule.MaxNumber)).ToArray();
                        diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                    }
                    else
                    {
                        diags.Should().BeEmpty();
                    }
                },
                new Options(OnCompileErrors: OnCompileErrors.Ignore));
        }

        [DataRow(@"
                    resource r1 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r1'
    }
    resource r2 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r2'
    }
    resource r3 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r3'
    }
    resource r4 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r4'
    }
    resource r5 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r5'
    }
    resource r6 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r6'
    }
    resource r7 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r7'
    }
    resource r8 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r8'
    }
    resource r9 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r9'
    }
    resource r10 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r10'
    }
    resource r11 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r11'
    }
    resource r12 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r12'
    }
    resource r13 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r13'
    }
    resource r14 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r14'
    }
    resource r15 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r15'
    }
    resource r16 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r16'
    }
    resource r17 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r17'
    }
    resource r18 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r18'
    }
    resource r19 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r19'
    }
    resource r20 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r20'
    }
    resource r21 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r21'
    }
    resource r22 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r22'
    }
    resource r23 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r23'
    }
    resource r24 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r24'
    }
    resource r25 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r25'
    }
    resource r26 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r26'
    }
    resource r27 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r27'
    }
    resource r28 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r28'
    }
    resource r29 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r29'
    }
    resource r30 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r30'
    }
    resource r31 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r31'
    }
    resource r32 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r32'
    }
    resource r33 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r33'
    }
    resource r34 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r34'
    }
    resource r35 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r35'
    }
    resource r36 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r36'
    }
    resource r37 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r37'
    }
    resource r38 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r38'
    }
    resource r39 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r39'
    }
    resource r40 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r40'
    }
    resource r41 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r41'
    }
    resource r42 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r42'
    }
    resource r43 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r43'
    }
    resource r44 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r44'
    }
    resource r45 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r45'
    }
    resource r46 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r46'
    }
    resource r47 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r47'
    }
    resource r48 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r48'
    }
    resource r49 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r49'
    }
    resource r50 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r50'
    }
    resource r51 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r51'
    }
    resource r52 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r52'
    }
    resource r53 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r53'
    }
    resource r54 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r54'
    }
    resource r55 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r55'
    }
    resource r56 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r56'
    }
    resource r57 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r57'
    }
    resource r58 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r58'
    }
    resource r59 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r59'
    }
    resource r60 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r60'
    }
    resource r61 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r61'
    }
    resource r62 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r62'
    }
    resource r63 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r63'
    }
    resource r64 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r64'
    }
    resource r65 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r65'
    }
    resource r66 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r66'
    }
    resource r67 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r67'
    }
    resource r68 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r68'
    }
    resource r69 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r69'
    }
    resource r70 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r70'
    }
    resource r71 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r71'
    }
    resource r72 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r72'
    }
    resource r73 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r73'
    }
    resource r74 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r74'
    }
    resource r75 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r75'
    }
    resource r76 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r76'
    }
    resource r77 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r77'
    }
    resource r78 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r78'
    }
    resource r79 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r79'
    }
    resource r80 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r80'
    }
    resource r81 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r81'
    }
    resource r82 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r82'
    }
    resource r83 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r83'
    }
    resource r84 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r84'
    }
    resource r85 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r85'
    }
    resource r86 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r86'
    }
    resource r87 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r87'
    }
    resource r88 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r88'
    }
    resource r89 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r89'
    }
    resource r90 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r90'
    }
    resource r91 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r91'
    }
    resource r92 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r92'
    }
    resource r93 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r93'
    }
    resource r94 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r94'
    }
    resource r95 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r95'
    }
    resource r96 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r96'
    }
    resource r97 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r97'
    }
    resource r98 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r98'
    }
    resource r99 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r99'
    }
    resource r100 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r100'
    }
    resource r101 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r101'
    }
    resource r102 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r102'
    }
    resource r103 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r103'
    }
    resource r104 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r104'
    }
    resource r105 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r105'
    }
    resource r106 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r106'
    }
    resource r107 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r107'
    }
    resource r108 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r108'
    }
    resource r109 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r109'
    }
    resource r110 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r110'
    }
    resource r111 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r111'
    }
    resource r112 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r112'
    }
    resource r113 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r113'
    }
    resource r114 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r114'
    }
    resource r115 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r115'
    }
    resource r116 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r116'
    }
    resource r117 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r117'
    }
    resource r118 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r118'
    }
    resource r119 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r119'
    }
    resource r120 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r120'
    }
    resource r121 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r121'
    }
    resource r122 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r122'
    }
    resource r123 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r123'
    }
    resource r124 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r124'
    }
    resource r125 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r125'
    }
    resource r126 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r126'
    }
    resource r127 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r127'
    }
    resource r128 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r128'
    }
    resource r129 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r129'
    }
    resource r130 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r130'
    }
    resource r131 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r131'
    }
    resource r132 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r132'
    }
    resource r133 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r133'
    }
    resource r134 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r134'
    }
    resource r135 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r135'
    }
    resource r136 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r136'
    }
    resource r137 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r137'
    }
    resource r138 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r138'
    }
    resource r139 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r139'
    }
    resource r140 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r140'
    }
    resource r141 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r141'
    }
    resource r142 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r142'
    }
    resource r143 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r143'
    }
    resource r144 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r144'
    }
    resource r145 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r145'
    }
    resource r146 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r146'
    }
    resource r147 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r147'
    }
    resource r148 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r148'
    }
    resource r149 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r149'
    }
    resource r150 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r150'
    }
    resource r151 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r151'
    }
    resource r152 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r152'
    }
    resource r153 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r153'
    }
    resource r154 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r154'
    }
    resource r155 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r155'
    }
    resource r156 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r156'
    }
    resource r157 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r157'
    }
    resource r158 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r158'
    }
    resource r159 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r159'
    }
    resource r160 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r160'
    }
    resource r161 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r161'
    }
    resource r162 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r162'
    }
    resource r163 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r163'
    }
    resource r164 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r164'
    }
    resource r165 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r165'
    }
    resource r166 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r166'
    }
    resource r167 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r167'
    }
    resource r168 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r168'
    }
    resource r169 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r169'
    }
    resource r170 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r170'
    }
    resource r171 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r171'
    }
    resource r172 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r172'
    }
    resource r173 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r173'
    }
    resource r174 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r174'
    }
    resource r175 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r175'
    }
    resource r176 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r176'
    }
    resource r177 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r177'
    }
    resource r178 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r178'
    }
    resource r179 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r179'
    }
    resource r180 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r180'
    }
    resource r181 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r181'
    }
    resource r182 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r182'
    }
    resource r183 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r183'
    }
    resource r184 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r184'
    }
    resource r185 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r185'
    }
    resource r186 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r186'
    }
    resource r187 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r187'
    }
    resource r188 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r188'
    }
    resource r189 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r189'
    }
    resource r190 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r190'
    }
    resource r191 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r191'
    }
    resource r192 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r192'
    }
    resource r193 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r193'
    }
    resource r194 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r194'
    }
    resource r195 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r195'
    }
    resource r196 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r196'
    }
    resource r197 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r197'
    }
    resource r198 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r198'
    }
    resource r199 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r199'
    }
    resource r200 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r200'
    }
    resource r201 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r201'
    }
    resource r202 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r202'
    }
    resource r203 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r203'
    }
    resource r204 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r204'
    }
    resource r205 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r205'
    }
    resource r206 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r206'
    }
    resource r207 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r207'
    }
    resource r208 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r208'
    }
    resource r209 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r209'
    }
    resource r210 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r210'
    }
    resource r211 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r211'
    }
    resource r212 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r212'
    }
    resource r213 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r213'
    }
    resource r214 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r214'
    }
    resource r215 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r215'
    }
    resource r216 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r216'
    }
    resource r217 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r217'
    }
    resource r218 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r218'
    }
    resource r219 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r219'
    }
    resource r220 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r220'
    }
    resource r221 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r221'
    }
    resource r222 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r222'
    }
    resource r223 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r223'
    }
    resource r224 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r224'
    }
    resource r225 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r225'
    }
    resource r226 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r226'
    }
    resource r227 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r227'
    }
    resource r228 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r228'
    }
    resource r229 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r229'
    }
    resource r230 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r230'
    }
    resource r231 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r231'
    }
    resource r232 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r232'
    }
    resource r233 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r233'
    }
    resource r234 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r234'
    }
    resource r235 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r235'
    }
    resource r236 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r236'
    }
    resource r237 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r237'
    }
    resource r238 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r238'
    }
    resource r239 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r239'
    }
    resource r240 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r240'
    }
    resource r241 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r241'
    }
    resource r242 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r242'
    }
    resource r243 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r243'
    }
    resource r244 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r244'
    }
    resource r245 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r245'
    }
    resource r246 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r246'
    }
    resource r247 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r247'
    }
    resource r248 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r248'
    }
    resource r249 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r249'
    }
    resource r250 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r250'
    }
    resource r251 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r251'
    }
    resource r252 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r252'
    }
    resource r253 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r253'
    }
    resource r254 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r254'
    }
    resource r255 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r255'
    }
    resource r256 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r256'
    }
    resource r257 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r257'
    }
    resource r258 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r258'
    }
    resource r259 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r259'
    }
    resource r260 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r260'
    }
    resource r261 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r261'
    }
    resource r262 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r262'
    }
    resource r263 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r263'
    }
    resource r264 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r264'
    }
    resource r265 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r265'
    }
    resource r266 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r266'
    }
    resource r267 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r267'
    }
    resource r268 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r268'
    }
    resource r269 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r269'
    }
    resource r270 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r270'
    }
    resource r271 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r271'
    }
    resource r272 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r272'
    }
    resource r273 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r273'
    }
    resource r274 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r274'
    }
    resource r275 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r275'
    }
    resource r276 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r276'
    }
    resource r277 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r277'
    }
    resource r278 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r278'
    }
    resource r279 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r279'
    }
    resource r280 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r280'
    }
    resource r281 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r281'
    }
    resource r282 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r282'
    }
    resource r283 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r283'
    }
    resource r284 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r284'
    }
    resource r285 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r285'
    }
    resource r286 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r286'
    }
    resource r287 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r287'
    }
    resource r288 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r288'
    }
    resource r289 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r289'
    }
    resource r290 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r290'
    }
    resource r291 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r291'
    }
    resource r292 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r292'
    }
    resource r293 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r293'
    }
    resource r294 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r294'
    }
    resource r295 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r295'
    }
    resource r296 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r296'
    }
    resource r297 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r297'
    }
    resource r298 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r298'
    }
    resource r299 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r299'
    }
    resource r300 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r300'
    }
    resource r301 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r301'
    }
    resource r302 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r302'
    }
    resource r303 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r303'
    }
    resource r304 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r304'
    }
    resource r305 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r305'
    }
    resource r306 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r306'
    }
    resource r307 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r307'
    }
    resource r308 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r308'
    }
    resource r309 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r309'
    }
    resource r310 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r310'
    }
    resource r311 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r311'
    }
    resource r312 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r312'
    }
    resource r313 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r313'
    }
    resource r314 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r314'
    }
    resource r315 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r315'
    }
    resource r316 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r316'
    }
    resource r317 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r317'
    }
    resource r318 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r318'
    }
    resource r319 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r319'
    }
    resource r320 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r320'
    }
    resource r321 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r321'
    }
    resource r322 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r322'
    }
    resource r323 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r323'
    }
    resource r324 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r324'
    }
    resource r325 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r325'
    }
    resource r326 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r326'
    }
    resource r327 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r327'
    }
    resource r328 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r328'
    }
    resource r329 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r329'
    }
    resource r330 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r330'
    }
    resource r331 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r331'
    }
    resource r332 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r332'
    }
    resource r333 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r333'
    }
    resource r334 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r334'
    }
    resource r335 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r335'
    }
    resource r336 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r336'
    }
    resource r337 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r337'
    }
    resource r338 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r338'
    }
    resource r339 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r339'
    }
    resource r340 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r340'
    }
    resource r341 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r341'
    }
    resource r342 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r342'
    }
    resource r343 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r343'
    }
    resource r344 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r344'
    }
    resource r345 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r345'
    }
    resource r346 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r346'
    }
    resource r347 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r347'
    }
    resource r348 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r348'
    }
    resource r349 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r349'
    }
    resource r350 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r350'
    }
    resource r351 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r351'
    }
    resource r352 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r352'
    }
    resource r353 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r353'
    }
    resource r354 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r354'
    }
    resource r355 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r355'
    }
    resource r356 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r356'
    }
    resource r357 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r357'
    }
    resource r358 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r358'
    }
    resource r359 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r359'
    }
    resource r360 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r360'
    }
    resource r361 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r361'
    }
    resource r362 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r362'
    }
    resource r363 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r363'
    }
    resource r364 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r364'
    }
    resource r365 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r365'
    }
    resource r366 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r366'
    }
    resource r367 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r367'
    }
    resource r368 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r368'
    }
    resource r369 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r369'
    }
    resource r370 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r370'
    }
    resource r371 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r371'
    }
    resource r372 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r372'
    }
    resource r373 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r373'
    }
    resource r374 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r374'
    }
    resource r375 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r375'
    }
    resource r376 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r376'
    }
    resource r377 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r377'
    }
    resource r378 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r378'
    }
    resource r379 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r379'
    }
    resource r380 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r380'
    }
    resource r381 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r381'
    }
    resource r382 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r382'
    }
    resource r383 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r383'
    }
    resource r384 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r384'
    }
    resource r385 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r385'
    }
    resource r386 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r386'
    }
    resource r387 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r387'
    }
    resource r388 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r388'
    }
    resource r389 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r389'
    }
    resource r390 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r390'
    }
    resource r391 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r391'
    }
    resource r392 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r392'
    }
    resource r393 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r393'
    }
    resource r394 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r394'
    }
    resource r395 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r395'
    }
    resource r396 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r396'
    }
    resource r397 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r397'
    }
    resource r398 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r398'
    }
    resource r399 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r399'
    }
    resource r400 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r400'
    }
    resource r401 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r401'
    }
    resource r402 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r402'
    }
    resource r403 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r403'
    }
    resource r404 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r404'
    }
    resource r405 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r405'
    }
    resource r406 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r406'
    }
    resource r407 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r407'
    }
    resource r408 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r408'
    }
    resource r409 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r409'
    }
    resource r410 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r410'
    }
    resource r411 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r411'
    }
    resource r412 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r412'
    }
    resource r413 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r413'
    }
    resource r414 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r414'
    }
    resource r415 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r415'
    }
    resource r416 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r416'
    }
    resource r417 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r417'
    }
    resource r418 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r418'
    }
    resource r419 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r419'
    }
    resource r420 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r420'
    }
    resource r421 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r421'
    }
    resource r422 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r422'
    }
    resource r423 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r423'
    }
    resource r424 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r424'
    }
    resource r425 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r425'
    }
    resource r426 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r426'
    }
    resource r427 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r427'
    }
    resource r428 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r428'
    }
    resource r429 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r429'
    }
    resource r430 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r430'
    }
    resource r431 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r431'
    }
    resource r432 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r432'
    }
    resource r433 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r433'
    }
    resource r434 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r434'
    }
    resource r435 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r435'
    }
    resource r436 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r436'
    }
    resource r437 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r437'
    }
    resource r438 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r438'
    }
    resource r439 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r439'
    }
    resource r440 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r440'
    }
    resource r441 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r441'
    }
    resource r442 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r442'
    }
    resource r443 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r443'
    }
    resource r444 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r444'
    }
    resource r445 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r445'
    }
    resource r446 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r446'
    }
    resource r447 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r447'
    }
    resource r448 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r448'
    }
    resource r449 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r449'
    }
    resource r450 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r450'
    }
    resource r451 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r451'
    }
    resource r452 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r452'
    }
    resource r453 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r453'
    }
    resource r454 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r454'
    }
    resource r455 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r455'
    }
    resource r456 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r456'
    }
    resource r457 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r457'
    }
    resource r458 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r458'
    }
    resource r459 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r459'
    }
    resource r460 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r460'
    }
    resource r461 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r461'
    }
    resource r462 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r462'
    }
    resource r463 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r463'
    }
    resource r464 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r464'
    }
    resource r465 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r465'
    }
    resource r466 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r466'
    }
    resource r467 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r467'
    }
    resource r468 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r468'
    }
    resource r469 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r469'
    }
    resource r470 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r470'
    }
    resource r471 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r471'
    }
    resource r472 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r472'
    }
    resource r473 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r473'
    }
    resource r474 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r474'
    }
    resource r475 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r475'
    }
    resource r476 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r476'
    }
    resource r477 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r477'
    }
    resource r478 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r478'
    }
    resource r479 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r479'
    }
    resource r480 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r480'
    }
    resource r481 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r481'
    }
    resource r482 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r482'
    }
    resource r483 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r483'
    }
    resource r484 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r484'
    }
    resource r485 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r485'
    }
    resource r486 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r486'
    }
    resource r487 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r487'
    }
    resource r488 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r488'
    }
    resource r489 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r489'
    }
    resource r490 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r490'
    }
    resource r491 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r491'
    }
    resource r492 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r492'
    }
    resource r493 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r493'
    }
    resource r494 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r494'
    }
    resource r495 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r495'
    }
    resource r496 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r496'
    }
    resource r497 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r497'
    }
    resource r498 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r498'
    }
    resource r499 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r499'
    }
    resource r500 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r500'
    }
    resource r501 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r501'
    }
    resource r502 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r502'
    }
    resource r503 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r503'
    }
    resource r504 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r504'
    }
    resource r505 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r505'
    }
    resource r506 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r506'
    }
    resource r507 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r507'
    }
    resource r508 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r508'
    }
    resource r509 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r509'
    }
    resource r510 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r510'
    }
    resource r511 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r511'
    }
    resource r512 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r512'
    }
    resource r513 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r513'
    }
    resource r514 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r514'
    }
    resource r515 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r515'
    }
    resource r516 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r516'
    }
    resource r517 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r517'
    }
    resource r518 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r518'
    }
    resource r519 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r519'
    }
    resource r520 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r520'
    }
    resource r521 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r521'
    }
    resource r522 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r522'
    }
    resource r523 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r523'
    }
    resource r524 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r524'
    }
    resource r525 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r525'
    }
    resource r526 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r526'
    }
    resource r527 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r527'
    }
    resource r528 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r528'
    }
    resource r529 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r529'
    }
    resource r530 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r530'
    }
    resource r531 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r531'
    }
    resource r532 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r532'
    }
    resource r533 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r533'
    }
    resource r534 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r534'
    }
    resource r535 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r535'
    }
    resource r536 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r536'
    }
    resource r537 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r537'
    }
    resource r538 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r538'
    }
    resource r539 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r539'
    }
    resource r540 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r540'
    }
    resource r541 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r541'
    }
    resource r542 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r542'
    }
    resource r543 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r543'
    }
    resource r544 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r544'
    }
    resource r545 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r545'
    }
    resource r546 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r546'
    }
    resource r547 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r547'
    }
    resource r548 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r548'
    }
    resource r549 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r549'
    }
    resource r550 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r550'
    }
    resource r551 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r551'
    }
    resource r552 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r552'
    }
    resource r553 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r553'
    }
    resource r554 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r554'
    }
    resource r555 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r555'
    }
    resource r556 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r556'
    }
    resource r557 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r557'
    }
    resource r558 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r558'
    }
    resource r559 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r559'
    }
    resource r560 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r560'
    }
    resource r561 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r561'
    }
    resource r562 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r562'
    }
    resource r563 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r563'
    }
    resource r564 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r564'
    }
    resource r565 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r565'
    }
    resource r566 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r566'
    }
    resource r567 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r567'
    }
    resource r568 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r568'
    }
    resource r569 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r569'
    }
    resource r570 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r570'
    }
    resource r571 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r571'
    }
    resource r572 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r572'
    }
    resource r573 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r573'
    }
    resource r574 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r574'
    }
    resource r575 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r575'
    }
    resource r576 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r576'
    }
    resource r577 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r577'
    }
    resource r578 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r578'
    }
    resource r579 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r579'
    }
    resource r580 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r580'
    }
    resource r581 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r581'
    }
    resource r582 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r582'
    }
    resource r583 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r583'
    }
    resource r584 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r584'
    }
    resource r585 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r585'
    }
    resource r586 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r586'
    }
    resource r587 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r587'
    }
    resource r588 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r588'
    }
    resource r589 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r589'
    }
    resource r590 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r590'
    }
    resource r591 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r591'
    }
    resource r592 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r592'
    }
    resource r593 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r593'
    }
    resource r594 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r594'
    }
    resource r595 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r595'
    }
    resource r596 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r596'
    }
    resource r597 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r597'
    }
    resource r598 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r598'
    }
    resource r599 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r599'
    }
    resource r600 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r600'
    }
    resource r601 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r601'
    }
    resource r602 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r602'
    }
    resource r603 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r603'
    }
    resource r604 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r604'
    }
    resource r605 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r605'
    }
    resource r606 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r606'
    }
    resource r607 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r607'
    }
    resource r608 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r608'
    }
    resource r609 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r609'
    }
    resource r610 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r610'
    }
    resource r611 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r611'
    }
    resource r612 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r612'
    }
    resource r613 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r613'
    }
    resource r614 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r614'
    }
    resource r615 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r615'
    }
    resource r616 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r616'
    }
    resource r617 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r617'
    }
    resource r618 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r618'
    }
    resource r619 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r619'
    }
    resource r620 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r620'
    }
    resource r621 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r621'
    }
    resource r622 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r622'
    }
    resource r623 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r623'
    }
    resource r624 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r624'
    }
    resource r625 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r625'
    }
    resource r626 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r626'
    }
    resource r627 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r627'
    }
    resource r628 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r628'
    }
    resource r629 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r629'
    }
    resource r630 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r630'
    }
    resource r631 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r631'
    }
    resource r632 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r632'
    }
    resource r633 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r633'
    }
    resource r634 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r634'
    }
    resource r635 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r635'
    }
    resource r636 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r636'
    }
    resource r637 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r637'
    }
    resource r638 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r638'
    }
    resource r639 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r639'
    }
    resource r640 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r640'
    }
    resource r641 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r641'
    }
    resource r642 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r642'
    }
    resource r643 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r643'
    }
    resource r644 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r644'
    }
    resource r645 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r645'
    }
    resource r646 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r646'
    }
    resource r647 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r647'
    }
    resource r648 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r648'
    }
    resource r649 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r649'
    }
    resource r650 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r650'
    }
    resource r651 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r651'
    }
    resource r652 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r652'
    }
    resource r653 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r653'
    }
    resource r654 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r654'
    }
    resource r655 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r655'
    }
    resource r656 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r656'
    }
    resource r657 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r657'
    }
    resource r658 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r658'
    }
    resource r659 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r659'
    }
    resource r660 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r660'
    }
    resource r661 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r661'
    }
    resource r662 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r662'
    }
    resource r663 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r663'
    }
    resource r664 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r664'
    }
    resource r665 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r665'
    }
    resource r666 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r666'
    }
    resource r667 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r667'
    }
    resource r668 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r668'
    }
    resource r669 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r669'
    }
    resource r670 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r670'
    }
    resource r671 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r671'
    }
    resource r672 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r672'
    }
    resource r673 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r673'
    }
    resource r674 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r674'
    }
    resource r675 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r675'
    }
    resource r676 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r676'
    }
    resource r677 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r677'
    }
    resource r678 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r678'
    }
    resource r679 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r679'
    }
    resource r680 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r680'
    }
    resource r681 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r681'
    }
    resource r682 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r682'
    }
    resource r683 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r683'
    }
    resource r684 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r684'
    }
    resource r685 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r685'
    }
    resource r686 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r686'
    }
    resource r687 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r687'
    }
    resource r688 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r688'
    }
    resource r689 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r689'
    }
    resource r690 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r690'
    }
    resource r691 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r691'
    }
    resource r692 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r692'
    }
    resource r693 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r693'
    }
    resource r694 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r694'
    }
    resource r695 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r695'
    }
    resource r696 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r696'
    }
    resource r697 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r697'
    }
    resource r698 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r698'
    }
    resource r699 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r699'
    }
    resource r700 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r700'
    }
    resource r701 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r701'
    }
    resource r702 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r702'
    }
    resource r703 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r703'
    }
    resource r704 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r704'
    }
    resource r705 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r705'
    }
    resource r706 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r706'
    }
    resource r707 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r707'
    }
    resource r708 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r708'
    }
    resource r709 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r709'
    }
    resource r710 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r710'
    }
    resource r711 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r711'
    }
    resource r712 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r712'
    }
    resource r713 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r713'
    }
    resource r714 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r714'
    }
    resource r715 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r715'
    }
    resource r716 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r716'
    }
    resource r717 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r717'
    }
    resource r718 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r718'
    }
    resource r719 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r719'
    }
    resource r720 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r720'
    }
    resource r721 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r721'
    }
    resource r722 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r722'
    }
    resource r723 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r723'
    }
    resource r724 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r724'
    }
    resource r725 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r725'
    }
    resource r726 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r726'
    }
    resource r727 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r727'
    }
    resource r728 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r728'
    }
    resource r729 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r729'
    }
    resource r730 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r730'
    }
    resource r731 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r731'
    }
    resource r732 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r732'
    }
    resource r733 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r733'
    }
    resource r734 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r734'
    }
    resource r735 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r735'
    }
    resource r736 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r736'
    }
    resource r737 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r737'
    }
    resource r738 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r738'
    }
    resource r739 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r739'
    }
    resource r740 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r740'
    }
    resource r741 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r741'
    }
    resource r742 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r742'
    }
    resource r743 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r743'
    }
    resource r744 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r744'
    }
    resource r745 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r745'
    }
    resource r746 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r746'
    }
    resource r747 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r747'
    }
    resource r748 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r748'
    }
    resource r749 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r749'
    }
    resource r750 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r750'
    }
    resource r751 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r751'
    }
    resource r752 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r752'
    }
    resource r753 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r753'
    }
    resource r754 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r754'
    }
    resource r755 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r755'
    }
    resource r756 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r756'
    }
    resource r757 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r757'
    }
    resource r758 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r758'
    }
    resource r759 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r759'
    }
    resource r760 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r760'
    }
    resource r761 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r761'
    }
    resource r762 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r762'
    }
    resource r763 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r763'
    }
    resource r764 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r764'
    }
    resource r765 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r765'
    }
    resource r766 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r766'
    }
    resource r767 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r767'
    }
    resource r768 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r768'
    }
    resource r769 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r769'
    }
    resource r770 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r770'
    }
    resource r771 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r771'
    }
    resource r772 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r772'
    }
    resource r773 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r773'
    }
    resource r774 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r774'
    }
    resource r775 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r775'
    }
    resource r776 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r776'
    }
    resource r777 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r777'
    }
    resource r778 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r778'
    }
    resource r779 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r779'
    }
    resource r780 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r780'
    }
    resource r781 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r781'
    }
    resource r782 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r782'
    }
    resource r783 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r783'
    }
    resource r784 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r784'
    }
    resource r785 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r785'
    }
    resource r786 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r786'
    }
    resource r787 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r787'
    }
    resource r788 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r788'
    }
    resource r789 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r789'
    }
    resource r790 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r790'
    }
    resource r791 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r791'
    }
    resource r792 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r792'
    }
    resource r793 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r793'
    }
    resource r794 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r794'
    }
    resource r795 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r795'
    }
    resource r796 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r796'
    }
    resource r797 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r797'
    }
    resource r798 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r798'
    }
    resource r799 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r799'
    }
    resource r800 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r800'
    }
    
        ")]
        [DataRow(@"
            resource r1 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r1'
    }
    resource r2 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r2'
    }
    resource r3 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r3'
    }
    resource r4 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r4'
    }
    resource r5 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r5'
    }
    resource r6 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r6'
    }
    resource r7 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r7'
    }
    resource r8 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r8'
    }
    resource r9 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r9'
    }
    resource r10 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r10'
    }
    resource r11 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r11'
    }
    resource r12 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r12'
    }
    resource r13 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r13'
    }
    resource r14 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r14'
    }
    resource r15 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r15'
    }
    resource r16 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r16'
    }
    resource r17 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r17'
    }
    resource r18 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r18'
    }
    resource r19 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r19'
    }
    resource r20 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r20'
    }
    resource r21 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r21'
    }
    resource r22 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r22'
    }
    resource r23 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r23'
    }
    resource r24 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r24'
    }
    resource r25 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r25'
    }
    resource r26 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r26'
    }
    resource r27 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r27'
    }
    resource r28 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r28'
    }
    resource r29 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r29'
    }
    resource r30 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r30'
    }
    resource r31 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r31'
    }
    resource r32 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r32'
    }
    resource r33 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r33'
    }
    resource r34 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r34'
    }
    resource r35 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r35'
    }
    resource r36 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r36'
    }
    resource r37 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r37'
    }
    resource r38 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r38'
    }
    resource r39 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r39'
    }
    resource r40 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r40'
    }
    resource r41 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r41'
    }
    resource r42 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r42'
    }
    resource r43 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r43'
    }
    resource r44 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r44'
    }
    resource r45 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r45'
    }
    resource r46 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r46'
    }
    resource r47 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r47'
    }
    resource r48 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r48'
    }
    resource r49 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r49'
    }
    resource r50 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r50'
    }
    resource r51 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r51'
    }
    resource r52 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r52'
    }
    resource r53 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r53'
    }
    resource r54 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r54'
    }
    resource r55 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r55'
    }
    resource r56 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r56'
    }
    resource r57 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r57'
    }
    resource r58 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r58'
    }
    resource r59 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r59'
    }
    resource r60 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r60'
    }
    resource r61 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r61'
    }
    resource r62 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r62'
    }
    resource r63 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r63'
    }
    resource r64 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r64'
    }
    resource r65 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r65'
    }
    resource r66 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r66'
    }
    resource r67 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r67'
    }
    resource r68 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r68'
    }
    resource r69 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r69'
    }
    resource r70 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r70'
    }
    resource r71 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r71'
    }
    resource r72 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r72'
    }
    resource r73 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r73'
    }
    resource r74 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r74'
    }
    resource r75 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r75'
    }
    resource r76 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r76'
    }
    resource r77 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r77'
    }
    resource r78 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r78'
    }
    resource r79 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r79'
    }
    resource r80 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r80'
    }
    resource r81 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r81'
    }
    resource r82 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r82'
    }
    resource r83 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r83'
    }
    resource r84 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r84'
    }
    resource r85 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r85'
    }
    resource r86 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r86'
    }
    resource r87 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r87'
    }
    resource r88 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r88'
    }
    resource r89 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r89'
    }
    resource r90 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r90'
    }
    resource r91 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r91'
    }
    resource r92 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r92'
    }
    resource r93 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r93'
    }
    resource r94 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r94'
    }
    resource r95 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r95'
    }
    resource r96 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r96'
    }
    resource r97 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r97'
    }
    resource r98 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r98'
    }
    resource r99 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r99'
    }
    resource r100 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r100'
    }
    resource r101 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r101'
    }
    resource r102 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r102'
    }
    resource r103 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r103'
    }
    resource r104 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r104'
    }
    resource r105 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r105'
    }
    resource r106 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r106'
    }
    resource r107 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r107'
    }
    resource r108 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r108'
    }
    resource r109 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r109'
    }
    resource r110 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r110'
    }
    resource r111 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r111'
    }
    resource r112 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r112'
    }
    resource r113 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r113'
    }
    resource r114 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r114'
    }
    resource r115 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r115'
    }
    resource r116 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r116'
    }
    resource r117 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r117'
    }
    resource r118 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r118'
    }
    resource r119 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r119'
    }
    resource r120 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r120'
    }
    resource r121 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r121'
    }
    resource r122 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r122'
    }
    resource r123 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r123'
    }
    resource r124 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r124'
    }
    resource r125 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r125'
    }
    resource r126 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r126'
    }
    resource r127 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r127'
    }
    resource r128 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r128'
    }
    resource r129 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r129'
    }
    resource r130 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r130'
    }
    resource r131 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r131'
    }
    resource r132 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r132'
    }
    resource r133 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r133'
    }
    resource r134 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r134'
    }
    resource r135 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r135'
    }
    resource r136 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r136'
    }
    resource r137 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r137'
    }
    resource r138 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r138'
    }
    resource r139 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r139'
    }
    resource r140 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r140'
    }
    resource r141 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r141'
    }
    resource r142 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r142'
    }
    resource r143 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r143'
    }
    resource r144 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r144'
    }
    resource r145 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r145'
    }
    resource r146 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r146'
    }
    resource r147 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r147'
    }
    resource r148 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r148'
    }
    resource r149 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r149'
    }
    resource r150 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r150'
    }
    resource r151 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r151'
    }
    resource r152 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r152'
    }
    resource r153 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r153'
    }
    resource r154 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r154'
    }
    resource r155 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r155'
    }
    resource r156 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r156'
    }
    resource r157 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r157'
    }
    resource r158 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r158'
    }
    resource r159 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r159'
    }
    resource r160 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r160'
    }
    resource r161 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r161'
    }
    resource r162 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r162'
    }
    resource r163 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r163'
    }
    resource r164 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r164'
    }
    resource r165 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r165'
    }
    resource r166 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r166'
    }
    resource r167 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r167'
    }
    resource r168 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r168'
    }
    resource r169 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r169'
    }
    resource r170 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r170'
    }
    resource r171 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r171'
    }
    resource r172 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r172'
    }
    resource r173 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r173'
    }
    resource r174 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r174'
    }
    resource r175 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r175'
    }
    resource r176 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r176'
    }
    resource r177 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r177'
    }
    resource r178 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r178'
    }
    resource r179 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r179'
    }
    resource r180 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r180'
    }
    resource r181 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r181'
    }
    resource r182 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r182'
    }
    resource r183 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r183'
    }
    resource r184 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r184'
    }
    resource r185 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r185'
    }
    resource r186 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r186'
    }
    resource r187 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r187'
    }
    resource r188 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r188'
    }
    resource r189 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r189'
    }
    resource r190 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r190'
    }
    resource r191 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r191'
    }
    resource r192 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r192'
    }
    resource r193 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r193'
    }
    resource r194 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r194'
    }
    resource r195 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r195'
    }
    resource r196 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r196'
    }
    resource r197 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r197'
    }
    resource r198 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r198'
    }
    resource r199 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r199'
    }
    resource r200 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r200'
    }
    resource r201 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r201'
    }
    resource r202 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r202'
    }
    resource r203 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r203'
    }
    resource r204 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r204'
    }
    resource r205 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r205'
    }
    resource r206 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r206'
    }
    resource r207 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r207'
    }
    resource r208 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r208'
    }
    resource r209 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r209'
    }
    resource r210 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r210'
    }
    resource r211 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r211'
    }
    resource r212 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r212'
    }
    resource r213 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r213'
    }
    resource r214 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r214'
    }
    resource r215 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r215'
    }
    resource r216 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r216'
    }
    resource r217 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r217'
    }
    resource r218 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r218'
    }
    resource r219 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r219'
    }
    resource r220 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r220'
    }
    resource r221 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r221'
    }
    resource r222 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r222'
    }
    resource r223 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r223'
    }
    resource r224 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r224'
    }
    resource r225 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r225'
    }
    resource r226 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r226'
    }
    resource r227 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r227'
    }
    resource r228 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r228'
    }
    resource r229 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r229'
    }
    resource r230 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r230'
    }
    resource r231 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r231'
    }
    resource r232 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r232'
    }
    resource r233 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r233'
    }
    resource r234 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r234'
    }
    resource r235 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r235'
    }
    resource r236 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r236'
    }
    resource r237 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r237'
    }
    resource r238 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r238'
    }
    resource r239 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r239'
    }
    resource r240 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r240'
    }
    resource r241 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r241'
    }
    resource r242 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r242'
    }
    resource r243 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r243'
    }
    resource r244 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r244'
    }
    resource r245 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r245'
    }
    resource r246 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r246'
    }
    resource r247 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r247'
    }
    resource r248 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r248'
    }
    resource r249 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r249'
    }
    resource r250 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r250'
    }
    resource r251 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r251'
    }
    resource r252 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r252'
    }
    resource r253 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r253'
    }
    resource r254 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r254'
    }
    resource r255 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r255'
    }
    resource r256 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r256'
    }
    resource r257 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r257'
    }
    resource r258 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r258'
    }
    resource r259 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r259'
    }
    resource r260 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r260'
    }
    resource r261 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r261'
    }
    resource r262 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r262'
    }
    resource r263 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r263'
    }
    resource r264 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r264'
    }
    resource r265 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r265'
    }
    resource r266 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r266'
    }
    resource r267 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r267'
    }
    resource r268 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r268'
    }
    resource r269 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r269'
    }
    resource r270 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r270'
    }
    resource r271 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r271'
    }
    resource r272 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r272'
    }
    resource r273 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r273'
    }
    resource r274 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r274'
    }
    resource r275 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r275'
    }
    resource r276 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r276'
    }
    resource r277 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r277'
    }
    resource r278 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r278'
    }
    resource r279 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r279'
    }
    resource r280 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r280'
    }
    resource r281 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r281'
    }
    resource r282 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r282'
    }
    resource r283 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r283'
    }
    resource r284 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r284'
    }
    resource r285 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r285'
    }
    resource r286 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r286'
    }
    resource r287 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r287'
    }
    resource r288 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r288'
    }
    resource r289 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r289'
    }
    resource r290 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r290'
    }
    resource r291 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r291'
    }
    resource r292 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r292'
    }
    resource r293 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r293'
    }
    resource r294 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r294'
    }
    resource r295 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r295'
    }
    resource r296 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r296'
    }
    resource r297 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r297'
    }
    resource r298 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r298'
    }
    resource r299 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r299'
    }
    resource r300 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r300'
    }
    resource r301 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r301'
    }
    resource r302 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r302'
    }
    resource r303 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r303'
    }
    resource r304 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r304'
    }
    resource r305 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r305'
    }
    resource r306 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r306'
    }
    resource r307 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r307'
    }
    resource r308 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r308'
    }
    resource r309 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r309'
    }
    resource r310 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r310'
    }
    resource r311 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r311'
    }
    resource r312 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r312'
    }
    resource r313 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r313'
    }
    resource r314 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r314'
    }
    resource r315 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r315'
    }
    resource r316 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r316'
    }
    resource r317 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r317'
    }
    resource r318 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r318'
    }
    resource r319 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r319'
    }
    resource r320 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r320'
    }
    resource r321 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r321'
    }
    resource r322 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r322'
    }
    resource r323 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r323'
    }
    resource r324 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r324'
    }
    resource r325 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r325'
    }
    resource r326 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r326'
    }
    resource r327 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r327'
    }
    resource r328 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r328'
    }
    resource r329 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r329'
    }
    resource r330 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r330'
    }
    resource r331 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r331'
    }
    resource r332 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r332'
    }
    resource r333 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r333'
    }
    resource r334 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r334'
    }
    resource r335 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r335'
    }
    resource r336 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r336'
    }
    resource r337 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r337'
    }
    resource r338 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r338'
    }
    resource r339 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r339'
    }
    resource r340 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r340'
    }
    resource r341 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r341'
    }
    resource r342 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r342'
    }
    resource r343 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r343'
    }
    resource r344 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r344'
    }
    resource r345 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r345'
    }
    resource r346 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r346'
    }
    resource r347 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r347'
    }
    resource r348 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r348'
    }
    resource r349 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r349'
    }
    resource r350 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r350'
    }
    resource r351 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r351'
    }
    resource r352 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r352'
    }
    resource r353 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r353'
    }
    resource r354 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r354'
    }
    resource r355 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r355'
    }
    resource r356 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r356'
    }
    resource r357 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r357'
    }
    resource r358 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r358'
    }
    resource r359 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r359'
    }
    resource r360 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r360'
    }
    resource r361 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r361'
    }
    resource r362 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r362'
    }
    resource r363 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r363'
    }
    resource r364 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r364'
    }
    resource r365 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r365'
    }
    resource r366 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r366'
    }
    resource r367 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r367'
    }
    resource r368 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r368'
    }
    resource r369 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r369'
    }
    resource r370 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r370'
    }
    resource r371 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r371'
    }
    resource r372 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r372'
    }
    resource r373 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r373'
    }
    resource r374 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r374'
    }
    resource r375 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r375'
    }
    resource r376 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r376'
    }
    resource r377 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r377'
    }
    resource r378 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r378'
    }
    resource r379 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r379'
    }
    resource r380 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r380'
    }
    resource r381 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r381'
    }
    resource r382 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r382'
    }
    resource r383 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r383'
    }
    resource r384 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r384'
    }
    resource r385 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r385'
    }
    resource r386 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r386'
    }
    resource r387 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r387'
    }
    resource r388 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r388'
    }
    resource r389 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r389'
    }
    resource r390 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r390'
    }
    resource r391 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r391'
    }
    resource r392 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r392'
    }
    resource r393 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r393'
    }
    resource r394 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r394'
    }
    resource r395 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r395'
    }
    resource r396 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r396'
    }
    resource r397 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r397'
    }
    resource r398 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r398'
    }
    resource r399 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r399'
    }
    resource r400 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r400'
    }
    resource r401 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r401'
    }
    resource r402 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r402'
    }
    resource r403 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r403'
    }
    resource r404 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r404'
    }
    resource r405 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r405'
    }
    resource r406 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r406'
    }
    resource r407 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r407'
    }
    resource r408 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r408'
    }
    resource r409 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r409'
    }
    resource r410 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r410'
    }
    resource r411 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r411'
    }
    resource r412 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r412'
    }
    resource r413 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r413'
    }
    resource r414 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r414'
    }
    resource r415 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r415'
    }
    resource r416 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r416'
    }
    resource r417 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r417'
    }
    resource r418 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r418'
    }
    resource r419 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r419'
    }
    resource r420 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r420'
    }
    resource r421 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r421'
    }
    resource r422 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r422'
    }
    resource r423 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r423'
    }
    resource r424 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r424'
    }
    resource r425 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r425'
    }
    resource r426 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r426'
    }
    resource r427 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r427'
    }
    resource r428 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r428'
    }
    resource r429 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r429'
    }
    resource r430 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r430'
    }
    resource r431 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r431'
    }
    resource r432 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r432'
    }
    resource r433 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r433'
    }
    resource r434 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r434'
    }
    resource r435 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r435'
    }
    resource r436 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r436'
    }
    resource r437 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r437'
    }
    resource r438 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r438'
    }
    resource r439 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r439'
    }
    resource r440 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r440'
    }
    resource r441 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r441'
    }
    resource r442 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r442'
    }
    resource r443 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r443'
    }
    resource r444 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r444'
    }
    resource r445 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r445'
    }
    resource r446 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r446'
    }
    resource r447 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r447'
    }
    resource r448 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r448'
    }
    resource r449 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r449'
    }
    resource r450 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r450'
    }
    resource r451 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r451'
    }
    resource r452 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r452'
    }
    resource r453 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r453'
    }
    resource r454 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r454'
    }
    resource r455 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r455'
    }
    resource r456 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r456'
    }
    resource r457 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r457'
    }
    resource r458 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r458'
    }
    resource r459 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r459'
    }
    resource r460 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r460'
    }
    resource r461 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r461'
    }
    resource r462 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r462'
    }
    resource r463 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r463'
    }
    resource r464 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r464'
    }
    resource r465 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r465'
    }
    resource r466 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r466'
    }
    resource r467 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r467'
    }
    resource r468 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r468'
    }
    resource r469 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r469'
    }
    resource r470 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r470'
    }
    resource r471 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r471'
    }
    resource r472 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r472'
    }
    resource r473 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r473'
    }
    resource r474 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r474'
    }
    resource r475 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r475'
    }
    resource r476 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r476'
    }
    resource r477 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r477'
    }
    resource r478 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r478'
    }
    resource r479 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r479'
    }
    resource r480 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r480'
    }
    resource r481 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r481'
    }
    resource r482 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r482'
    }
    resource r483 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r483'
    }
    resource r484 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r484'
    }
    resource r485 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r485'
    }
    resource r486 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r486'
    }
    resource r487 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r487'
    }
    resource r488 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r488'
    }
    resource r489 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r489'
    }
    resource r490 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r490'
    }
    resource r491 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r491'
    }
    resource r492 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r492'
    }
    resource r493 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r493'
    }
    resource r494 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r494'
    }
    resource r495 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r495'
    }
    resource r496 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r496'
    }
    resource r497 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r497'
    }
    resource r498 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r498'
    }
    resource r499 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r499'
    }
    resource r500 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r500'
    }
    resource r501 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r501'
    }
    resource r502 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r502'
    }
    resource r503 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r503'
    }
    resource r504 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r504'
    }
    resource r505 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r505'
    }
    resource r506 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r506'
    }
    resource r507 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r507'
    }
    resource r508 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r508'
    }
    resource r509 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r509'
    }
    resource r510 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r510'
    }
    resource r511 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r511'
    }
    resource r512 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r512'
    }
    resource r513 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r513'
    }
    resource r514 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r514'
    }
    resource r515 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r515'
    }
    resource r516 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r516'
    }
    resource r517 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r517'
    }
    resource r518 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r518'
    }
    resource r519 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r519'
    }
    resource r520 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r520'
    }
    resource r521 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r521'
    }
    resource r522 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r522'
    }
    resource r523 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r523'
    }
    resource r524 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r524'
    }
    resource r525 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r525'
    }
    resource r526 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r526'
    }
    resource r527 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r527'
    }
    resource r528 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r528'
    }
    resource r529 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r529'
    }
    resource r530 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r530'
    }
    resource r531 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r531'
    }
    resource r532 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r532'
    }
    resource r533 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r533'
    }
    resource r534 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r534'
    }
    resource r535 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r535'
    }
    resource r536 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r536'
    }
    resource r537 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r537'
    }
    resource r538 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r538'
    }
    resource r539 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r539'
    }
    resource r540 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r540'
    }
    resource r541 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r541'
    }
    resource r542 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r542'
    }
    resource r543 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r543'
    }
    resource r544 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r544'
    }
    resource r545 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r545'
    }
    resource r546 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r546'
    }
    resource r547 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r547'
    }
    resource r548 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r548'
    }
    resource r549 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r549'
    }
    resource r550 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r550'
    }
    resource r551 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r551'
    }
    resource r552 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r552'
    }
    resource r553 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r553'
    }
    resource r554 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r554'
    }
    resource r555 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r555'
    }
    resource r556 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r556'
    }
    resource r557 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r557'
    }
    resource r558 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r558'
    }
    resource r559 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r559'
    }
    resource r560 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r560'
    }
    resource r561 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r561'
    }
    resource r562 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r562'
    }
    resource r563 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r563'
    }
    resource r564 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r564'
    }
    resource r565 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r565'
    }
    resource r566 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r566'
    }
    resource r567 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r567'
    }
    resource r568 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r568'
    }
    resource r569 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r569'
    }
    resource r570 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r570'
    }
    resource r571 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r571'
    }
    resource r572 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r572'
    }
    resource r573 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r573'
    }
    resource r574 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r574'
    }
    resource r575 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r575'
    }
    resource r576 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r576'
    }
    resource r577 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r577'
    }
    resource r578 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r578'
    }
    resource r579 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r579'
    }
    resource r580 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r580'
    }
    resource r581 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r581'
    }
    resource r582 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r582'
    }
    resource r583 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r583'
    }
    resource r584 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r584'
    }
    resource r585 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r585'
    }
    resource r586 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r586'
    }
    resource r587 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r587'
    }
    resource r588 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r588'
    }
    resource r589 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r589'
    }
    resource r590 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r590'
    }
    resource r591 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r591'
    }
    resource r592 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r592'
    }
    resource r593 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r593'
    }
    resource r594 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r594'
    }
    resource r595 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r595'
    }
    resource r596 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r596'
    }
    resource r597 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r597'
    }
    resource r598 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r598'
    }
    resource r599 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r599'
    }
    resource r600 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r600'
    }
    resource r601 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r601'
    }
    resource r602 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r602'
    }
    resource r603 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r603'
    }
    resource r604 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r604'
    }
    resource r605 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r605'
    }
    resource r606 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r606'
    }
    resource r607 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r607'
    }
    resource r608 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r608'
    }
    resource r609 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r609'
    }
    resource r610 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r610'
    }
    resource r611 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r611'
    }
    resource r612 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r612'
    }
    resource r613 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r613'
    }
    resource r614 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r614'
    }
    resource r615 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r615'
    }
    resource r616 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r616'
    }
    resource r617 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r617'
    }
    resource r618 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r618'
    }
    resource r619 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r619'
    }
    resource r620 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r620'
    }
    resource r621 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r621'
    }
    resource r622 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r622'
    }
    resource r623 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r623'
    }
    resource r624 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r624'
    }
    resource r625 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r625'
    }
    resource r626 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r626'
    }
    resource r627 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r627'
    }
    resource r628 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r628'
    }
    resource r629 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r629'
    }
    resource r630 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r630'
    }
    resource r631 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r631'
    }
    resource r632 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r632'
    }
    resource r633 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r633'
    }
    resource r634 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r634'
    }
    resource r635 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r635'
    }
    resource r636 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r636'
    }
    resource r637 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r637'
    }
    resource r638 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r638'
    }
    resource r639 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r639'
    }
    resource r640 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r640'
    }
    resource r641 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r641'
    }
    resource r642 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r642'
    }
    resource r643 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r643'
    }
    resource r644 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r644'
    }
    resource r645 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r645'
    }
    resource r646 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r646'
    }
    resource r647 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r647'
    }
    resource r648 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r648'
    }
    resource r649 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r649'
    }
    resource r650 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r650'
    }
    resource r651 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r651'
    }
    resource r652 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r652'
    }
    resource r653 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r653'
    }
    resource r654 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r654'
    }
    resource r655 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r655'
    }
    resource r656 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r656'
    }
    resource r657 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r657'
    }
    resource r658 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r658'
    }
    resource r659 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r659'
    }
    resource r660 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r660'
    }
    resource r661 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r661'
    }
    resource r662 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r662'
    }
    resource r663 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r663'
    }
    resource r664 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r664'
    }
    resource r665 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r665'
    }
    resource r666 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r666'
    }
    resource r667 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r667'
    }
    resource r668 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r668'
    }
    resource r669 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r669'
    }
    resource r670 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r670'
    }
    resource r671 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r671'
    }
    resource r672 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r672'
    }
    resource r673 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r673'
    }
    resource r674 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r674'
    }
    resource r675 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r675'
    }
    resource r676 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r676'
    }
    resource r677 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r677'
    }
    resource r678 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r678'
    }
    resource r679 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r679'
    }
    resource r680 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r680'
    }
    resource r681 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r681'
    }
    resource r682 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r682'
    }
    resource r683 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r683'
    }
    resource r684 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r684'
    }
    resource r685 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r685'
    }
    resource r686 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r686'
    }
    resource r687 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r687'
    }
    resource r688 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r688'
    }
    resource r689 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r689'
    }
    resource r690 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r690'
    }
    resource r691 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r691'
    }
    resource r692 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r692'
    }
    resource r693 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r693'
    }
    resource r694 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r694'
    }
    resource r695 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r695'
    }
    resource r696 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r696'
    }
    resource r697 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r697'
    }
    resource r698 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r698'
    }
    resource r699 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r699'
    }
    resource r700 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r700'
    }
    resource r701 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r701'
    }
    resource r702 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r702'
    }
    resource r703 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r703'
    }
    resource r704 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r704'
    }
    resource r705 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r705'
    }
    resource r706 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r706'
    }
    resource r707 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r707'
    }
    resource r708 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r708'
    }
    resource r709 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r709'
    }
    resource r710 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r710'
    }
    resource r711 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r711'
    }
    resource r712 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r712'
    }
    resource r713 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r713'
    }
    resource r714 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r714'
    }
    resource r715 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r715'
    }
    resource r716 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r716'
    }
    resource r717 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r717'
    }
    resource r718 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r718'
    }
    resource r719 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r719'
    }
    resource r720 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r720'
    }
    resource r721 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r721'
    }
    resource r722 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r722'
    }
    resource r723 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r723'
    }
    resource r724 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r724'
    }
    resource r725 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r725'
    }
    resource r726 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r726'
    }
    resource r727 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r727'
    }
    resource r728 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r728'
    }
    resource r729 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r729'
    }
    resource r730 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r730'
    }
    resource r731 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r731'
    }
    resource r732 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r732'
    }
    resource r733 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r733'
    }
    resource r734 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r734'
    }
    resource r735 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r735'
    }
    resource r736 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r736'
    }
    resource r737 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r737'
    }
    resource r738 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r738'
    }
    resource r739 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r739'
    }
    resource r740 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r740'
    }
    resource r741 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r741'
    }
    resource r742 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r742'
    }
    resource r743 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r743'
    }
    resource r744 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r744'
    }
    resource r745 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r745'
    }
    resource r746 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r746'
    }
    resource r747 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r747'
    }
    resource r748 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r748'
    }
    resource r749 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r749'
    }
    resource r750 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r750'
    }
    resource r751 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r751'
    }
    resource r752 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r752'
    }
    resource r753 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r753'
    }
    resource r754 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r754'
    }
    resource r755 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r755'
    }
    resource r756 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r756'
    }
    resource r757 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r757'
    }
    resource r758 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r758'
    }
    resource r759 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r759'
    }
    resource r760 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r760'
    }
    resource r761 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r761'
    }
    resource r762 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r762'
    }
    resource r763 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r763'
    }
    resource r764 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r764'
    }
    resource r765 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r765'
    }
    resource r766 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r766'
    }
    resource r767 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r767'
    }
    resource r768 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r768'
    }
    resource r769 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r769'
    }
    resource r770 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r770'
    }
    resource r771 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r771'
    }
    resource r772 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r772'
    }
    resource r773 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r773'
    }
    resource r774 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r774'
    }
    resource r775 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r775'
    }
    resource r776 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r776'
    }
    resource r777 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r777'
    }
    resource r778 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r778'
    }
    resource r779 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r779'
    }
    resource r780 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r780'
    }
    resource r781 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r781'
    }
    resource r782 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r782'
    }
    resource r783 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r783'
    }
    resource r784 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r784'
    }
    resource r785 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r785'
    }
    resource r786 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r786'
    }
    resource r787 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r787'
    }
    resource r788 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r788'
    }
    resource r789 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r789'
    }
    resource r790 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r790'
    }
    resource r791 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r791'
    }
    resource r792 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r792'
    }
    resource r793 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r793'
    }
    resource r794 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r794'
    }
    resource r795 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r795'
    }
    resource r796 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r796'
    }
    resource r797 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r797'
    }
    resource r798 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r798'
    }
    resource r799 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r799'
    }
    resource r800 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r800'
    }
    resource r801 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r801'
    }
    
            ",
            "r1")]
        [DataRow(@"
            resource r1 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r1'
    }
    resource r2 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r2'
    }
    resource r3 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r3'
    }
    resource r4 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r4'
    }
    resource r5 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r5'
    }
    resource r6 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r6'
    }
    resource r7 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r7'
    }
    resource r8 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r8'
    }
    resource r9 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r9'
    }
    resource r10 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r10'
    }
    resource r11 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r11'
    }
    resource r12 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r12'
    }
    resource r13 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r13'
    }
    resource r14 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r14'
    }
    resource r15 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r15'
    }
    resource r16 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r16'
    }
    resource r17 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r17'
    }
    resource r18 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r18'
    }
    resource r19 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r19'
    }
    resource r20 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r20'
    }
    resource r21 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r21'
    }
    resource r22 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r22'
    }
    resource r23 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r23'
    }
    resource r24 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r24'
    }
    resource r25 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r25'
    }
    resource r26 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r26'
    }
    resource r27 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r27'
    }
    resource r28 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r28'
    }
    resource r29 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r29'
    }
    resource r30 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r30'
    }
    resource r31 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r31'
    }
    resource r32 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r32'
    }
    resource r33 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r33'
    }
    resource r34 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r34'
    }
    resource r35 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r35'
    }
    resource r36 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r36'
    }
    resource r37 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r37'
    }
    resource r38 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r38'
    }
    resource r39 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r39'
    }
    resource r40 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r40'
    }
    resource r41 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r41'
    }
    resource r42 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r42'
    }
    resource r43 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r43'
    }
    resource r44 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r44'
    }
    resource r45 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r45'
    }
    resource r46 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r46'
    }
    resource r47 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r47'
    }
    resource r48 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r48'
    }
    resource r49 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r49'
    }
    resource r50 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r50'
    }
    resource r51 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r51'
    }
    resource r52 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r52'
    }
    resource r53 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r53'
    }
    resource r54 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r54'
    }
    resource r55 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r55'
    }
    resource r56 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r56'
    }
    resource r57 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r57'
    }
    resource r58 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r58'
    }
    resource r59 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r59'
    }
    resource r60 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r60'
    }
    resource r61 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r61'
    }
    resource r62 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r62'
    }
    resource r63 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r63'
    }
    resource r64 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r64'
    }
    resource r65 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r65'
    }
    resource r66 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r66'
    }
    resource r67 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r67'
    }
    resource r68 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r68'
    }
    resource r69 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r69'
    }
    resource r70 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r70'
    }
    resource r71 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r71'
    }
    resource r72 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r72'
    }
    resource r73 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r73'
    }
    resource r74 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r74'
    }
    resource r75 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r75'
    }
    resource r76 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r76'
    }
    resource r77 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r77'
    }
    resource r78 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r78'
    }
    resource r79 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r79'
    }
    resource r80 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r80'
    }
    resource r81 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r81'
    }
    resource r82 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r82'
    }
    resource r83 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r83'
    }
    resource r84 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r84'
    }
    resource r85 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r85'
    }
    resource r86 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r86'
    }
    resource r87 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r87'
    }
    resource r88 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r88'
    }
    resource r89 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r89'
    }
    resource r90 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r90'
    }
    resource r91 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r91'
    }
    resource r92 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r92'
    }
    resource r93 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r93'
    }
    resource r94 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r94'
    }
    resource r95 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r95'
    }
    resource r96 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r96'
    }
    resource r97 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r97'
    }
    resource r98 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r98'
    }
    resource r99 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r99'
    }
    resource r100 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r100'
    }
    resource r101 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r101'
    }
    resource r102 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r102'
    }
    resource r103 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r103'
    }
    resource r104 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r104'
    }
    resource r105 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r105'
    }
    resource r106 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r106'
    }
    resource r107 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r107'
    }
    resource r108 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r108'
    }
    resource r109 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r109'
    }
    resource r110 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r110'
    }
    resource r111 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r111'
    }
    resource r112 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r112'
    }
    resource r113 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r113'
    }
    resource r114 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r114'
    }
    resource r115 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r115'
    }
    resource r116 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r116'
    }
    resource r117 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r117'
    }
    resource r118 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r118'
    }
    resource r119 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r119'
    }
    resource r120 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r120'
    }
    resource r121 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r121'
    }
    resource r122 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r122'
    }
    resource r123 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r123'
    }
    resource r124 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r124'
    }
    resource r125 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r125'
    }
    resource r126 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r126'
    }
    resource r127 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r127'
    }
    resource r128 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r128'
    }
    resource r129 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r129'
    }
    resource r130 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r130'
    }
    resource r131 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r131'
    }
    resource r132 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r132'
    }
    resource r133 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r133'
    }
    resource r134 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r134'
    }
    resource r135 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r135'
    }
    resource r136 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r136'
    }
    resource r137 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r137'
    }
    resource r138 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r138'
    }
    resource r139 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r139'
    }
    resource r140 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r140'
    }
    resource r141 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r141'
    }
    resource r142 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r142'
    }
    resource r143 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r143'
    }
    resource r144 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r144'
    }
    resource r145 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r145'
    }
    resource r146 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r146'
    }
    resource r147 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r147'
    }
    resource r148 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r148'
    }
    resource r149 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r149'
    }
    resource r150 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r150'
    }
    resource r151 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r151'
    }
    resource r152 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r152'
    }
    resource r153 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r153'
    }
    resource r154 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r154'
    }
    resource r155 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r155'
    }
    resource r156 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r156'
    }
    resource r157 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r157'
    }
    resource r158 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r158'
    }
    resource r159 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r159'
    }
    resource r160 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r160'
    }
    resource r161 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r161'
    }
    resource r162 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r162'
    }
    resource r163 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r163'
    }
    resource r164 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r164'
    }
    resource r165 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r165'
    }
    resource r166 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r166'
    }
    resource r167 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r167'
    }
    resource r168 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r168'
    }
    resource r169 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r169'
    }
    resource r170 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r170'
    }
    resource r171 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r171'
    }
    resource r172 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r172'
    }
    resource r173 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r173'
    }
    resource r174 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r174'
    }
    resource r175 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r175'
    }
    resource r176 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r176'
    }
    resource r177 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r177'
    }
    resource r178 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r178'
    }
    resource r179 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r179'
    }
    resource r180 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r180'
    }
    resource r181 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r181'
    }
    resource r182 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r182'
    }
    resource r183 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r183'
    }
    resource r184 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r184'
    }
    resource r185 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r185'
    }
    resource r186 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r186'
    }
    resource r187 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r187'
    }
    resource r188 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r188'
    }
    resource r189 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r189'
    }
    resource r190 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r190'
    }
    resource r191 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r191'
    }
    resource r192 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r192'
    }
    resource r193 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r193'
    }
    resource r194 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r194'
    }
    resource r195 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r195'
    }
    resource r196 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r196'
    }
    resource r197 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r197'
    }
    resource r198 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r198'
    }
    resource r199 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r199'
    }
    resource r200 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r200'
    }
    resource r201 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r201'
    }
    resource r202 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r202'
    }
    resource r203 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r203'
    }
    resource r204 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r204'
    }
    resource r205 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r205'
    }
    resource r206 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r206'
    }
    resource r207 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r207'
    }
    resource r208 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r208'
    }
    resource r209 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r209'
    }
    resource r210 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r210'
    }
    resource r211 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r211'
    }
    resource r212 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r212'
    }
    resource r213 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r213'
    }
    resource r214 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r214'
    }
    resource r215 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r215'
    }
    resource r216 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r216'
    }
    resource r217 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r217'
    }
    resource r218 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r218'
    }
    resource r219 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r219'
    }
    resource r220 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r220'
    }
    resource r221 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r221'
    }
    resource r222 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r222'
    }
    resource r223 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r223'
    }
    resource r224 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r224'
    }
    resource r225 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r225'
    }
    resource r226 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r226'
    }
    resource r227 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r227'
    }
    resource r228 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r228'
    }
    resource r229 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r229'
    }
    resource r230 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r230'
    }
    resource r231 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r231'
    }
    resource r232 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r232'
    }
    resource r233 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r233'
    }
    resource r234 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r234'
    }
    resource r235 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r235'
    }
    resource r236 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r236'
    }
    resource r237 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r237'
    }
    resource r238 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r238'
    }
    resource r239 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r239'
    }
    resource r240 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r240'
    }
    resource r241 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r241'
    }
    resource r242 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r242'
    }
    resource r243 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r243'
    }
    resource r244 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r244'
    }
    resource r245 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r245'
    }
    resource r246 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r246'
    }
    resource r247 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r247'
    }
    resource r248 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r248'
    }
    resource r249 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r249'
    }
    resource r250 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r250'
    }
    resource r251 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r251'
    }
    resource r252 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r252'
    }
    resource r253 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r253'
    }
    resource r254 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r254'
    }
    resource r255 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r255'
    }
    resource r256 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r256'
    }
    resource r257 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r257'
    }
    resource r258 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r258'
    }
    resource r259 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r259'
    }
    resource r260 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r260'
    }
    resource r261 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r261'
    }
    resource r262 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r262'
    }
    resource r263 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r263'
    }
    resource r264 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r264'
    }
    resource r265 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r265'
    }
    resource r266 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r266'
    }
    resource r267 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r267'
    }
    resource r268 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r268'
    }
    resource r269 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r269'
    }
    resource r270 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r270'
    }
    resource r271 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r271'
    }
    resource r272 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r272'
    }
    resource r273 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r273'
    }
    resource r274 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r274'
    }
    resource r275 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r275'
    }
    resource r276 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r276'
    }
    resource r277 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r277'
    }
    resource r278 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r278'
    }
    resource r279 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r279'
    }
    resource r280 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r280'
    }
    resource r281 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r281'
    }
    resource r282 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r282'
    }
    resource r283 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r283'
    }
    resource r284 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r284'
    }
    resource r285 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r285'
    }
    resource r286 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r286'
    }
    resource r287 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r287'
    }
    resource r288 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r288'
    }
    resource r289 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r289'
    }
    resource r290 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r290'
    }
    resource r291 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r291'
    }
    resource r292 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r292'
    }
    resource r293 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r293'
    }
    resource r294 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r294'
    }
    resource r295 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r295'
    }
    resource r296 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r296'
    }
    resource r297 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r297'
    }
    resource r298 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r298'
    }
    resource r299 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r299'
    }
    resource r300 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r300'
    }
    resource r301 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r301'
    }
    resource r302 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r302'
    }
    resource r303 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r303'
    }
    resource r304 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r304'
    }
    resource r305 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r305'
    }
    resource r306 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r306'
    }
    resource r307 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r307'
    }
    resource r308 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r308'
    }
    resource r309 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r309'
    }
    resource r310 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r310'
    }
    resource r311 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r311'
    }
    resource r312 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r312'
    }
    resource r313 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r313'
    }
    resource r314 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r314'
    }
    resource r315 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r315'
    }
    resource r316 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r316'
    }
    resource r317 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r317'
    }
    resource r318 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r318'
    }
    resource r319 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r319'
    }
    resource r320 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r320'
    }
    resource r321 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r321'
    }
    resource r322 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r322'
    }
    resource r323 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r323'
    }
    resource r324 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r324'
    }
    resource r325 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r325'
    }
    resource r326 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r326'
    }
    resource r327 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r327'
    }
    resource r328 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r328'
    }
    resource r329 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r329'
    }
    resource r330 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r330'
    }
    resource r331 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r331'
    }
    resource r332 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r332'
    }
    resource r333 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r333'
    }
    resource r334 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r334'
    }
    resource r335 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r335'
    }
    resource r336 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r336'
    }
    resource r337 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r337'
    }
    resource r338 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r338'
    }
    resource r339 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r339'
    }
    resource r340 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r340'
    }
    resource r341 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r341'
    }
    resource r342 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r342'
    }
    resource r343 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r343'
    }
    resource r344 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r344'
    }
    resource r345 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r345'
    }
    resource r346 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r346'
    }
    resource r347 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r347'
    }
    resource r348 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r348'
    }
    resource r349 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r349'
    }
    resource r350 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r350'
    }
    resource r351 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r351'
    }
    resource r352 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r352'
    }
    resource r353 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r353'
    }
    resource r354 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r354'
    }
    resource r355 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r355'
    }
    resource r356 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r356'
    }
    resource r357 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r357'
    }
    resource r358 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r358'
    }
    resource r359 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r359'
    }
    resource r360 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r360'
    }
    resource r361 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r361'
    }
    resource r362 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r362'
    }
    resource r363 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r363'
    }
    resource r364 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r364'
    }
    resource r365 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r365'
    }
    resource r366 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r366'
    }
    resource r367 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r367'
    }
    resource r368 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r368'
    }
    resource r369 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r369'
    }
    resource r370 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r370'
    }
    resource r371 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r371'
    }
    resource r372 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r372'
    }
    resource r373 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r373'
    }
    resource r374 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r374'
    }
    resource r375 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r375'
    }
    resource r376 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r376'
    }
    resource r377 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r377'
    }
    resource r378 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r378'
    }
    resource r379 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r379'
    }
    resource r380 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r380'
    }
    resource r381 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r381'
    }
    resource r382 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r382'
    }
    resource r383 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r383'
    }
    resource r384 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r384'
    }
    resource r385 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r385'
    }
    resource r386 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r386'
    }
    resource r387 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r387'
    }
    resource r388 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r388'
    }
    resource r389 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r389'
    }
    resource r390 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r390'
    }
    resource r391 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r391'
    }
    resource r392 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r392'
    }
    resource r393 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r393'
    }
    resource r394 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r394'
    }
    resource r395 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r395'
    }
    resource r396 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r396'
    }
    resource r397 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r397'
    }
    resource r398 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r398'
    }
    resource r399 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r399'
    }
    resource r400 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r400'
    }
    resource r401 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r401'
    }
    resource r402 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r402'
    }
    resource r403 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r403'
    }
    resource r404 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r404'
    }
    resource r405 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r405'
    }
    resource r406 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r406'
    }
    resource r407 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r407'
    }
    resource r408 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r408'
    }
    resource r409 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r409'
    }
    resource r410 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r410'
    }
    resource r411 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r411'
    }
    resource r412 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r412'
    }
    resource r413 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r413'
    }
    resource r414 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r414'
    }
    resource r415 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r415'
    }
    resource r416 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r416'
    }
    resource r417 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r417'
    }
    resource r418 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r418'
    }
    resource r419 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r419'
    }
    resource r420 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r420'
    }
    resource r421 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r421'
    }
    resource r422 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r422'
    }
    resource r423 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r423'
    }
    resource r424 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r424'
    }
    resource r425 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r425'
    }
    resource r426 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r426'
    }
    resource r427 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r427'
    }
    resource r428 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r428'
    }
    resource r429 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r429'
    }
    resource r430 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r430'
    }
    resource r431 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r431'
    }
    resource r432 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r432'
    }
    resource r433 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r433'
    }
    resource r434 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r434'
    }
    resource r435 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r435'
    }
    resource r436 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r436'
    }
    resource r437 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r437'
    }
    resource r438 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r438'
    }
    resource r439 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r439'
    }
    resource r440 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r440'
    }
    resource r441 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r441'
    }
    resource r442 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r442'
    }
    resource r443 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r443'
    }
    resource r444 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r444'
    }
    resource r445 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r445'
    }
    resource r446 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r446'
    }
    resource r447 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r447'
    }
    resource r448 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r448'
    }
    resource r449 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r449'
    }
    resource r450 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r450'
    }
    resource r451 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r451'
    }
    resource r452 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r452'
    }
    resource r453 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r453'
    }
    resource r454 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r454'
    }
    resource r455 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r455'
    }
    resource r456 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r456'
    }
    resource r457 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r457'
    }
    resource r458 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r458'
    }
    resource r459 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r459'
    }
    resource r460 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r460'
    }
    resource r461 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r461'
    }
    resource r462 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r462'
    }
    resource r463 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r463'
    }
    resource r464 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r464'
    }
    resource r465 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r465'
    }
    resource r466 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r466'
    }
    resource r467 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r467'
    }
    resource r468 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r468'
    }
    resource r469 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r469'
    }
    resource r470 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r470'
    }
    resource r471 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r471'
    }
    resource r472 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r472'
    }
    resource r473 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r473'
    }
    resource r474 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r474'
    }
    resource r475 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r475'
    }
    resource r476 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r476'
    }
    resource r477 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r477'
    }
    resource r478 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r478'
    }
    resource r479 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r479'
    }
    resource r480 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r480'
    }
    resource r481 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r481'
    }
    resource r482 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r482'
    }
    resource r483 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r483'
    }
    resource r484 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r484'
    }
    resource r485 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r485'
    }
    resource r486 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r486'
    }
    resource r487 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r487'
    }
    resource r488 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r488'
    }
    resource r489 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r489'
    }
    resource r490 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r490'
    }
    resource r491 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r491'
    }
    resource r492 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r492'
    }
    resource r493 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r493'
    }
    resource r494 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r494'
    }
    resource r495 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r495'
    }
    resource r496 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r496'
    }
    resource r497 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r497'
    }
    resource r498 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r498'
    }
    resource r499 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r499'
    }
    resource r500 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r500'
    }
    resource r501 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r501'
    }
    resource r502 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r502'
    }
    resource r503 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r503'
    }
    resource r504 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r504'
    }
    resource r505 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r505'
    }
    resource r506 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r506'
    }
    resource r507 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r507'
    }
    resource r508 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r508'
    }
    resource r509 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r509'
    }
    resource r510 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r510'
    }
    resource r511 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r511'
    }
    resource r512 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r512'
    }
    resource r513 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r513'
    }
    resource r514 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r514'
    }
    resource r515 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r515'
    }
    resource r516 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r516'
    }
    resource r517 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r517'
    }
    resource r518 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r518'
    }
    resource r519 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r519'
    }
    resource r520 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r520'
    }
    resource r521 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r521'
    }
    resource r522 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r522'
    }
    resource r523 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r523'
    }
    resource r524 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r524'
    }
    resource r525 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r525'
    }
    resource r526 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r526'
    }
    resource r527 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r527'
    }
    resource r528 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r528'
    }
    resource r529 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r529'
    }
    resource r530 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r530'
    }
    resource r531 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r531'
    }
    resource r532 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r532'
    }
    resource r533 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r533'
    }
    resource r534 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r534'
    }
    resource r535 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r535'
    }
    resource r536 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r536'
    }
    resource r537 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r537'
    }
    resource r538 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r538'
    }
    resource r539 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r539'
    }
    resource r540 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r540'
    }
    resource r541 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r541'
    }
    resource r542 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r542'
    }
    resource r543 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r543'
    }
    resource r544 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r544'
    }
    resource r545 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r545'
    }
    resource r546 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r546'
    }
    resource r547 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r547'
    }
    resource r548 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r548'
    }
    resource r549 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r549'
    }
    resource r550 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r550'
    }
    resource r551 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r551'
    }
    resource r552 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r552'
    }
    resource r553 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r553'
    }
    resource r554 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r554'
    }
    resource r555 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r555'
    }
    resource r556 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r556'
    }
    resource r557 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r557'
    }
    resource r558 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r558'
    }
    resource r559 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r559'
    }
    resource r560 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r560'
    }
    resource r561 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r561'
    }
    resource r562 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r562'
    }
    resource r563 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r563'
    }
    resource r564 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r564'
    }
    resource r565 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r565'
    }
    resource r566 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r566'
    }
    resource r567 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r567'
    }
    resource r568 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r568'
    }
    resource r569 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r569'
    }
    resource r570 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r570'
    }
    resource r571 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r571'
    }
    resource r572 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r572'
    }
    resource r573 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r573'
    }
    resource r574 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r574'
    }
    resource r575 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r575'
    }
    resource r576 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r576'
    }
    resource r577 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r577'
    }
    resource r578 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r578'
    }
    resource r579 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r579'
    }
    resource r580 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r580'
    }
    resource r581 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r581'
    }
    resource r582 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r582'
    }
    resource r583 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r583'
    }
    resource r584 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r584'
    }
    resource r585 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r585'
    }
    resource r586 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r586'
    }
    resource r587 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r587'
    }
    resource r588 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r588'
    }
    resource r589 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r589'
    }
    resource r590 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r590'
    }
    resource r591 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r591'
    }
    resource r592 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r592'
    }
    resource r593 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r593'
    }
    resource r594 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r594'
    }
    resource r595 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r595'
    }
    resource r596 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r596'
    }
    resource r597 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r597'
    }
    resource r598 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r598'
    }
    resource r599 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r599'
    }
    resource r600 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r600'
    }
    resource r601 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r601'
    }
    resource r602 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r602'
    }
    resource r603 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r603'
    }
    resource r604 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r604'
    }
    resource r605 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r605'
    }
    resource r606 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r606'
    }
    resource r607 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r607'
    }
    resource r608 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r608'
    }
    resource r609 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r609'
    }
    resource r610 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r610'
    }
    resource r611 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r611'
    }
    resource r612 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r612'
    }
    resource r613 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r613'
    }
    resource r614 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r614'
    }
    resource r615 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r615'
    }
    resource r616 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r616'
    }
    resource r617 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r617'
    }
    resource r618 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r618'
    }
    resource r619 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r619'
    }
    resource r620 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r620'
    }
    resource r621 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r621'
    }
    resource r622 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r622'
    }
    resource r623 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r623'
    }
    resource r624 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r624'
    }
    resource r625 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r625'
    }
    resource r626 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r626'
    }
    resource r627 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r627'
    }
    resource r628 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r628'
    }
    resource r629 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r629'
    }
    resource r630 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r630'
    }
    resource r631 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r631'
    }
    resource r632 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r632'
    }
    resource r633 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r633'
    }
    resource r634 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r634'
    }
    resource r635 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r635'
    }
    resource r636 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r636'
    }
    resource r637 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r637'
    }
    resource r638 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r638'
    }
    resource r639 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r639'
    }
    resource r640 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r640'
    }
    resource r641 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r641'
    }
    resource r642 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r642'
    }
    resource r643 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r643'
    }
    resource r644 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r644'
    }
    resource r645 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r645'
    }
    resource r646 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r646'
    }
    resource r647 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r647'
    }
    resource r648 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r648'
    }
    resource r649 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r649'
    }
    resource r650 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r650'
    }
    resource r651 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r651'
    }
    resource r652 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r652'
    }
    resource r653 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r653'
    }
    resource r654 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r654'
    }
    resource r655 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r655'
    }
    resource r656 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r656'
    }
    resource r657 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r657'
    }
    resource r658 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r658'
    }
    resource r659 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r659'
    }
    resource r660 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r660'
    }
    resource r661 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r661'
    }
    resource r662 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r662'
    }
    resource r663 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r663'
    }
    resource r664 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r664'
    }
    resource r665 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r665'
    }
    resource r666 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r666'
    }
    resource r667 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r667'
    }
    resource r668 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r668'
    }
    resource r669 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r669'
    }
    resource r670 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r670'
    }
    resource r671 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r671'
    }
    resource r672 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r672'
    }
    resource r673 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r673'
    }
    resource r674 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r674'
    }
    resource r675 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r675'
    }
    resource r676 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r676'
    }
    resource r677 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r677'
    }
    resource r678 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r678'
    }
    resource r679 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r679'
    }
    resource r680 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r680'
    }
    resource r681 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r681'
    }
    resource r682 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r682'
    }
    resource r683 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r683'
    }
    resource r684 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r684'
    }
    resource r685 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r685'
    }
    resource r686 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r686'
    }
    resource r687 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r687'
    }
    resource r688 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r688'
    }
    resource r689 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r689'
    }
    resource r690 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r690'
    }
    resource r691 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r691'
    }
    resource r692 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r692'
    }
    resource r693 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r693'
    }
    resource r694 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r694'
    }
    resource r695 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r695'
    }
    resource r696 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r696'
    }
    resource r697 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r697'
    }
    resource r698 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r698'
    }
    resource r699 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r699'
    }
    resource r700 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r700'
    }
    resource r701 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r701'
    }
    resource r702 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r702'
    }
    resource r703 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r703'
    }
    resource r704 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r704'
    }
    resource r705 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r705'
    }
    resource r706 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r706'
    }
    resource r707 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r707'
    }
    resource r708 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r708'
    }
    resource r709 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r709'
    }
    resource r710 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r710'
    }
    resource r711 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r711'
    }
    resource r712 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r712'
    }
    resource r713 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r713'
    }
    resource r714 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r714'
    }
    resource r715 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r715'
    }
    resource r716 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r716'
    }
    resource r717 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r717'
    }
    resource r718 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r718'
    }
    resource r719 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r719'
    }
    resource r720 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r720'
    }
    resource r721 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r721'
    }
    resource r722 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r722'
    }
    resource r723 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r723'
    }
    resource r724 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r724'
    }
    resource r725 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r725'
    }
    resource r726 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r726'
    }
    resource r727 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r727'
    }
    resource r728 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r728'
    }
    resource r729 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r729'
    }
    resource r730 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r730'
    }
    resource r731 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r731'
    }
    resource r732 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r732'
    }
    resource r733 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r733'
    }
    resource r734 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r734'
    }
    resource r735 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r735'
    }
    resource r736 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r736'
    }
    resource r737 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r737'
    }
    resource r738 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r738'
    }
    resource r739 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r739'
    }
    resource r740 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r740'
    }
    resource r741 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r741'
    }
    resource r742 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r742'
    }
    resource r743 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r743'
    }
    resource r744 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r744'
    }
    resource r745 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r745'
    }
    resource r746 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r746'
    }
    resource r747 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r747'
    }
    resource r748 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r748'
    }
    resource r749 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r749'
    }
    resource r750 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r750'
    }
    resource r751 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r751'
    }
    resource r752 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r752'
    }
    resource r753 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r753'
    }
    resource r754 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r754'
    }
    resource r755 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r755'
    }
    resource r756 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r756'
    }
    resource r757 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r757'
    }
    resource r758 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r758'
    }
    resource r759 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r759'
    }
    resource r760 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r760'
    }
    resource r761 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r761'
    }
    resource r762 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r762'
    }
    resource r763 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r763'
    }
    resource r764 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r764'
    }
    resource r765 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r765'
    }
    resource r766 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r766'
    }
    resource r767 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r767'
    }
    resource r768 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r768'
    }
    resource r769 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r769'
    }
    resource r770 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r770'
    }
    resource r771 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r771'
    }
    resource r772 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r772'
    }
    resource r773 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r773'
    }
    resource r774 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r774'
    }
    resource r775 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r775'
    }
    resource r776 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r776'
    }
    resource r777 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r777'
    }
    resource r778 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r778'
    }
    resource r779 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r779'
    }
    resource r780 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r780'
    }
    resource r781 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r781'
    }
    resource r782 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r782'
    }
    resource r783 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r783'
    }
    resource r784 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r784'
    }
    resource r785 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r785'
    }
    resource r786 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r786'
    }
    resource r787 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r787'
    }
    resource r788 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r788'
    }
    resource r789 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r789'
    }
    resource r790 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r790'
    }
    resource r791 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r791'
    }
    resource r792 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r792'
    }
    resource r793 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r793'
    }
    resource r794 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r794'
    }
    resource r795 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r795'
    }
    resource r796 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r796'
    }
    resource r797 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r797'
    }
    resource r798 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r798'
    }
    resource r799 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r799'
    }
    resource r800 'Microsoft.Network/virtualNetworks@2021-05-01' = {
    name: 'r800'

        resource s1 'subnets@2021-05-01' = {
            name: 's1'
        }
    }   
    
            ",
            "r1")]

        [DataTestMethod]
        public void TestRule(string text, params string[] unusedParams)
        {
            CompileAndTest(text, unusedParams);
        }
    }
}
