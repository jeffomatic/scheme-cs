using System;
using Xunit;
using System.Collections.Generic;

namespace SchemeCs.Tests {
    public class LexerTest {
        [Fact]
        public void Test() {
            Assert.Equal(
               Lexer.Lex(""),
               new List<Token>()
            );

            Assert.Equal(
                Lexer.Lex("\"hi\" \"with \\\"escapes\\\"\""),
                new List<Token> {
                    new StringLiteralToken("hi"),
                    new StringLiteralToken("with \"escapes\""),
                }
            );

            Assert.Equal(
                Lexer.Lex("1 -1 1234 -1234 1234.0 -1234.0"),
                new List<Token> {
                    new NumberLiteralToken("1"),
                    new NumberLiteralToken("-1"),
                    new NumberLiteralToken("1234"),
                    new NumberLiteralToken("-1234"),
                    new NumberLiteralToken("1234.0"),
                    new NumberLiteralToken("-1234.0"),
                }
            );

            Assert.Equal(
                Lexer.Lex("(1) (-1) (1234) (-1234) (1234.0) (-1234.0)"),
                new List<Token> {
                    new OpenParenToken(),
                    new NumberLiteralToken("1"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new NumberLiteralToken("-1"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new NumberLiteralToken("1234"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new NumberLiteralToken("-1234"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new NumberLiteralToken("1234.0"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new NumberLiteralToken("-1234.0"),
                    new CloseParenToken(),
                }
            );

            Assert.Equal(
                Lexer.Lex("a abc abc123 - -a"),
                new List<Token> {
                    new IdentifierToken("a"),
                    new IdentifierToken("abc"),
                    new IdentifierToken("abc123"),
                    new IdentifierToken("-"),
                    new IdentifierToken("-a"),
                }
            );

            Assert.Equal(
                Lexer.Lex("(a) (abc)"),
                new List<Token> {
                    new OpenParenToken(),
                    new IdentifierToken("a"),
                    new CloseParenToken(),
                    new OpenParenToken(),
                    new IdentifierToken("abc"),
                    new CloseParenToken(),
                }
            );
        }
    }
}
