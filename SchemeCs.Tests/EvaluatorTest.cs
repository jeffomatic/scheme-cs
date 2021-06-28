using System;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace SchemeCs.Tests {
    public class EvaluatorTest {
        private struct Example {
            public string src;
            public Value want;

            public Example(string src, Value want) {
                this.src = src;
                this.want = want;
            }
        }

        private readonly ITestOutputHelper output;

        public EvaluatorTest(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void Test() {
            var examples = new List<Example> {
                new Example(
                    "(+ 1 1)",
                    new NumberValue(2)
                ),
                new Example(
                    "(+ (+ 1 2) (+ 3 4))",
                    new NumberValue(10)
                ),
                new Example(
                    "#t",
                    new BooleanValue(true)
                ),
                new Example(
                    "#f",
                    new BooleanValue(false)
                ),
                new Example(
                    "(and #t #t)",
                    new BooleanValue(true)
                ),
                new Example(
                    "(and #t #f)",
                    new BooleanValue(false)
                ),
                new Example(
                    "(not #t)",
                    new BooleanValue(false)
                ),
                new Example(
                    "(or #t #t)",
                    new BooleanValue(true)
                ),
                new Example(
                    "(or #f #t)",
                    new BooleanValue(true)
                ),
                new Example(
                    "(or #f #f)",
                    new BooleanValue(false)
                ),
                new Example(
                    "(if (> 1 2) 3 4)",
                    new NumberValue(4)
                ),
                new Example(
                    "(if (< 1 2) 3 4)",
                    new NumberValue(3)
                ),
                new Example(
                    "(if (> 1 2) 4 3)",
                    new NumberValue(3)
                ),
                new Example(
                    "(if (< 1 2) 4 3)",
                    new NumberValue(4)
                ),
                // proc definitions
                new Example("(define f (lambda () 1)) (f)", new NumberValue(1.0)),
                new Example("(define f (lambda (a) a)) (f 2)", new NumberValue(2.0)),
                // local definitions (let)
                new Example("(let ((x 1)) x)", new NumberValue(1.0)),
                new Example("(let ((x 1) (y 2)) (+ x y))", new NumberValue(3.0)),
                new Example("(let ((x (+ 1 3))) x)", new NumberValue(4.0)),
                // fibonacci with define
                new Example(
                    @"
                    (define fib (lambda (n)
                                  (if (= n 0)
                                    0
                                    (if (= n 1)
                                      1
                                      (+ (fib (- n 1))
                                        (fib (- n 2)))))))
                    (fib 10)
                    ",
                    new NumberValue(55.0)
                ),
                // fibonacci with y-combinator
                new Example(
                    @"
                    (let ((y (lambda (f)
                               ((lambda (proc)
                                   (f (lambda (arg) ((proc proc) arg))))
                                (lambda (proc)
                                   (f (lambda (arg) ((proc proc) arg)))))))
                          (fib-pre (lambda (f)
                                      (lambda (n)
                                         (if (= n 0)
                                             0
                                             (if (= n 1)
                                                 1
                                                 (+ (f (- n 1)) (f (- n 2)))))))))
                       (let ((fib (y fib-pre)))
                          (fib 10)))
                    ",
                    new NumberValue(55.0)
                ),
            };

            foreach (var example in examples) {
                output.WriteLine(example.src);
                var toks = Lexer.Lex(example.src);
                var seq = Parser.Parse(toks);
                var got = Evaluator.Eval(Stdlib.Create(), seq);
                Assert.Equal(got, example.want);
            }
        }
    }
}
