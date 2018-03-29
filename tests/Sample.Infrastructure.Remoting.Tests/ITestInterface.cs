using System;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Remoting.Tests
{
    internal interface ITestInterface
    {
        Task<bool> AsyncBooleanMethod(string input);
        Task AsyncVoidMethod();
        void VoidMethod();
        DateTime SyncMethod();
        Task<object> AsyncObjectMethod();
    }
}