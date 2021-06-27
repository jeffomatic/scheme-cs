using System;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace SchemeCs.Tests {
    public class ParserTest {
        private struct Example {
            public string src;
            public Sequence want;

            public Example(string src, Sequence want) {
                this.src = src;
                this.want = want;
            }
        }

        private readonly ITestOutputHelper output;

        public ParserTest(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void Test() {
            var examples = new List<Example> {
                new Example(
                    "",
                    new Sequence()
                ),
                new Example(
                    "()",
                    new Sequence(new List<Expression> {
                        new ListExpr(),
                    })
                ),
                new Example(
                    "() ()",
                    new Sequence(new List<Expression> {
                        new ListExpr(),
                        new ListExpr(),
                    })
                ),
                new Example(
                    "a \"123\" 1",
                    new Sequence(new List<Expression> {
                        new Reference("a"),
                        new StringLiteral("123"),
                        new NumberLiteral(1),
                    })
                ),
                new Example(
                    "((a))",
                    new Sequence(new List<Expression> {
                        new ListExpr(new List<Expression> {
                            new ListExpr(new List<Expression> {
                                new Reference("a"),
                            })
                        })
                    })
                ),
                new Example(
                    "(+ 1 (* 3 4))",
                    new Sequence(new List<Expression> {
                        new ListExpr(new List<Expression> {
                            new Reference("+"),
                            new NumberLiteral(1),
                            new ListExpr(new List<Expression> {
                                new Reference("*"),
                                new NumberLiteral(3),
                                new NumberLiteral(4),
                            })
                        })
                    })
                ),
            };

            foreach (var example in examples) {
                output.WriteLine(example.src);
                var toks = Lexer.Lex(example.src);
                Assert.Equal(Parser.Parse(toks), example.want);
            }
        }
    }
}
