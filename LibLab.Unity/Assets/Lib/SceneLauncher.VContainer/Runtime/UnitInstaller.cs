﻿// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using VContainer;
using VContainer.Unity;

namespace SceneLauncher.VContainer
{
    public class UnitInstaller : IInstaller
    {
        public static readonly UnitInstaller Instance = new();

        private UnitInstaller()
        {

        }

        public void Install(IContainerBuilder builder)
        {
        }
    }
}
