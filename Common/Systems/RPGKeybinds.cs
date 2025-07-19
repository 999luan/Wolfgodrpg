using Terraria.ModLoader;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGKeybinds : ModSystem
    {
        public static ModKeybind ToggleMasterUIKeybind { get; private set; }

        public override void Load()
        {
            ToggleMasterUIKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Wolfgod UI", "K");
        }

        public override void Unload()
        {
            ToggleMasterUIKeybind = null;
        }
    }

    public class RPGKeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet)
        {
            if (RPGKeybinds.ToggleMasterUIKeybind.JustPressed)
            {
                ModContent.GetInstance<WolfgodUISystem>().ToggleMasterUI();
            }
        }
    }
}
