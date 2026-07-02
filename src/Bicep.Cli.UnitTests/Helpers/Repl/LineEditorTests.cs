// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Cli.Helpers.Repl;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests.Helpers.Repl;

[TestClass]
public class LineEditorTests
{
    private static LineEditor CreateEditor(string text = "")
    {
        var editor = new LineEditor();
        Type(editor, text);
        return editor;
    }

    private static void Type(LineEditor editor, string text)
    {
        foreach (var rune in text.EnumerateRunes())
        {
            editor.Insert(rune);
        }
    }

    private static string GetText(LineEditor editor) => string.Concat(editor.Buffer);

    [TestMethod]
    public void Typing_inserts_at_the_end_and_advances_the_cursor()
    {
        var editor = CreateEditor("hello");

        GetText(editor).Should().Be("hello");
        editor.Cursor.Should().Be(5);
    }

    [TestMethod]
    public void MoveToStart_and_MoveToEnd_move_the_cursor_to_the_line_boundaries()
    {
        var editor = CreateEditor("hello");

        editor.MoveToStart();
        editor.Cursor.Should().Be(0);

        editor.MoveToEnd();
        editor.Cursor.Should().Be(5);
    }

    [TestMethod]
    public void MoveLeft_and_MoveRight_clamp_at_the_line_boundaries()
    {
        var editor = CreateEditor("ab");

        editor.MoveToStart();
        editor.MoveLeft();
        editor.Cursor.Should().Be(0);

        editor.MoveToEnd();
        editor.MoveRight();
        editor.Cursor.Should().Be(2);
    }

    [TestMethod]
    public void MoveToWordBoundary_jumps_between_words()
    {
        var editor = CreateEditor("foo bar");
        editor.MoveToStart();

        editor.MoveToWordBoundary(1);
        editor.Cursor.Should().Be(4, "forward should skip whitespace and land at the start of 'bar'");

        editor.MoveToWordBoundary(1);
        editor.Cursor.Should().Be(7, "forward at the last word should land at the end of the line");

        editor.MoveToWordBoundary(-1);
        editor.Cursor.Should().Be(4, "backward should land at the start of 'bar'");

        editor.MoveToWordBoundary(-1);
        editor.Cursor.Should().Be(0, "backward should land at the start of 'foo'");
    }

    [TestMethod]
    public void MoveToWordBoundary_stops_at_transitions_between_letters_and_symbols()
    {
        var editor = CreateEditor("ab+cd");
        editor.MoveToStart();

        editor.MoveToWordBoundary(1);
        editor.Cursor.Should().Be(2, "forward should stop before the '+' symbol");

        editor.MoveToWordBoundary(1);
        editor.Cursor.Should().Be(3, "forward should stop after the '+' symbol");

        editor.MoveToWordBoundary(1);
        editor.Cursor.Should().Be(5, "forward should reach the end of the line");
    }

    [TestMethod]
    public void Insert_inserts_at_the_cursor_position()
    {
        var editor = CreateEditor("ac");
        editor.MoveToStart();
        editor.MoveRight();

        editor.Insert(new Rune('b'));

        GetText(editor).Should().Be("abc");
        editor.Cursor.Should().Be(2);
    }

    [TestMethod]
    public void Backspace_removes_the_character_before_the_cursor()
    {
        var editor = CreateEditor("abc");

        editor.Backspace();

        GetText(editor).Should().Be("ab");
        editor.Cursor.Should().Be(2);
    }

    [TestMethod]
    public void Backspace_at_the_start_of_the_line_is_a_noop()
    {
        var editor = CreateEditor("abc");
        editor.MoveToStart();

        editor.Backspace();

        GetText(editor).Should().Be("abc");
        editor.Cursor.Should().Be(0);
    }

    [TestMethod]
    public void Delete_removes_the_character_at_the_cursor()
    {
        var editor = CreateEditor("abc");
        editor.MoveToStart();

        editor.Delete();

        GetText(editor).Should().Be("bc");
        editor.Cursor.Should().Be(0);
    }

    [TestMethod]
    public void Delete_at_the_end_of_the_line_is_a_noop()
    {
        var editor = CreateEditor("abc");

        editor.Delete();

        GetText(editor).Should().Be("abc");
        editor.Cursor.Should().Be(3);
    }

    [TestMethod]
    public void Insert_and_Backspace_treat_a_supplementary_rune_as_a_single_unit()
    {
        var editor = new LineEditor();

        editor.Insert(new Rune(0x1F4AA)); // 💪
        editor.Buffer.Should().HaveCount(1);
        editor.Cursor.Should().Be(1);

        editor.Backspace();
        editor.Buffer.Should().BeEmpty();
        editor.Cursor.Should().Be(0);
    }

    [TestMethod]
    public void Undo_reverts_the_last_tracked_edit()
    {
        var editor = new LineEditor();
        editor.Insert(new Rune('a'));
        editor.Track();
        editor.Insert(new Rune('b'));
        editor.Track();

        editor.Undo();
        GetText(editor).Should().Be("a");
        editor.Cursor.Should().Be(1);

        editor.Undo();
        GetText(editor).Should().Be("");
        editor.Cursor.Should().Be(0);
    }

    [TestMethod]
    public void Undo_with_no_history_is_a_noop()
    {
        var editor = new LineEditor();

        editor.Undo();

        GetText(editor).Should().Be("");
        editor.Cursor.Should().Be(0);
    }

    [TestMethod]
    public void Redo_reapplies_an_undone_edit()
    {
        var editor = new LineEditor();
        editor.Insert(new Rune('a'));
        editor.Track();
        editor.Insert(new Rune('b'));
        editor.Track();

        editor.Undo();
        editor.Undo();
        GetText(editor).Should().Be("");

        editor.Redo();
        GetText(editor).Should().Be("a");

        editor.Redo();
        GetText(editor).Should().Be("ab");
    }

    [TestMethod]
    public void Editing_after_an_undo_discards_the_redo_history()
    {
        var editor = new LineEditor();
        editor.Insert(new Rune('a'));
        editor.Track();
        editor.Insert(new Rune('b'));
        editor.Track();

        editor.Undo();
        GetText(editor).Should().Be("a");

        editor.Insert(new Rune('c'));
        editor.Track();
        GetText(editor).Should().Be("ac");

        editor.Redo();
        GetText(editor).Should().Be("ac", "there should be nothing to redo after a new edit");

        editor.Undo();
        GetText(editor).Should().Be("a");
    }

    [TestMethod]
    public void Track_after_a_cursor_only_move_does_not_create_a_new_undo_entry()
    {
        var editor = new LineEditor();
        editor.Insert(new Rune('a'));
        editor.Track();
        editor.Insert(new Rune('b'));
        editor.Track();

        editor.MoveToStart();
        editor.Track();

        editor.Undo();
        GetText(editor).Should().Be("a", "a caret-only move should not add an undo step");
    }

    [TestMethod]
    public void Reset_replaces_the_buffer_and_commits_an_undoable_snapshot()
    {
        var editor = new LineEditor();

        editor.Reset("foo".EnumerateRunes());
        GetText(editor).Should().Be("foo");
        editor.Cursor.Should().Be(3);

        editor.Insert(new Rune('x'));
        editor.Track();
        GetText(editor).Should().Be("foox");

        editor.Undo();
        GetText(editor).Should().Be("foo", "the reset line should be an undoable state");
    }
}
