using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SideBar.Settings.Items;
using osu.Game.Rulesets.Hikariii.Features.Player.Plugins;

namespace osu.Game.Rulesets.Hikariii.Features.Player.Graphics.SettingsItems
{
    public abstract class SettingsEntry
    {
        public IBindable Bindable { get; set; } = null!;

        public LocalisableString Name
        {
            get => name;
            set
            {
                if (string.IsNullOrEmpty(name.ToString()))
                    name = value;
                else if (AllowChangeName)
                    name = value;
                else
                    throw new InvalidOperationException("不允许覆盖原始名称");
            }
        }

        public LocalisableString Description
        {
            get => desc;
            set
            {
                if (string.IsNullOrEmpty(desc.ToString()))
                    desc = value;
                else if (AllowChangeDescription)
                    desc = value;
                else
                    throw new InvalidOperationException("不允许覆盖原始名称");
            }
        }

        public IconUsage Icon = FontAwesome.Regular.QuestionCircle;

        protected bool AllowChangeName;
        protected bool AllowChangeDescription;

        private LocalisableString name;
        private LocalisableString desc;

        public abstract Drawable ToSettingsItem();
        public abstract Drawable? ToLLinSettingsItem();
    }

    public class SeparatorSettingsEntry : SettingsEntry
    {
        public override Drawable ToSettingsItem()
        {
            return new OsuSpriteText
            {
                Text = Name,
                Font = OsuFont.GetFont(size: 19),
                Margin = new MarginPadding { Horizontal = 20, Vertical = (11.5f / 2) }
            };
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsSeparatorPiece
            {
                Description = Name,
                Icon = this.Icon
            };
        }
    }

    public class NumberSettingsEntry<T> : SettingsEntry
        where T : struct, INumber<T>, IMinMaxValue<T>, IConvertible
    {
        public bool DisplayAsPercentage = false;
        public float KeyboardStep = 0.1f;
        public bool CommitOnMouseRelease = false;

        public NumberSettingsEntry()
        {
            Icon = FontAwesome.Solid.SlidersH;
        }

        public override Drawable ToSettingsItem()
        {
            return new SettingsItemV2(new FormSliderBar<T>()
            {
                //todo: 感觉这么做有些dirty，但起码能用
                //todo: 可以换成"Current = { BindTarget = ... }"这样？
                Current = (Bindable<T>)Bindable.GetBoundCopy(),
                Caption = Name,
                HintText = Description,
                DisplayAsPercentage = this.DisplayAsPercentage,
                KeyboardStep = this.KeyboardStep,
                TransferValueOnCommit = CommitOnMouseRelease
            });
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsSliderPiece<T>
            {
                Description = Name,
                TooltipText = Description,
                Bindable = (Bindable<T>)Bindable.GetBoundCopy(),
                Icon = this.Icon,
                DisplayAsPercentage = this.DisplayAsPercentage,
                TransferValueOnCommit = CommitOnMouseRelease
            };
        }
    }

    public class BooleanSettingsEntry : SettingsEntry
    {
        public BooleanSettingsEntry()
        {
            Icon = FontAwesome.Solid.ToggleOn;
        }

        public override Drawable ToSettingsItem()
        {
            return new SettingsItemV2(new FormCheckBox
            {
                Current = (Bindable<bool>)Bindable.GetBoundCopy(),
                Caption = Name,
                HintText = Description
            });
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsTogglePiece
            {
                Description = Name,
                TooltipText = Description,
                Bindable = (Bindable<bool>)Bindable.GetBoundCopy(),
                Icon = this.Icon
            };
        }
    }

    public partial class ListSettingsEntry<T> : SettingsEntry
    {
        public IEnumerable<T>? Values;

        public ListSettingsEntry()
        {
            Icon = FontAwesome.Solid.List;
        }

        public override Drawable ToSettingsItem()
        {
            return new SettingsItemV2(new LinguaFormDropdown<T>
            {
                Caption = Name,
                Current = (Bindable<T>)Bindable.GetBoundCopy(),
                Items = Values
            });
        }

        public partial class LinguaFormDropdown<X> : FormDropdown<X>
        {
            protected override LocalisableString GenerateItemText(X item)
            {
                return item.GetLocalisableDescription();
            }
        }

        public partial class MOsuDropdown<X> : OsuDropdown<X>
        {
            public MOsuDropdown()
            {
                RelativeSizeAxes = Axes.X;
            }

            protected override LocalisableString GenerateItemText(X item)
            {
                if (item is LLinPluginProvider provider)
                    return provider.GetLocalisableDescription();

                return base.GenerateItemText(item);
            }
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsListPiece<T>
            {
                Description = Name,
                TooltipText = Description,
                Bindable = (Bindable<T>)Bindable.GetBoundCopy(),
                Icon = this.Icon,
                Values = this.Values?.ToList() ?? new List<T>()
            };
        }
    }

    public class StringSettingsEntry : SettingsEntry
    {
        public StringSettingsEntry()
        {
            Icon = FontAwesome.Solid.TextWidth;
        }

        public override Drawable ToSettingsItem()
        {
            return new SettingsItemV2(new FormTextBox()
            {
                Current = (Bindable<string>)Bindable.GetBoundCopy(),
                Caption = Name,
                HintText = Description.ToString()
            });
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsStringPiece
            {
                Bindable = (Bindable<string>)Bindable.GetBoundCopy(),
                Description = Name,
                TooltipText = Description,
                Icon = this.Icon
            };
        }
    }

    public class EnumSettingsEntry<T> : SettingsEntry
        where T : struct, Enum
    {
        public EnumSettingsEntry()
        {
            Icon = FontAwesome.Solid.List;
        }

        public override Drawable ToSettingsItem()
        {
            return new SettingsItemV2(new FormEnumDropdown<T>
            {
                Current = (Bindable<T>)Bindable.GetBoundCopy(),
                Caption = Name,
                HintText = Description.ToString()
            });
        }

        public override Drawable? ToLLinSettingsItem()
        {
            return new SettingsEnumPiece<T>
            {
                Description = Name,
                TooltipText = Description,
                Bindable = (Bindable<T>)Bindable.GetBoundCopy(),
                Icon = this.Icon
            };
        }
    }
}
