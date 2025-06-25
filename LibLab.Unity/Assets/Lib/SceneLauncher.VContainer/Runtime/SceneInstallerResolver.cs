// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public class SceneInstallerResolver
    {
        private static readonly Dictionary<string, IInstaller> _installers = new();

        public IInstaller Resolve(Scene scene)
        {
            if (_installers.TryGetValue(scene.path, out var installer))
            {
                return installer;
            }

            return UnitInstaller.Instance;
        }

        public void Register(string path, IInstaller installer)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(path));
            Assert.IsNotNull(installer);
            _installers[path] = installer;
        }
    }
}
