using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Xunit;

namespace NCalc.Tests
{

    public interface INestContext
    {

    }
    public class ExtendTest
    {

        [Fact]
        public void Function_Nest_Test()
        {
            var expTest = @"Round(Add(GetOne(),AddOne([One]),[Three])+1.0365,2)";
            var exp = new Expression(expTest);
            //exp.EvaluateFunction += Exp_EvaluateFunction;
            //exp.EvaluateParameter += Exp_EvaluateParameter;
            //var result = exp.Evaluate();
            //Assert.Equal(7.04d, result);

            var func2 = Convert<NestContext>(exp);

            var result2 = func2(new NestContext());
            Assert.Equal(7.04d, result2);

        }

        [Fact]
        public void Double_Args_Test()
        {
            //var expText = "1.03";
            //var exp = new Expression(expText);

            //var result = exp.Evaluate();
            //Assert.Equal(1.03d, result);

            //var func2 = Convert<NestContext>(exp);

            //var result2 = func2(new NestContext());
            //Assert.Equal(1.03d, result2);


            var expTest2 = "Max(1,2)";
            var exp2 = new Expression(expTest2);
            var maxResult = exp2.Evaluate();
            Assert.Equal(2, maxResult);

           
            var maxFunc = exp2.ToLambda<int>();
            Assert.Equal(2, maxFunc());
        }

        [Fact]
        public void String_Add_Test()
        {
            var expText = "[Name]+[FirstName]+[Test]";
            var exp = new Expression(expText);
            exp.EvaluateParameter += (name, e) =>
            {
                switch (name)
                {
                    case "Name":
                        e.Result = "Eric";
                       break;
                    case "FirstName":
                        e.Result = "Zhou";
                        break;
                    case "Test":
                        e.Result = null;
                        break;
                }
            };
            var text = exp.Evaluate();
            Assert.Equal("EricZhou", text);

            var textFunc = exp.ToLambda<NestContext,string>();

            Assert.Equal("EricZhou", textFunc(new NestContext()));

          


        }

        [Fact]
        public void String_Contains_Test()
        {
            var expText = "Contains([Name],'ri')";
            var containsExp = new Expression(expText);
            var containsFunc = containsExp.ToLambda<NestContext, bool>();

            var context = new NestContext();
            var result = containsFunc(context);
            Assert.True(result);
        }


        private static Func<INestContext, double> Convert<TContext>(Expression exp) where  TContext: class
        {
            var func = exp.ToLambda<TContext, double>();

       

            return context => func((TContext)context);
        }

        

        private class NestContext:INestContext
        {
            public int GetOne()
            {
                return 1;
            }

            public int AddOne(int p)
            {
                return p + 1;
            }

            public int Add(int x, int y, int z)
            {
                return x + y + z;
            }

            public string Name { get; set; } = "Eric";

            public string FirstName { get; set; } = "Zhou";
            public string Test { get; set; } = null;

            public int One { get; set; } = 1;

            public int Three { get; set; } = 3;

            public bool Contains(string src, string dst)
            {
                return src != null && src.Contains(dst);
            }
        }

        private void Exp_EvaluateParameter(string name, ParameterArgs args)
        {
            switch (name)
            {
                case "One":
                    args.Result = 1;
                    break;
                case "Three":
                    args.Result = 3;
                    break;
            }
        }

        private void Exp_EvaluateFunction(string name, FunctionArgs args)
        {
            switch (name)
            {
                case "GetOne":
                    args.Result = 1;
                    break;
                case "AddOne":
                    args.Result = 1+ (int)args.Parameters[0].Evaluate();
                    break;
                case "Add":
                    args.Result = (int) args.Parameters[0].Evaluate() + (int) args.Parameters[1].Evaluate() +
                                  (int) args.Parameters[2].Evaluate();
                    break;
            }
        }
    }
}