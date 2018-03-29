using System;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Remoting.Tests
{
    internal class TestImplementation : ITestInterface
    {
        public Task<bool> AsyncBooleanMethod(string input)
        {
            return Task.FromResult(true);
        }

        public Task AsyncVoidMethod()
        {
            return Task.CompletedTask;
        }

        public void VoidMethod()
        {
            return;
        }

        public DateTime SyncMethod()
        {
            return DateTime.UtcNow;
        }

        public Task<object> AsyncObjectMethod()
        {
            return Task.FromResult(new object());
        }

        public Task<object> AsyncObjectMethodWithParameters(DateTime date, bool flag, object @class, Type type)
        {
            return Task.FromResult(new object());
        }
    }
}