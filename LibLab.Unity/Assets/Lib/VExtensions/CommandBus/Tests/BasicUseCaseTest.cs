using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;

namespace VExtensions.CommandBus.Tests
{
    public class BasicUseCaseTest : TestWithCancellationToken
    {
        private readonly IObjectResolver _resolver;

        public BasicUseCaseTest()
        {
            var bld = new ContainerBuilder();
            bld.UseCommandBus();
            bld.Register<FullNameHandler>(Lifetime.Singleton);
            bld.Register<CommandHandlerRegistry>(container =>
            {
                var registry = new CommandHandlerRegistry();
                registry.TryAdd(typeof(GetFullName), container.Resolve<FullNameHandler>());
                return registry;
            }, Lifetime.Singleton);
            // bld.Register<FullNameHandler>(Lifetime.Singleton).As<ICommandHandler<ICommand<string>, string>>();
            _resolver = bld.Build();
        }

        [UnityTest]
        public IEnumerator ExecuteGetFullName() => UniTask.ToCoroutine(async () =>
        {
            string result = await new GetFullName()
            {
                FirstName = "John", LastName = "Doe"
            }.ExecuteAsync(DisposeCancellationToken);
            Assert.AreEqual("John Doe", result, "Full name should be 'John Doe'");
        });

        protected override void OnDisposed()
        {
            _resolver.Dispose();
        }
    }
}
