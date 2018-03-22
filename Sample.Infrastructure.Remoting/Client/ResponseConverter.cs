using System;
using System.Reflection;
using System.Threading.Tasks;
using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Client
{
    internal class ResponseConverter
    {
        private readonly MethodInfo _fromResultMethod;
        private readonly MethodInfo _fromExceptionMethod;

        public ResponseConverter()
        {
            _fromResultMethod = GetType().GetMethod(nameof(FromResult), BindingFlags.Instance | BindingFlags.NonPublic);
            _fromExceptionMethod = GetType().GetMethod(nameof(FromException), BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public dynamic Convert(RemoteResponse response, Type targetType)
        {
            if (response.IsFaulted)
            {
                return _fromExceptionMethod.MakeGenericMethod(targetType).Invoke(this, new[] { response.Exception });
            }
            else
            {
                return _fromResultMethod.MakeGenericMethod(targetType).Invoke(this, new[] { response.Response });
            }
        }

        private Task<T> FromResult<T>(object o)
        {
            return Task.FromResult((T) o);
        }

        private Task<T> FromException<T>(Exception ex)
        {
            return Task.FromException<T>(ex);
        }
    }
}