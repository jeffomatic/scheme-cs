using System;
using System.Collections.Generic;

namespace SchemeCs {
    public sealed class Parser {
        public class InvalidCloseParen : Exception { }

        private readonly List<Token> tokens;
        private readonly Sequence topLevel;

        public static Sequence Parse(List<Token> tokens) {
            var parser = new Parser(tokens);
            parser.Run();
            return parser.topLevel;
        }

        private Parser(List<Token> tokens) {
            this.tokens = tokens;
            topLevel = new Sequence();
        }

        private void Run() {
            var stack = new List<ExprContainer> { topLevel };

            foreach (var tok in tokens) {
                switch (tok) {
                    case OpenParenToken:
                        var list = new ListExpr();
                        stack[^1].Add(list);
                        stack.Add(list);
                        break;

                    case CloseParenToken:
                        if (stack.Count < 2) {
                            throw new InvalidCloseParen();
                        }
                        stack.RemoveAt(stack.Count - 1);
                        break;

                    case NumberLiteralToken t:
                        stack[^1].Add(new NumberLiteral(Double.Parse(t.Value)));
                        break;

                    case StringLiteralToken t:
                        stack[^1].Add(new StringLiteral(t.Value));
                        break;

                    case IdentifierToken t:
                        stack[^1].Add(new Symbol(t.Value));
                        break;
                }
            }
        }
    }
}