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
        [UnityTest]
        public IEnumerator ExecuteGetFullName() => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();
            bld.UseCommandBus(bus => { bus.AddCommand<GetFullName, FullNameHandler, string>(); });
            using var resolver = bld.Build();
            string result = await new GetFullName() { FirstName = "John", LastName = "Doe" }
                .ExecuteAsync(DisposeCancellationToken);
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
                    .ExecuteAsync(DisposeCancellationToken);
            }, "Should throw an exception for unregistered command");
        }

        [UnityTest]
        public IEnumerator ExecuteFilteredGetFullName() => UniTask.ToCoroutine(async () =>
        {
            var bld = new ContainerBuilder();

            bld.UseCommandBus(bus => { bus.AddFilterCommand<GetFullName, FilteredFullNameHandler, string>(); });
            using var resolver = bld.Build();
            string result = await new GetFullName() { FirstName = "John", LastName = "Doe" }
                .ExecuteAsync(DisposeCancellationToken);
            Assert.AreEqual("John Doe (filtered)", result, "Full name should be 'John Doe (filtered)'");
        });
    }
}
