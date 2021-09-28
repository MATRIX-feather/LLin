using System.Collections.Generic;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Game.Input.Bindings;

namespace LLin.Game.KeyBind
{
    public class LLinGlobalActionContainer : DatabasedKeyBindingContainer<LLinAction>, IHandleGlobalKeyboardInput
    {
        public LLinGlobalActionContainer()
            : base(matchingMode: KeyCombinationMatchingMode.Modifiers)
        {
        }

        public override IEnumerable<IKeyBinding> DefaultKeyBindings => globalKeyBindings;

        private IEnumerable<IKeyBinding> globalKeyBindings => new[]
        {
            new KeyBinding(new[] { InputKey.LControl }, LLinAction.OpenOptions)
        };
    }

    public enum LLinAction
    {
        OpenOptions
    }
}
