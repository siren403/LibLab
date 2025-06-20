using NUnit.Framework;

namespace SceneLauncher.VContainer.Tests
{
    [TestFixture]
    public class SceneDefinitionTest
    {
        [Test]
        public void Equals()
        {
            var a = new ScenePathDefinition("Assets/");
            var b = new ScenePathDefinition("Assets/");

            Assert.True(a.Equals(b));
        }
    }
}
