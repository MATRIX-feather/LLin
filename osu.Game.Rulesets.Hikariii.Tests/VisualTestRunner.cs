// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework;
using osu.Framework.Platform;

namespace osu.Game.Rulesets.Hikariii.Tests
{
    public static class VisualTestRunner
    {
        [STAThread]
        public static int Main(string[] args)
        {
            using (DesktopGameHost host = Host.GetSuitableDesktopHost(@"osu", new HostOptions { }))
            {
                host.Run(new HikariiiTestBrowser());
                return 0;
            }
        }
    }
}
