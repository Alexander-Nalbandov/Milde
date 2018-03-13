using System;
using System.Linq;
using System.Linq.Expressions;
using Sample.Infrastructure.Remoting.Rabbit;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Handlers;
using static System.Linq.Expressions.Expression;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello World!");

            object[] args = {"asdasd"};
            var instance = new UserManagementService();
            var methodInfo = typeof(IUserManagementService).GetMethod("CreateUser");
            var instanseExpr = Constant(instance);

            ParameterExpression[] parameters = methodInfo.GetParameters().Select(e => Parameter(e.ParameterType)).ToArray();
            var methodCallExpression = Call(instanseExpr, methodInfo, parameters);
            var funcExpr = Lambda<Func<object[], dynamic>>(methodCallExpression, parameters);
            var func = funcExpr.Compile();
            var result = func(args);

            //new SampleClientProgram().Run();
        }
    }
}