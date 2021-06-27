using Xunit;

namespace SchemeCs.Tests {
    public class TokenTest {
        [Fact]
        public void Test() {
            Assert.True(new OpenParenToken().Equals(new OpenParenToken()));
            Assert.True(new CloseParenToken().Equals(new CloseParenToken()));
            Assert.False(new OpenParenToken().Equals(new CloseParenToken()));

            Assert.True(new StringLiteralToken("a").Equals(new StringLiteralToken("a")));
            Assert.False(new StringLiteralToken("a").Equals(new StringLiteralToken("b")));

            Assert.True(new NumberLiteralToken("1").Equals(new NumberLiteralToken("1")));
            Assert.False(new NumberLiteralToken("1").Equals(new NumberLiteralToken("2")));

            Assert.True(new IdentifierToken("a").Equals(new IdentifierToken("a")));
            Assert.False(new IdentifierToken("a").Equals(new IdentifierToken("b")));

            Assert.False(new NumberLiteralToken("1").Equals(new StringLiteralToken("1")));
            Assert.False(new StringLiteralToken("1").Equals(new IdentifierToken("1")));
            Assert.False(new IdentifierToken("1").Equals(new NumberLiteralToken("1")));
        }
    }
}
