using System.Collections.Generic;

using Xunit;

namespace SchemeCs.Tests {
    public class ValueTest {
        [Fact]
        public void NilValueTest() {
            Assert.Equal(new NilValue(), new NilValue());
            Assert.NotEqual(new NilValue(), new NilValue());
        }

        [Fact]
        public void NumberValueTest() {
            Assert.Equal(new NumberValue(1), new NumberValue(1));
            Assert.NotEqual(new NumberValue(1), new NumberValue(2));
        }

        [Fact]
        public void EnvironmentTest() {
            var envA = new Environment();
            var envB = new Environment();
            Assert.Equal(envA, envB);

            envA.Set("foo", new NumberValue(1));
            Assert.NotEqual(envA, envB);

            envB.Set("foo", new NumberValue(2));
            Assert.NotEqual(envA, envB);

            envB.Set("foo", new NumberValue(1));
            Assert.Equal(envA, envB);

            var envC = new Environment(envA);
            var envD = new Environment(envA);
            Assert.Equal(envC, envD);

            envC.Set("foo", new NumberValue(1));
            Assert.NotEqual(envC, envD);

            envD.Set("foo", new NumberValue(2));
            Assert.NotEqual(envC, envD);

            envD.Set("foo", new NumberValue(1));
            Assert.Equal(envC, envD);

            // Mutate parent env
            envA.Set("bar", new NumberValue(3));
            Assert.Equal(envC, envD);
        }
    }
}
