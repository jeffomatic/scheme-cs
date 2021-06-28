using System.Collections.Generic;

#nullable enable

// don't require HashCode override on any of these
#pragma warning disable 0659

namespace SchemeCs {
    public abstract class Value { }

    public class NilValue : Value {
        public override bool Equals(object? obj) {
            return obj switch {
                NilValue _ => true,
                _ => false,
            };
        }
    }

    public class BooleanValue : Value {
        public static BooleanValue? Downcast(Value v) {
            return v switch {
                BooleanValue bv => bv,
                _ => null,
            };
        }

        public bool Value { get; }

        public BooleanValue(bool v) {
            Value = v;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                BooleanValue other => other.Value == Value,
                _ => false,
            };
        }
    }

    public class NumberValue : Value {
        public static NumberValue? Downcast(Value v) {
            return v switch {
                NumberValue nv => nv,
                _ => null,
            };
        }

        public double Value { get; }

        public NumberValue(double v) {
            Value = v;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                NumberValue nv => nv.Value == Value,
                _ => false,
            };
        }
    }

    public class StringValue : Value {
        public string Value { get; }

        public StringValue(string v) {
            Value = v;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                StringValue nv => nv.Value == Value,
                _ => false,
            };
        }
    }

    public class Environment {
        private readonly Dictionary<string, Value> values;
        private readonly Environment? parent;

        public Environment(Environment? parent = null) {
            values = new Dictionary<string, Value>();
            this.parent = parent;
        }

        public Value? Get(string k) {
            if (values.ContainsKey(k)) {
                return values[k];
            }

            if (parent != null) {
                return parent.Get(k);
            }

            return null;
        }

        public void Set(string k, Value v) {
            values[k] = v;
        }

        // Define globally-available value.
        public void Define(string k, Value v) {
            if (parent != null) {
                parent.Define(k, v);
                return;
            }

            Set(k, v);
        }

        public override bool Equals(object? obj) {
            return obj switch {
                Environment e =>
                    Utils.DictionaryEquals(e.values, values) &&
                    (
                        (e.parent == null && parent == null) ||
                        (e.parent != null && parent != null && e.parent.Equals(parent))
                    ),
                _ => false,
            };
        }
    }

    public class ClosureValue : Value {
        public static ClosureValue? Downcast(Value v) {
            return v switch {
                ClosureValue cv => cv,
                _ => null,
            };
        }

        public List<string> Params { get; }
        public Expression Body { get; }
        public Environment Env { get; }

        public ClosureValue(List<string> parameters, Expression body, Environment env) {
            Params = parameters;
            Body = body;
            Env = env;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                ClosureValue cv =>
                    Utils.ListEquals(cv.Params, Params) &&
                    cv.Body.Equals(Body) &&
                    cv.Env.Equals(Env),
                _ => false,
            };
        }
    }

    public class PrimitiveValue : Value {
        public static PrimitiveValue? Downcast(Value v) {
            return v switch {
                PrimitiveValue pv => pv,
                _ => null,
            };
        }

        public delegate Value EvalDel(List<Value> args);

        public EvalDel Eval { get; }

        public PrimitiveValue(EvalDel eval) {
            Eval = eval;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                PrimitiveValue pv => pv.Eval == Eval,
                _ => false,
            };
        }
    }
}