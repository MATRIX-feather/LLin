using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Users.Drawables;

namespace osu.Game.Rulesets.Hikariii.Graphics;

public partial class AvatarPicture : CompositeDrawable
{
    public static readonly List<int> DEFAULT_VALID_USERS =
    [
        1
    ];

    private readonly int userId;

    public AvatarPicture(int userId)
    {
        this.userId = userId;
    }

    [BackgroundDependencyLoader]
    private void load(APIAccess api)
    {
        this.AddInternal(new UpdateableAvatar(isInteractive: false, user: new APIUser { Id = userId })
        {
            RelativeSizeAxes = Axes.Both
        });
    }
}
