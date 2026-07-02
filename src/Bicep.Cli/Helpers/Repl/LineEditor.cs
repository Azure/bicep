// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;

namespace Bicep.Cli.Helpers.Repl;

/// <summary>
/// Mutable single-line edit buffer: owns the runes on the current line and the cursor position, and
/// exposes the small set of edit/navigation operations that the REPL key handler dispatches to. This is
/// the extension point - add a new operation here (e.g. word jump, delete-to-end) and wire it to a key
/// in the console command's key handler.
/// </summary>
public sealed class LineEditor
{
    private readonly record struct Snapshot(ImmutableArray<Rune> Runes, int Cursor);
    private readonly List<Rune> buffer = [];
    private readonly List<Snapshot> history = [new([], 0)];
    private int historyIndex;

    public IReadOnlyList<Rune> Buffer => buffer;

    public int Cursor { get; private set; }

    public void MoveLeft() => Cursor = Math.Max(Cursor - 1, 0);

    public void MoveRight() => Cursor = Math.Min(Cursor + 1, buffer.Count);

    public void MoveToStart() => Cursor = 0;

    public void MoveToEnd() => Cursor = buffer.Count;

    public void MoveToWordBoundary(int direction)
    {
        direction = Math.Sign(direction);

        if (direction > 0)
        {
            for (var i = Cursor; i < buffer.Count - 1; i++)
            {
                if (IsWordBoundary(i, i + 1))
                {
                    Cursor = i + 1;
                    return;
                }
            }
            Cursor = buffer.Count;
        }
        else
        {
            for (var i = Math.Min(Cursor, buffer.Count) - 1; i > 0; i--)
            {
                if (IsWordBoundary(i - 1, i))
                {
                    Cursor = i;
                    return;
                }
            }
            Cursor = 0;
        }

        bool IsWordBoundary(int index1, int index2)
        {
            if (index2 >= buffer.Count)
            {
                return false;
            }

            var r1 = buffer[index1];
            var r2 = buffer[index2];

            var isWhitespace1 = Rune.IsWhiteSpace(r1);
            var isWhitespace2 = Rune.IsWhiteSpace(r2);
            if (isWhitespace1 && !isWhitespace2)
            {
                return true;
            }
            if (isWhitespace1 || isWhitespace2)
            {
                return false;
            }

            return Rune.IsLetterOrDigit(r1) != Rune.IsLetterOrDigit(r2);
        }
    }

    public void Insert(Rune rune)
    {
        buffer.Insert(Cursor, rune);
        Cursor++;
    }

    public void Backspace()
    {
        if (Cursor > 0 && Cursor <= buffer.Count)
        {
            buffer.RemoveAt(Cursor - 1);
            Cursor = Math.Max(Cursor - 1, 0);
        }
    }

    public void Delete()
    {
        if (Cursor < buffer.Count)
        {
            buffer.RemoveAt(Cursor);
        }
    }

    public void Reset(IEnumerable<Rune> runes)
    {
        buffer.Clear();
        buffer.AddRange(runes);
        Cursor = buffer.Count;
        // we commit a snapshot here so that the user is able to undo back to the original state of the line
        // even if they haven't typed anything yet.
        Track();
    }

    /// <summary>
    /// Commits the current buffer/cursor to the undo history. If the text is unchanged since the last
    /// commit, such as when only the caret moves, only the cursor position is updated in place; otherwise a new snapshot is appended and any redo
    /// tail is discarded. Call after every edit/navigation.
    /// </summary>
    public void Track()
    {
        var current = history[historyIndex];
        if (MatchesCurrentBuffer(current.Runes))
        {
            if (Cursor != current.Cursor)
            {
                history[historyIndex] = current with { Cursor = Cursor };
            }
            return;
        }

        // are there snapshots ahead of us? (i.e. we undid, then typed)
        // if so, remove them, since they are no longer reachable from the current state.
        if (historyIndex < history.Count - 1)
        {
            history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
        }

        history.Add(new Snapshot([.. buffer], Cursor));
        historyIndex = history.Count - 1;
    }

    public void Undo()
    {
        if (historyIndex > 0)
        {
            Restore(history[--historyIndex]);
        }
    }

    public void Redo()
    {
        if (historyIndex < history.Count - 1)
        {
            Restore(history[++historyIndex]);
        }
    }

    private void Restore(Snapshot snapshot)
    {
        buffer.Clear();
        buffer.AddRange(snapshot.Runes);
        Cursor = snapshot.Cursor;
    }

    private bool MatchesCurrentBuffer(ImmutableArray<Rune> runes)
    {
        if (buffer.Count != runes.Length)
        {
            return false;
        }

        for (var i = 0; i < buffer.Count; i++)
        {
            if (buffer[i] != runes[i])
            {
                return false;
            }
        }

        return true;
    }
}
