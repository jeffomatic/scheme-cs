using System;
using System.Collections.Generic;

namespace SchemeCs {
    public static class Evaluator {
        public static Value Eval(Environment env, Expression expr) {
            return expr switch {
                NumberLiteral n => new NumberValue(n.Value),
                StringLiteral s => new StringValue(s.Value),
                Symbol r => env.Get(r.Identifier) ?? new NilValue(),
                Sequence seq => EvalSequence(env, seq),
                ListExpr list => EvalList(env, list),
                _ => throw new InvalidOperationException(),
            };
        }

        public static Value EvalSequence(Environment env, Sequence seq) {
            Value res = new NilValue();
            foreach (var expr in seq.Children) {
                res = Eval(env, expr);
            }
            return res;
        }

        public static Value EvalList(Environment env, ListExpr list) {
            if (list.Children.Count == 0) {
                return new NilValue();
            }

            return list.Children[0] switch {
                Symbol s =>
                    s.Identifier switch {
                        "define" => EvalDefine(env, list),
                        "lambda" => EvalLambda(env, list),
                        "if" => EvalIf(env, list),
                        "let" => EvalLet(env, list),
                        _ => EvalApplication(env, list),
                    },
                _ => EvalApplication(env, list),
            };
        }

        public sealed class InvalidDefineExpression : Exception { }

        public static Value EvalDefine(Environment env, ListExpr list) {
            if (list.Children.Count != 3) {
                throw new InvalidDefineExpression();
            }

            var symbol = Symbol.Downcast(list.Children[1]);
            if (symbol == null) {
                throw new InvalidDefineExpression();
            }

            var value = Eval(env, list.Children[2]);
            env.Define(symbol.Identifier, value);

            return new NilValue();
        }

        public sealed class InvalidLambdaExpression : Exception { }

        public static Value EvalLambda(Environment env, ListExpr list) {
            if (list.Children.Count != 3) {
                throw new InvalidLambdaExpression();
            }

            var paramList = ListExpr.Downcast(list.Children[1]);
            if (paramList == null) {
                throw new InvalidLambdaExpression();
            }

            var paramSymbols = new List<string>();
            foreach (var p in paramList.Children) {
                var sym = Symbol.Downcast(p);
                if (sym == null) {
                    throw new InvalidLambdaExpression();
                }
                paramSymbols.Add(sym.Identifier);
            }

            var body = list.Children[2];

            return new ClosureValue(paramSymbols, body, env);
        }

        public sealed class InvalidIfExpression : Exception { }
        public sealed class InvalidPredicateValue : Exception { }

        public static Value EvalIf(Environment env, ListExpr list) {
            if (list.Children.Count != 4) {
                throw new InvalidIfExpression();
            }

            var predicate = BooleanValue.Downcast(Eval(env, list.Children[1]));
            if (predicate == null) {
                throw new InvalidPredicateValue();
            }

            var branch = predicate.Value ? list.Children[2] : list.Children[3];
            return Eval(env, branch);
        }

        public sealed class InvalidLetExpression : Exception { }

        public static Value EvalLet(Environment env, ListExpr list) {
            if (list.Children.Count != 3) {
                throw new InvalidLetExpression();
            }

            var assignments = ListExpr.Downcast(list.Children[1]);
            if (assignments == null) {
                throw new InvalidLetExpression();
            }

            var letEnv = new Environment(env);
            foreach (var assignment in assignments.Children) {
                var asList = ListExpr.Downcast(assignment);
                if (asList == null) {
                    throw new InvalidLetExpression();
                }

                if (asList.Children.Count != 2) {
                    throw new InvalidLetExpression();
                }

                var sym = Symbol.Downcast(asList.Children[0]);
                if (sym == null) {
                    throw new InvalidLetExpression();
                }

                var val = Eval(env, asList.Children[1]);
                letEnv.Set(sym.Identifier, val);
            }

            return Eval(letEnv, list.Children[2]);
        }

        public sealed class InvalidApplication : Exception { }

        public static Value EvalApplication(Environment env, ListExpr list) {
            var primitive = PrimitiveValue.Downcast(Eval(env, list.Children[0]));
            if (primitive != null) {
                var argVals = new List<Value>();
                foreach (var expr in list.Children.GetRange(1, list.Children.Count - 1)) {
                    argVals.Add(Eval(env, expr));
                }
                return primitive.Eval(argVals);
            }

            var closure = ClosureValue.Downcast(Eval(env, list.Children[0]));
            if (closure == null) {
                throw new InvalidApplication();
            }

            if (closure.Params.Count != list.Children.Count - 1) {
                throw new InvalidApplication();
            }

            var frame = new Environment(closure.Env);
            for (var i = 0; i < closure.Params.Count; i++) {
                var sym = closure.Params[i];
                var val = Eval(env, list.Children[i + 1]);
                frame.Set(sym, val);
            }

            return Eval(frame, closure.Body);
        }
    }
}
