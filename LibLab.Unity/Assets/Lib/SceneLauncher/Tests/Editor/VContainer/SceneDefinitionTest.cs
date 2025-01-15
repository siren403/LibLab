#if VCONTAINER

using NUnit.Framework;
using SceneLauncher;

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
#endif