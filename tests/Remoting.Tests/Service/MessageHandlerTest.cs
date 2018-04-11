using System;
using System.Collections.Generic;
using System.Linq;
using Milde.Remoting.Communication;
using Milde.Remoting.Contracts;
using Milde.Remoting.Service;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Shouldly;
using Xunit;

namespace Milde.Remoting.Tests.Service
{
    public class MessageHandlerTest
    {
        private static readonly IEnumerable<object[]> _correctCallParameters = new []
        {
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[] {true}},
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new object[] {  }},
            new object[] { nameof(ITestInterface.AsyncObjectMethodWithParameters), new [] { DateTime.Now, true, new object(), typeof(object) }},
        };

        private static readonly IEnumerable<object[]> _incorrectNubmerOfArguments = new []
        {
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[0]},
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[] {true, "unnecessary extra parameter", false}},
            new object[] { nameof(ITestInterface.AsyncObjectMethod), new [] { "unnecessary extra parameter" } },
        };

        private static readonly IEnumerable<object[]> _incorrectTypeOfArguments = new []
        {
            //TODO: Currently any string is successfully serialized to boolean. 
            //We might need to introduce more strict arguments typing 
            //new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[] { 123 }}, 
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new [] {new object()}},
            new object[] { nameof(ITestInterface.AsyncBooleanMethod), new object[] {DateTime.Now}}
        };

        private static readonly IEnumerable<object[]> _unallowedMethodSignature = new []
        {
            new object[] { nameof(ITestInterface.SyncMethod), new object[0] { }},
            new object[] { nameof(ITestInterface.VoidMethod), new object[0] { }},
            new object[] { nameof(ITestInterface.AsyncVoidMethod), new object[0] { } },
            new object[] { nameof(ITestInterface.NotImplementedMethod), new object[0] { } },
        };

        private static readonly IEnumerable<object[]> _faultedMethod = new []
        {
            new object[] { nameof(ITestInterface.NotImplementedMethod), new object[0] { } },
        };

        private static readonly IEnumerable<object[]> _missingMethod = new []
        {
            new object[] { "MissingMethod", new[] { "hello" }},
        };

        [Theory]
        [MemberData(nameof(CorrectCallParameters), false, false)]
        [MemberData(nameof(IncorrectNumberOfArguments), true, false)]
        [MemberData(nameof(IncorrectTypeOfArguments), true, false)]
        [MemberData(nameof(UnallowedMethodSignatures), true, false)]
        [MemberData(nameof(FaultedMethod), true, false)]
        [MemberData(nameof(MissingMethod), true, false)]
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

        public static IEnumerable<object[]> IncorrectNumberOfArguments(bool expectError, bool expectFaulted)
        {
            return _incorrectNubmerOfArguments.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }
        public static IEnumerable<object[]> IncorrectTypeOfArguments(bool expectError, bool expectFaulted)
        {
            return _incorrectTypeOfArguments.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }

        public static IEnumerable<object[]> UnallowedMethodSignatures(bool expectError, bool expectFaulted)
        {
            return _unallowedMethodSignature.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }
        public static IEnumerable<object[]> FaultedMethod(bool expectError, bool expectFaulted)
        {
            return _faultedMethod.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }
        public static IEnumerable<object[]> MissingMethod(bool expectError, bool expectFaulted)
        {
            return _missingMethod.Select(callParameter => new[] { callParameter[0], callParameter[1], expectError, expectFaulted });
        }
    }
}
