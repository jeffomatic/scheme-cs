#nullable enable

// don't require HashCode override on any of these
#pragma warning disable 0659

namespace SchemeCs {
    public abstract class Token {
    }

    public class StringLiteralToken : Token {
        public string Value { get; }

        public StringLiteralToken(string s) {
            Value = s;
        }

        public override bool Equals(object? other) {
            return other switch {
                StringLiteralToken t => t.Value == Value,
                _ => false,
            };
        }
    }

    public class NumberLiteralToken : Token {
        public string Value { get; }

        public NumberLiteralToken(string s) {
            Value = s;
        }

        public override bool Equals(object? other) {
            return other switch {
                NumberLiteralToken t => t.Value == Value,
                _ => false,
            };
        }
    }

    public class OpenParenToken : Token {
        public override bool Equals(object? other) {
            return other switch {
                OpenParenToken _ => true,
                _ => false,
            };
        }
    }

    public class CloseParenToken : Token {
        public override bool Equals(object? other) {
            return other switch {
                CloseParenToken _ => true,
                _ => false,
            };
        }
    }

    public class IdentifierToken : Token {
        public string Value { get; }

        public IdentifierToken(string s) {
            Value = s;
        }

        public override bool Equals(object? other) {
            return other switch {
                IdentifierToken t => t.Value == Value,
                _ => false,
            };
        }
    }
}