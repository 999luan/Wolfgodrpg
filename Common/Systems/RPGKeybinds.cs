using Terraria.ModLoader;
using Terraria;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGKeybinds : ModSystem
    {
        public static ModKeybind ToggleMasterUIKeybind { get; private set; }
        public static ModKeybind CombatModeKey { get; private set; }

        public override void Load()
        {
            ToggleMasterUIKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Wolfgod UI", "K");
            CombatModeKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Combat Mode", "Q");
        }

        public override void Unload()
        {
            ToggleMasterUIKeybind = null;
            CombatModeKey = null;
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
            
            // Toggle modo de combate
            if (RPGKeybinds.CombatModeKey.JustPressed)
            {
                var rpgPlayer = Player.GetModPlayer<Common.Players.RPGPlayer>();
                if (rpgPlayer != null)
                {
                    rpgPlayer.CombatModeActive = !rpgPlayer.CombatModeActive;
                    Main.NewText($"Modo de Combate {(rpgPlayer.CombatModeActive ? "Ativado" : "Desativado")}", 
                        rpgPlayer.CombatModeActive ? Microsoft.Xna.Framework.Color.Green : Microsoft.Xna.Framework.Color.Gray);
                }
            }
        }
    }
}
