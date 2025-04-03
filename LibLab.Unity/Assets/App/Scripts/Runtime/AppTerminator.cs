// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace App
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class AppTerminator
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int TerminateProcess(IntPtr hProcess, uint exitCode);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitializeOnLoad()
    {
        Application.quitting += Application_Quitting;
    }

    private static void Application_Quitting()
    {
        IntPtr handle = System.Diagnostics.Process.GetCurrentProcess().Handle;
        TerminateProcess(handle, 0);
    }
#endif
    }
}
