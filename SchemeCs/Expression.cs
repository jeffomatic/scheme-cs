using System.Collections.Generic;

#nullable enable

// don't require HashCode override on any of these
#pragma warning disable 0659

namespace SchemeCs {
    public abstract class Expression { }

    public abstract class ExprContainer : Expression {
        public abstract List<Expression> Children { get; }

        public abstract void Add(Expression e);
    }

    public class Reference : Expression {
        public string Identifier { get; }

        public Reference(string identifier) {
            Identifier = identifier;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                Reference r => r.Identifier == Identifier,
                _ => false
            };
        }
    }

    public class StringLiteral : Expression {
        public string Value { get; }

        public StringLiteral(string value) {
            Value = value;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                StringLiteral other => other.Value == Value,
                _ => false
            };
        }
    }

    public class NumberLiteral : Expression {
        public double Value { get; }

        public NumberLiteral(double value) {
            Value = value;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                NumberLiteral other => other.Value == Value,
                _ => false
            };
        }
    }

    public class ListExpr : ExprContainer {
        public override List<Expression> Children { get; }

        public ListExpr() {
            Children = new List<Expression>();
        }

        public ListExpr(List<Expression> children) {
            Children = children;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                ListExpr other => Utils.ListEquals(other.Children, Children),
                _ => false
            };
        }

        public override void Add(Expression e) {
            Children.Add(e);
        }
    }

    public class Sequence : ExprContainer {
        public override List<Expression> Children { get; }

        public Sequence() {
            Children = new List<Expression>();
        }

        public Sequence(List<Expression> children) {
            Children = children;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                Sequence other => Utils.ListEquals(other.Children, Children),
                _ => false
            };
        }

        public override void Add(Expression e) {
            Children.Add(e);
        }
    }
}