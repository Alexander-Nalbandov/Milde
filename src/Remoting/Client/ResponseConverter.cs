using System;
using System.Reflection;
using System.Threading.Tasks;
using Milde.Remoting.Communication;

namespace Milde.Remoting.Client
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
            try
            {
                if (response.IsFaulted)
                {
                    return _fromExceptionMethod.MakeGenericMethod(targetType).Invoke(this, new[] {response.Exception});
                }
                else
                {
                    return _fromResultMethod.MakeGenericMethod(targetType).Invoke(this, new[] {response.Response});
                }

            }
            catch (TargetInvocationException ex)
            {
                throw ex.GetBaseException();
            }
        }

        private Task<T> FromResult<T>(object o)
        {
            return Task.FromResult((T) o);
        }

        /// <summary>
        /// Source exception is wrapped into TargetInvocationExcpetion in order to preserve remote stask trace
        /// Otherwise Source exception is unwrapped on await statement and the stack trace will be replaced  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        private Task<T> FromException<T>(Exception ex)
        {
            return Task.FromException<T>(new TargetInvocationException(ex));
        }
    }
}