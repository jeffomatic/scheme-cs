using System;
using System.Collections.Generic;

namespace SchemeCs {
    public static class Stdlib {
        // TODO: move this to a more appropriate location
        public sealed class InvalidArgumentValue : Exception { }
        public sealed class InvalidArgumentCount : Exception { }

        public static Environment Create() {
            var env = new Environment();

            env.Set("#t", new BooleanValue(true));
            env.Set("#f", new BooleanValue(false));

            env.Set("+", new PrimitiveValue((List<Value> args) => {
                double res = 0;
                foreach (var arg in args) {
                    var n = NumberValue.Downcast(arg);
                    if (n == null) {
                        throw new InvalidArgumentValue();
                    }
                    res += n.Value;
                }
                return new NumberValue(res);
            }));

            env.Set("-", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                var a = NumberValue.Downcast(args[0]);
                var b = NumberValue.Downcast(args[1]);

                if (a == null || b == null) {
                    throw new InvalidArgumentValue();
                }

                return new NumberValue(a.Value - b.Value);
            }));

            env.Set("*", new PrimitiveValue((List<Value> args) => {
                double res = 1;
                foreach (var arg in args) {
                    var n = NumberValue.Downcast(arg);
                    if (n == null) {
                        throw new InvalidArgumentValue();
                    }
                    res *= n.Value;
                }
                return new NumberValue(res);
            }));

            env.Set("/", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                var a = NumberValue.Downcast(args[0]);
                var b = NumberValue.Downcast(args[1]);

                if (a == null || b == null) {
                    throw new InvalidArgumentValue();
                }

                return new NumberValue(a.Value / b.Value);
            }));

            env.Set("=", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                // This doesn't quite work with closures.
                return new BooleanValue(args[0].Equals(args[1]));
            }));

            env.Set(">", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                var a = NumberValue.Downcast(args[0]);
                var b = NumberValue.Downcast(args[1]);

                if (a == null || b == null) {
                    throw new InvalidArgumentValue();
                }

                // This doesn't quite work with closures.
                return new BooleanValue(a.Value > b.Value);
            }));

            env.Set("not", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 1) {
                    throw new InvalidArgumentCount();
                }

                var arg = BooleanValue.Downcast(args[0]);
                if (arg == null) {
                    throw new InvalidArgumentValue();
                }

                return new BooleanValue(!arg.Value);
            }));

            env.Set("and", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                var a = BooleanValue.Downcast(args[0]);
                var b = BooleanValue.Downcast(args[1]);

                if (a == null || b == null) {
                    throw new InvalidArgumentValue();
                }

                return new BooleanValue(a.Value && b.Value);
            }));

            env.Set("or", new PrimitiveValue((List<Value> args) => {
                if (args.Count != 2) {
                    throw new InvalidArgumentCount();
                }

                var a = BooleanValue.Downcast(args[0]);
                var b = BooleanValue.Downcast(args[1]);

                if (a == null || b == null) {
                    throw new InvalidArgumentValue();
                }

                return new BooleanValue(a.Value || b.Value);
            }));

            const string lib = @"
(define >= (lambda (a b) (or (> a b) (= a b))))
(define < (lambda (a b) (not (>= a b))))
(define <= (lambda (a b) (not (> a b))))
            ";

            Evaluator.Eval(env, Parser.Parse(Lexer.Lex(lib)));

            return env;
        }
    }
}