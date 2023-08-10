// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Configuration
{
    /// <summary>
    /// Allows for some limited JSON text manipulation without destroying commands or existing whitespace
    /// </summary>
    public class JsonEditor
    {
        private string json;
        private int indent;
        private ImmutableArray<int> lineStarts;

        public JsonEditor(string json, int indent = 2)
        {
            this.json = json;
            this.indent = indent;
            lineStarts = TextCoordinateConverter.GetLineStarts(json);
        }

        // If the insertion path already exists, or can't be added (eg array instead of object exists on the path), returns null
        public (int line, int column, string insertText)? InsertIfNotExist(string[] propertyPaths, object valueIfNotExist)
        {
            if (propertyPaths.Length == 0)
            {
                throw new ArgumentException($"{nameof(propertyPaths)} must not be empty");
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return AppendToEndOfJson(Stringify(PropertyPathToObject(propertyPaths, valueIfNotExist)));
            }

            TextReader textReader = new StringReader(json);
            JsonReader jsonReader = new JsonTextReader(textReader);

            JObject? jObject = null;
            try
            {
                jObject = JObject.Load(jsonReader, new JsonLoadSettings
                {
                    LineInfoHandling = LineInfoHandling.Load,
                    CommentHandling = CommentHandling.Load,
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Ignore,
                });
            }
            catch (Exception)
            {
            }

            if (jObject is null)
            {
                // Just append to existing text
                return AppendToEndOfJson(Stringify(PropertyPathToObject(propertyPaths, valueIfNotExist)));
            }

            JObject? currentObject = jObject;
            List<string> remainingPaths = new(propertyPaths);
            while (remainingPaths.Count > 0)
            {
                string path = PopFromLeft(remainingPaths);
                JToken? nextLevel = currentObject[path];
                if (nextLevel is null)
                {
                    int line = ((IJsonLineInfo)currentObject).LineNumber - 1; // 1-indexed to 0-indexed
                    int column = ((IJsonLineInfo)currentObject).LinePosition - 1;
                    object insertionValue = valueIfNotExist;
                    remainingPaths.Reverse();
                    foreach (string propertyName in remainingPaths)
                    {
                        Dictionary<string, object> newObject = new();
                        newObject[propertyName] = insertionValue;
                        insertionValue = newObject;
                    }
                    remainingPaths.Reverse();
                    string newPath = string.Join('.', remainingPaths);
                    string insertionValueAsString = Stringify(insertionValue);

                    int insertLine;
                    int insertColumn;
                    int currentIndent;
                    bool hasSiblings = currentObject.Children().Any(child => child.Type != JTokenType.Comment);
                    insertLine = line;
                    insertColumn = column + 1;
                    currentIndent = GetIndentationOfLine(line) + indent; // use indent of line with the starting "{" as the nested indent level

                    // We will insert before the first sibling
                    string propertyInsertion =
                        "\n" +
                        IndentEachLine(
                            $"\"{path}\": {insertionValueAsString}",
                            currentIndent);
                    if (hasSiblings)
                    {
                        propertyInsertion += ",";
                    }

                    // Need a newline after the insertion if there's anything else on the line
                    var lineStarts = TextCoordinateConverter.GetLineStarts(json);
                    int offset = TextCoordinateConverter.GetOffset(lineStarts, line, column);
                    char? charAfterInsertion = json.Length > offset ? json[offset + 1] : null;
                    if (charAfterInsertion != '\n' && charAfterInsertion != '\r')
                    {
                        propertyInsertion += '\n';
                    }

                    return (insertLine, insertColumn, propertyInsertion);
                }

                if (remainingPaths.Count == 0)
                {
                    // We found matches all the way to the leaf, doesn't matter what the leaf value is, we will leave it alone
                    return null;
                }

                if (nextLevel is JObject nextObject)
                {
                    currentObject = nextObject;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static string ApplyInsertion(string text, (int line, int column, string text) insertion)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);
            int offset = TextCoordinateConverter.GetOffset(lineStarts, insertion.line, insertion.column);
            return text.Substring(0, offset) + insertion.text + text.Substring(offset);
        }

        private (int line, int column, string insertText)? AppendToEndOfJson(string text)
        {
            var lastLine = lineStarts.Length - 1;
            var lastLineLength = json.Length - lineStarts[lineStarts.Length - 1];
            Debug.Assert(lastLine >= 0 && lastLine < lineStarts.Length, $"{nameof(lastLine)} out of bounds");
            Debug.Assert(lastLineLength >= 0, $"{nameof(lastLineLength)} out of bounds");

            if (lastLineLength > 0)
            {
                text = "\n" + text;
            }

            return (lastLine, lastLineLength, text);
        }

        private string PopFromLeft(List<string> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException($"{nameof(list)} should not be empty");
            }

            string value = list[0];
            list.RemoveAt(0);
            return value;
        }

        private object PropertyPathToObject(string[] propertyPaths, object propertyValue)
        {
            List<string> remainingPaths = new(propertyPaths);
            remainingPaths.Reverse();
            Dictionary<string, object> result = new();
            string leafName = PopFromLeft(remainingPaths);
            result[leafName] = propertyValue;

            foreach (string propertyName in remainingPaths)
            {
                Dictionary<string, object> embeddedObject = new();
                embeddedObject[propertyName] = result;
                result = embeddedObject;
            }

            return result;
        }

        private string Stringify(object value, int? indent = null)
        {
            indent ??= this.indent;

            StringBuilder sb = new StringBuilder();
            TextWriter textWriter = new StringWriter(sb);
            JsonTextWriter jsonWriter = new(textWriter);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.IndentChar = ' ';
            jsonWriter.Indentation = indent.Value;

            new JsonSerializer().Serialize(jsonWriter, value);

            return sb.ToString();
        }

        private string IndentEachLine(string text, int indent)
        {
            string indentString = new string(' ', indent);
            return indentString + text.Replace("\n", "\n" + indentString);
        }

        private int GetIndentationOfLine(int line)
        {
            var startOfLine = lineStarts[line];
            for (int i = startOfLine; i < json.Length; ++i)
            {
                char ch = json[i];
                if (ch == '\n' || ch == '\r' || !char.IsWhiteSpace(ch))
                {
                    return i - startOfLine;
                }
            }

            // Last line, either empty or just whitespace
            return json.Length - startOfLine;
        }
    }
}

