// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using FluentAssertions;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class CommentSelectionTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void VerifyCommentSelection()
        {
            ProjectItemTestExtension projectItem = TestProject![@"LanguageConfiguration\main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);

            var startPosition = new Position(1, 1);
            var endPosition = new Position(1, editor.Contents.Length);

            VerifyCommentSelection(editor, startPosition, endPosition, "//param cosmosDBDatabaseName string = 'db'");

            endPosition = new Position(1, editor.Contents.Length);

            VerifyUncommentSelection(editor, startPosition, endPosition, "param cosmosDBDatabaseName string = 'db'");
        }

        private void VerifyCommentSelection(IVisualStudioTextEditorTestExtension editor, Position startPosition, Position endPosition, string expected)
        {
            editor.Selection.Select(new Range(startPosition, endPosition));
            VsHostUtility.VsHost!.ObjectModel.Commanding.ExecuteCommand(EditorConstants.EditorCommandSet, (uint)EditorConstants.EditorCommandID.ToggleLineComments);

            editor.Contents.Should().Be(expected);
        }

        private void VerifyUncommentSelection(IVisualStudioTextEditorTestExtension editor, Position startPosition, Position endPosition, string expected)
        {
            editor.Selection.Select(new Range(startPosition, endPosition));
            VsHostUtility.VsHost!.ObjectModel.Commanding.ExecuteCommand(EditorConstants.EditorCommandSet, (uint)EditorConstants.EditorCommandID.ToggleLineComments);

            editor.Contents.Should().Be(expected);
        }
    }
}
