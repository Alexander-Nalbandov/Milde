using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Shouldly;
using Xunit;

namespace Sample.Infrastructure.Remoting.Tests
{
    public class ResponseConverterTest
    {
        public static IEnumerable<object[]> CorrectTypeParameters()
        {
            yield return new object[] {"asd", typeof(string)};
            yield return new object[] {-123, typeof(int)};
            yield return new object[] {132.1m, typeof(decimal)};
            yield return new object[] {DateTime.UtcNow, typeof(DateTime)};
            yield return new object[] {0U, typeof(uint)};
            yield return new object[] {12.5D, typeof(double)};
            yield return new[] {new object(), typeof(object)};
            yield return new object[] {"asd", typeof(object)};
            yield return new object[] {132, typeof(object)};
        }

        public static IEnumerable<object[]> IncorrectTypeParameters()
        {
            yield return new object[] {"asd", typeof(decimal)};
            yield return new object[] {-123, typeof(string)};
            yield return new object[] {132L, typeof(int)};
            yield return new object[] {0U, typeof(long)};
            yield return new object[] {12.5D, typeof(ushort)};
            yield return new[] {new object(), typeof(string)};
        }

        [Theory]
        [MemberData(nameof(CorrectTypeParameters))]
        public void TaskOfExpectedTypeReturned(object responseData, Type targetType)
        {
            var converter = new ResponseConverter();
            var response = new RemoteResponse(responseData);

            object result = converter.Convert(response, targetType);
            result.ShouldBeAssignableTo(typeof(Task<>).MakeGenericType(targetType));
        }

        [Theory]
        [MemberData(nameof(IncorrectTypeParameters))]
        public void ThrowsOnIncorrectType(object responseData, Type targetType)
        {
            var converter = new ResponseConverter();
            var response = new RemoteResponse(responseData);

            Assert.Throws<InvalidCastException>(() => converter.Convert(response, targetType));
        }

        [Fact]
        public async void ExceptionWrapped()
        {
            Exception ex = null;
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                ex = e;
            }
            var converter = new ResponseConverter();
            var response = new RemoteResponse(ex);

            object result = converter.Convert(response, typeof(int));

            var castedResult = result.ShouldBeAssignableTo<Task<int>>();
            castedResult.IsFaulted.ShouldBe(true);
            castedResult.Exception.ShouldNotBeNull();
            var targetInvocationException = castedResult.Exception.InnerExceptions.First().ShouldBeAssignableTo<TargetInvocationException>();
            targetInvocationException.GetBaseException().ShouldBeAssignableTo<NotImplementedException>();
            await Assert.ThrowsAsync<TargetInvocationException>(() => castedResult);
        }

        //TODO remove
        //This is just to demonstrate stack trace behavior on await's exception unwrapping
        //[Fact]
        //public async void ExceptionWrapped2()
        //{
        //    Exception ex = null;
        //    try
        //    {
        //        throw new NotImplementedException();
        //    }
        //    catch (Exception e)
        //    {
        //        ex = e;
        //    }
        //    var converter = new ResponseConverter();
        //    var response = new RemoteResponse(ex);

        //    object result = converter.Convert(response, typeof(int));

        //    var castedResult = result.ShouldBeAssignableTo<Task<int>>();
        //    castedResult.IsFaulted.ShouldBe(true);
        //    castedResult.Exception.ShouldNotBeNull();
        //    await castedResult;
        //}
    }
}