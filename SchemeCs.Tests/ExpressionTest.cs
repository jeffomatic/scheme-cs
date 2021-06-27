using System.Collections.Generic;

using Xunit;

namespace SchemeCs.Tests {
    public class ExpressionTest {
        [Fact]
        public void Test() {
            Assert.True(new Reference("a").Equals(new Reference("a")));
            Assert.False(new Reference("a").Equals(new Reference("b")));

            Assert.True(new StringLiteral("a").Equals(new StringLiteral("a")));
            Assert.False(new StringLiteral("a").Equals(new StringLiteral("b")));

            Assert.True(new NumberLiteral(1).Equals(new NumberLiteral(1)));
            Assert.False(new NumberLiteral(1).Equals(new NumberLiteral(2)));

            Assert.Equal(new ListExpr(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }), new ListExpr(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }));
            Assert.NotEqual(new ListExpr(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }), new ListExpr(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("b")
            }));

            Assert.Equal(new Sequence(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }), new Sequence(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }));
            Assert.NotEqual(new Sequence(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("a")
            }), new Sequence(new List<Expression> {
                new NumberLiteral(1),
                new StringLiteral("b")
            }));
        }
    }
}
