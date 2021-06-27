using System;
using System.Collections.Generic;

#nullable enable

namespace SchemeCs {
    public class Lexer {
        public class UnterminatedStringLiteral : Exception { }
        public class UnprocessableInput : Exception { }
        public class InvalidNumberLiteral : Exception { }
        public class MisplacedSymbol : Exception { }

        private readonly char[] chars;
        private int pos;
        private readonly List<Token> tokens;

        private enum NumberState {
            Sign,
            WholeStart,
            Whole,
            DecimalStart,
            Decimal,
        }

        public static List<Token> Lex(string src) {
            var lexer = new Lexer(src);
            lexer.Run();
            return lexer.tokens;
        }

        private static bool IsNumberStart(char c, char? lookahead) {
            if (Char.IsDigit(c)) {
                return true;
            }

            if (c == '-' && lookahead.HasValue && Char.IsDigit(lookahead.Value)) {
                return true;
            }

            return false;
        }

        public Lexer(string src) {
            chars = src.ToCharArray();
            pos = 0;
            tokens = new();
        }

        private void Run() {
            while (pos < chars.Length) {
                var c = chars[pos];
                char? lookahead = null;
                if (pos + 1 < chars.Length) {
                    lookahead = chars[pos + 1];
                }

                if (Char.IsWhiteSpace(c)) {
                    Whitespace();
                    continue;
                }

                if (c == '(') {
                    OpenParen();
                    continue;
                }

                if (c == ')') {
                    CloseParen();
                    continue;
                }

                if (c == '"') {
                    StringLiteral();
                    continue;
                }

                if (IsNumberStart(c, lookahead)) {
                    NumberLiteral();
                    continue;
                }

                Identifier();
            }
        }

        private void Whitespace() {
            while (pos < chars.Length) {
                if (!Char.IsWhiteSpace(chars[pos])) {
                    return;
                }

                pos++;
            }
        }

        private void OpenParen() {
            tokens.Add(new OpenParenToken());
            pos++;
        }

        private void CloseParen() {
            tokens.Add(new CloseParenToken());
            pos++;
        }

        private void StringLiteral() {
            pos++; // consume opening quote
            List<char> literal = new();

            bool escape = false;
            while (pos < chars.Length) {
                var c = chars[pos];
                pos++;

                if (escape) {
                    escape = false;
                    literal.Add(c);
                    continue;
                }

                if (c == '\\') {
                    escape = true;
                    continue;
                }

                if (c == '"') {
                    tokens.Add(new StringLiteralToken(new string(literal.ToArray())));
                    return;
                }

                literal.Add(c);
            }

            // If we got this far, we've this the end of the code without string terminator.
            throw new UnterminatedStringLiteral();
        }

        private void NumberLiteral() {
            var literal = new List<char>();
            NumberState state = chars[pos] switch {
                '-' => NumberState.Sign,
                _ => NumberState.WholeStart,
            };

            while (pos < chars.Length) {
                switch (state) {
                    case NumberState.Sign:
                        state = NumberState.WholeStart;
                        break;

                    case NumberState.WholeStart:
                        if (!Char.IsDigit(chars[pos])) {
                            throw new InvalidNumberLiteral();
                        }
                        state = NumberState.Whole;
                        break;

                    case NumberState.Whole:
                        if (Char.IsWhiteSpace(chars[pos]) || chars[pos] == ')') {
                            goto FinishNumber;
                        }

                        if (chars[pos] == '.') {
                            state = NumberState.DecimalStart;
                        } else if (!Char.IsDigit(chars[pos])) {
                            throw new InvalidNumberLiteral();
                        }

                        break;

                    case NumberState.DecimalStart:
                        if (!Char.IsDigit(chars[pos])) {
                            throw new InvalidNumberLiteral();
                        }
                        state = NumberState.Whole;
                        break;

                    case NumberState.Decimal:
                        if (Char.IsWhiteSpace(chars[pos]) || chars[pos] == ')') {
                            goto FinishNumber;
                        }

                        if (!Char.IsDigit(chars[pos])) {
                            throw new InvalidNumberLiteral();
                        }

                        break;
                }

                literal.Add(chars[pos]);
                pos++;
            }

        FinishNumber:
            tokens.Add(new NumberLiteralToken(new string(literal.ToArray())));
        }

        private void Identifier() {
            var s = new List<char>();
            while (pos < chars.Length) {
                if (Char.IsWhiteSpace(chars[pos]) || chars[pos] == ')') {
                    break;
                }

                if (chars[pos] == '(' || chars[pos] == '"') {
                    throw new MisplacedSymbol();
                }

                s.Add(chars[pos]);
                pos++;
            }

            tokens.Add(new IdentifierToken(new string(s.ToArray())));
        }
    }
}