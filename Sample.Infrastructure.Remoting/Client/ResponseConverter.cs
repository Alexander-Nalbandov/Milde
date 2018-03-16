using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Remoting.Client
{
    internal class ResponseConverter
    {
        private readonly MethodInfo _castMethod;

        public ResponseConverter()
        {
            _castMethod = GetType().GetMethod("Cast", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public dynamic Convert(object obj, Type targetType)
        {
            var castMethod = _castMethod.MakeGenericMethod(targetType);
            var castedResponse = castMethod.Invoke(this, new[]
            {
                obj
            });

            return castedResponse;
        }

        private Task<T> Cast<T>(object o)
        {
            return Task.FromResult((T) o);
        }
    }
}