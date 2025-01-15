#if VCONTAINER
using System;
using NUnit.Framework;
using SceneLauncher.VContainer.Internal;
using VContainer;

namespace SceneLauncher.Tests.Editor
{
    [TestFixture]
    public class InternalTest
    {
        [Test]
        public void AddSceneInstaller()
        {
            var installers = new SceneInstallers();
            var parser = new ScenePathParser();
            Assert.Throws<ArgumentException>(() =>
            {
                installers.Add(new SceneInstaller(""), parser, SceneInstallers.AddMode.Sub);
                installers.Add(new SceneInstaller(""), parser, SceneInstallers.AddMode.Sub);
            });
        }

        [Test]
        public void MatchAliases()
        {
            var libPath = "Assets/Lib";
            var parser = new ScenePathParser
            {
                Aliases =
                {
                    ["$lib"] = libPath
                }
            };

            // Success
            {
                var scenePath = "Scenes/Scene.unity";
                switch (parser.Parse($"$lib/{scenePath}"))
                {
                    case ParseSuccess(var value):
                        Assert.AreEqual($"{libPath}/{scenePath}", value);
                        break;
                    case ParseFailure(var error):
                        Assert.Fail(error.ToString());
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }

            // Failure
            {
                switch (parser.Parse("----failure----/Scenes/Scene.unity"))
                {
                    case ParseSuccess(var value):
                        Assert.Fail(value);
                        break;
                    case ParseFailure(var error):
                        Assert.AreEqual(ParseError.NotStartedAlias, error);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }

                switch (parser.Parse("$----failure----"))
                {
                    case ParseSuccess(var value):
                        Assert.Fail(value);
                        break;
                    case ParseFailure(var error):
                        Assert.AreEqual(ParseError.NotIncludeAliasSymbol, error);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }

                switch (parser.Parse("$----failure----/Scenes/Scene.unity"))
                {
                    case ParseSuccess(var value):
                        Assert.Fail(value);
                        break;
                    case ParseFailure(var error):
                        Assert.AreEqual(ParseError.NotFoundAlias, error);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
    }

    internal class SceneInstaller : ISceneInstaller
    {
        public SceneInstaller(string path)
        {
            ScenePath = path;
        }

        public string ScenePath { get; }

        public void Install(IContainerBuilder builder)
        {
        }
    }
}
#endif