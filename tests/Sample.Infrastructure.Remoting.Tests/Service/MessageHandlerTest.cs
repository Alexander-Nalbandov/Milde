using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Moq;
using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Service;
using Serilog;
using Shouldly;
using Xunit;

namespace Sample.Infrastructure.Remoting.Tests.Service
{
    public class MessageHandlerTest
    {
        private static readonly IEnumerable<object[]> _correctCallParameters = new []
        {
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new [] {"hello"}},
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[] {"hello", "unnecessary extra parameter", false}},
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new [] { "unnecessary extra parameter" } },
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new object[] { false }},
            new object[] { nameof(ITestInterface.AsyncObjectMethodWithParameters), new [] { DateTime.Now, true, new object(), typeof(object) }},
        };

        private static readonly IEnumerable<object[]> _incorrectCallParameters = new []
        {
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new[] { "hello" }},
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[0]},
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new[] { "hello" }},
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new[] { "hello" }},
        };

        private static readonly IEnumerable<object[]> _faultedInvocationParameters = new []
        {
            new object[] { nameof(ITestInterface.SyncMethod), new[] { "hello" }},
            new object[] { nameof(ITestInterface.VoidMethod), new[] { "hello" }},
            new object[] { nameof(ITestInterface.AsyncVoidMethod), new[] { "hello" } },
        };

        [Theory]
        [MemberData(nameof(CorrectCallParameters), false, false)]
        //[MemberData(nameof(IncorrectCallParameters), true, false)]
        //[MemberData(nameof(FaultedInvocationParameters), false, true)]
        public void Test(string methodName, object[] args, bool expectErrorResponse, bool expectFaultedInvocation)
        {
            var listener = new Mock<IListener<ITestInterface, RemoteRequest>>();
            Func<RemoteRequest, bool> requestHandler = null;
            listener.Setup(e => e.AddHandler(It.IsAny<Func<RemoteRequest, bool>>()))
                .Callback<Func<RemoteRequest, bool>>(e => requestHandler = e);

            var sender = new Mock<ISender<ITestInterface, RemoteResponse>>();
            sender.Setup(e => e.Send(It.Is<RemoteResponse>(r => r.IsFaulted == expectErrorResponse))).Verifiable();

            var logger = new Mock<ILogger>();
            var implementationMock = new Mock<TestImplementation>();
            var msgHandler =
                new MessageHandler<TestImplementation, ITestInterface>(listener.Object, sender.Object, implementationMock.Object, logger.Object);

            requestHandler(new RemoteRequest(methodName, args.Select(JsonConvert.SerializeObject).ToArray()))
                .ShouldBe(!expectFaultedInvocation);

            if (!expectFaultedInvocation)
                sender.Verify();
        }

        public static IEnumerable<object[]> CorrectCallParameters(bool expectError, bool expectFaulted)
        {
            return _correctCallParameters.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }

        public static IEnumerable<object[]> IncorrectCallParameters(bool expectError, bool expectFaulted)
        {
            return _incorrectCallParameters.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }

        public static IEnumerable<object[]> FaultedInvocationParameters(bool expectError, bool expectFaulted)
        {
            return _faultedInvocationParameters.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }
    }
}
