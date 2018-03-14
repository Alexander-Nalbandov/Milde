using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Remoting.Client
{
    public class ResponseConverter
    {
        private readonly MethodInfo _castMethod;


        public ResponseConverter()
        {
            this._castMethod = this.GetType().GetMethod("Cast", BindingFlags.Instance | BindingFlags.NonPublic);
        }


        public object Convert(object obj, Type targetType)
        {
            var castMethod = this._castMethod.MakeGenericMethod(targetType);
            var castedResponse = castMethod.Invoke(this, new[]
            {
                obj
            });

            return castedResponse;
        }

        private Task<T> Cast<T>(object o)
        {
            return Task.FromResult((T)o);
        }
    }
}
