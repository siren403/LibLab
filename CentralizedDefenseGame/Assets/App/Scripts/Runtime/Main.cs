// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using SceneLauncher.VContainer;
using UnityEngine;

namespace App
{
    public static class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            var resolver = new SceneInstallerResolver();
            resolver.Register("Assets/App/Scenes/App_MainScene.unity", new Scenes.MainScene());
            resolver.Register("Assets/App/Scenes/App_DefenseGameScene.unity", new Scenes.DefenseGame.DefenseGameScene());
            SceneScopeInitializer.Initialize(resolver);
        }
    }
}
