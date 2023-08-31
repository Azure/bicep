// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Bicep.LangServer.UnitTests.Configuration
{
    [TestClass]
    public class JsonEditorTests
    {
        [TestMethod]
        public void EmptyPaths_Throws()
        {
            Action action = () => TestInsertion(
                "",
                "",
                new { level = "warning" },
                "");
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void AlreadyExists_TopLevelPath()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers",
                "value",
                null);
        }

        [TestMethod]
        public void AlreadyExists_Middle()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers.core.rules.no-unused-params",
                new { level = "warning" },
                null
            );
        }

        [TestMethod]
        public void AlreadyExists_Leaf()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                null
            );
        }

        [TestMethod]
        public void InvalidJson_Empty_ShouldReproduceDefaultValue()
        {
            TestInsertion(
                "",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                @"{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}"
            );
        }

        [TestMethod]
        public void InvalidJson_JustWhitespaceAndComments_ShouldAppendDefaultValue()
        {
            TestInsertion(
                @"
                    // Well hello there

                // again",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                @"
                    // Well hello there

                // again
{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void EmptyJsonObjectNoSpaces()
        {
            TestInsertion(
                @"{}",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                @"{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void EmptyJsonObjectWithNewline()
        {
            TestInsertion(
                @"{
}",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                @"{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void EmptyJson()
        {
            TestInsertion(
                "",
                "analyzers.core.rules.no-unused-params.level",
                "info",
                @"{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void EmptyJsonWithWhitespace()
        {
            TestInsertion(
                @"

",
                "analyzers.core.rules.no-unused-params.level",
                "warning",
                @"

{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void InsertIntoEmptyObject_Deep()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""another-rule"": {
        }
      }
    }
  }
}",
                "analyzers.core.rules.another-rule.level",
                "warning",
                    @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""another-rule"": {
          ""level"": ""warning""
        }
      }
    }
  }
}"
            );
        }


        [TestMethod]
        public void InsertAsSibling_TopLevel_Simple()
        {
            TestInsertion(
                @"{
  ""one"": {}
}",
                "two",
                "hello",
                    @"{
  ""two"": ""hello"",
  ""one"": {}
}"
            );
        }

        [TestMethod]
        public void InsertAsSibling_TopLevel()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers2.core.rules.no-unused-params.level",
                "off",
                    @"{
  ""analyzers2"": {
    ""core"": {
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}"
            );
        }

        [TestMethod]
        public void InsertAsSibling_Deep()
        {
            TestInsertion(
                @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers.core.rules.no-unused-vars.level",
                "warning",
                    @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-vars"": {
          ""level"": ""warning""
        },
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}"
            );
        }

        [TestMethod]
        public void DoesntAffectComments()
        {
            TestInsertion(
                @"{
// One
  ""analyzers"": {
    ""core"": {// Two
      ""verbose"": false, // Three
      ""enabled"": true,
      ""rules"": {
        // Four
        ""no-unused-params"": { // Five
          // Six
          ""level"": ""info""
        }
      }
    }
  }
}",
                "analyzers.core.rules.no-unused-vars.level",
                "warning",
                @"{
// One
  ""analyzers"": {
    ""core"": {// Two
      ""verbose"": false, // Three
      ""enabled"": true,
      ""rules"": {
        ""no-unused-vars"": {
          ""level"": ""warning""
        },
        // Four
        ""no-unused-params"": { // Five
          // Six
          ""level"": ""info""
        }
      }
    }
  }
}"
            );
        }

        [TestMethod]
        public void FollowsExistingFormatting()
        {
            TestInsertion(
                @"{

  ""analyzers"": {

""core"": {
      ""verbose"": false,
      ""enabled"": true,
                    ""rules"": {
                      ""no-unused-params"": {

                        ""level"":
                        ""info""
                      }
      }
    }
  }
}",
                "analyzers.core.rules.no-unused-vars.level",
                "warning",
                @"{

  ""analyzers"": {

""core"": {
      ""verbose"": false,
      ""enabled"": true,
                    ""rules"": {
                      ""no-unused-vars"": {
                        ""level"": ""warning""
                      },
                      ""no-unused-params"": {

                        ""level"":
                        ""info""
                      }
      }
    }
  }
}"
            );
        }

        [TestMethod]
        public void FollowsExistingFormatting_WithinRulesSection()
        {
            TestInsertion(
                @"{
  ""analyzers"":
    {
    }
}",
                "analyzers.core.rules.no-unused-vars.level",
                "warning",
                @"{
  ""analyzers"":
    {
      ""core"": {
        ""rules"": {
          ""no-unused-vars"": {
            ""level"": ""warning""
          }
        }
      }
    }
}"
            );
        }

        private void TestInsertion(string beforeText, string insertionPath, object insertionValue, string? afterText)
        {
            (int line, int column, string text)? insertion =
                new JsonEditor(beforeText).
                    InsertIfNotExist(
                        insertionPath.Split('.').Where(p => p.Length > 0).ToArray(),
                        insertionValue);
            if (afterText is null)
            {
                insertion.Should().BeNull();
            }
            else
            {
                insertion.Should().NotBeNull();
                var newText = JsonEditor.ApplyInsertion(beforeText, insertion!.Value);

                newText = newText.Replace("\r\n", "\n");
                afterText = afterText.Replace("\r\n", "\n");
                newText.Should().Be(afterText);
            }
        }
    }
}
