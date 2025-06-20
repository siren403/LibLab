using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;
using VitalRouter;
using VitalRouter.R3;
using VitalRouter.VContainer;

namespace VExtensions.CommandBus.Tests
{
    public class BasicUseCaseTest : TestWithCancellationToken
    {
        [UnityTest]
        public IEnumerator ExecuteGetFullName() => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();
            bld.RegisterCommandBus(bus => { bus.AddCommand<GetFullName, GetFullNameHandler, string>(); });
            using var resolver = bld.Build();
            string result = await new GetFullName() { FirstName = "John", LastName = "Doe" }
                .ExecuteAsync<GetFullName, string>(DisposeCancellationToken);
            Assert.AreEqual("John Doe", result, "Full name should be 'John Doe'");
        });

        [Test]
        public void NotRegisteredCommandThrowsException()
        {
            var bld = new ContainerBuilder();
            using var resolver = bld.Build();
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await new GetFullName() { FirstName = "John", LastName = "Doe" }
                    .ExecuteAsync<GetFullName, string>(DisposeCancellationToken);
            }, "Should throw an exception for unregistered command");
        }

        [UnityTest]
        public IEnumerator ExecuteFilteredGetFullName() => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();

            bld.RegisterCommandBus(bus => { bus.AddFilterCommand<GetFullName, FilteredFullNameHandler, string>(); });
            using var resolver = bld.Build();
            string result = await new GetFullName() { FirstName = "John", LastName = "Doe" }
                .ExecuteAsync<GetFullName, string>(DisposeCancellationToken);
            Assert.AreEqual("John Doe (filtered)", result, "Full name should be 'John Doe (filtered)'");
        });

        [Test]
        public void ExecuteSetFullName()
        {
            var bld = new ContainerBuilder();
            bld.RegisterCommandBus(bus => { bus.AddCommand<SetFullName, SetFullNameHandler>(); });
            using var resolver = bld.Build();
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await new SetFullName().ExecuteAsync(DisposeCancellationToken);
            });

            Assert.DoesNotThrowAsync(async () =>
            {
                await new SetFullName() { FirstName = "John", LastName = "Doe" }.ExecuteAsync(
                    DisposeCancellationToken);
            });
        }

        [UnityTest]
        public IEnumerator UserUseCase() => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();
            bld.Register<User>(Lifetime.Singleton).AsSelf();
            bld.RegisterCommandBus(bus =>
            {
                bus.AddCommand<GetUserFirstName, GetUserFirstNameHandler, string>();
                bus.AddCommand<SetUserFirstName, SetUserFirstNameHandler>();
                bus.AddCommand<GetFullName, GetFullNameHandler, string>();
            });
            bld.RegisterVitalRouter(routing => { routing.Filters.Add<Logging>(); });
            using var resolver = bld.Build();
            string firstName = "Jane";
            await new SetUserFirstName() { Name = firstName }.ExecuteAsync(DisposeCancellationToken);

            var tasks = new[]
            {
                new GetUserFirstName().ExecuteAsync<GetUserFirstName, string>(DisposeCancellationToken),
                new GetFullName() { FirstName = firstName, LastName = "Doe" }
                    .ExecuteAsync(DisposeCancellationToken)
            };
            string[] results = await tasks;
            string a = results[0];
            string b = results[1];

            Assert.AreEqual(firstName, a, "First name should match the set value");
            Assert.AreEqual("Jane Doe", b, "Full name should be 'Jane Doe'");

            Router.Default.AsObservable<GetFullName>();
        });
    }
}
