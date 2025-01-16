#if VCONTAINER
using System;
using System.Collections.Generic;

namespace SceneLauncher.VContainer.Internal
{
    internal class SceneInstallers
    {
        public enum AddMode
        {
            Main,
            Sub
        }

        private readonly Dictionary<string, ISceneInstaller> _installers = new();

        public string MainScenePath { get; private set; }

        public void Add(ISceneInstaller installer, ScenePathParser parser, AddMode mode)
        {
            switch (parser.Parse(installer.ScenePath))
            {
                case ParseSuccess(var value):
                    if (mode == AddMode.Main)
                    {
                        MainScenePath = value;
                    }

                    _installers.Add(value, installer);
                    break;
                case ParseFailure(ParseError.NotStartedAlias):
                    if (mode == AddMode.Main)
                    {
                        MainScenePath = installer.ScenePath;
                    }

                    _installers.Add(installer.ScenePath, installer);
                    break;
                case ParseFailure(var error):
                    throw new ArgumentException(error.ToString());
            }
        }

        public bool TryGetValue(string key, out ISceneInstaller value, out AddMode mode)
        {
            mode = key == MainScenePath ? AddMode.Main : AddMode.Sub;
            return _installers.TryGetValue(key, out value);
        }

        public void Clear()
        {
            _installers.Clear();
            MainScenePath = null;
        }
    }
}
#endif