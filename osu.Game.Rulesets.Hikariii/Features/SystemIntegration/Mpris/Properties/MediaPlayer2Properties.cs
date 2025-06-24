#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using M.DBus;
using Tmds.DBus;

namespace osu.Game.Rulesets.Hikariii.Features.SystemIntegration.Mpris.Properties;

[Dictionary]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "ConvertToAutoProperty")]
[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
public class MediaPlayer2Properties
{
    private readonly bool _CanQuit = true;

    public bool CanQuit => _CanQuit;

    private readonly bool _Fullscreen = false;

    public bool Fullscreen => _Fullscreen;

    private readonly bool _CanSetFullscreen = false;

    public bool CanSetFullscreen => _CanSetFullscreen;

    private readonly bool _CanRaise = true;

    public bool CanRaise => _CanRaise;

    private readonly bool _HasTrackList = false;

    public bool HasTrackList => _HasTrackList;

    private readonly string _Identity = "osu!";

    public string Identity => _Identity;

    private readonly string _DesktopEntry = "osu!";

    public string DesktopEntry => _DesktopEntry;

    private readonly string[] _SupportedUriSchemes =
    {
        "osu://"
    };

    public string[] SupportedUriSchemes => _SupportedUriSchemes;

    private readonly string[] _SupportedMimeTypes = Array.Empty<string>();

    public string[] SupportedMimeTypes => _SupportedMimeTypes;

    private IDictionary<string, object> members;

    public object Get(string prop)
    {
        ServiceUtils.CheckIfDirectoryNotReady(this, members, out members);
        return ServiceUtils.GetValueFor(this, prop, members);
    }

    internal bool Set(string name, object newValue)
    {
        ServiceUtils.CheckIfDirectoryNotReady(this, members, out members);
        return ServiceUtils.SetValueFor(this, name, newValue, members);
    }

    internal bool Contains(string prop)
    {
        ServiceUtils.CheckIfDirectoryNotReady(this, members, out members);
        return ServiceUtils.CheckifContained(this, prop, members);
    }
}
