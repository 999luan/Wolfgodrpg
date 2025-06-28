using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Wolfgodrpg.Common.UI;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGKeybinds : ModSystem
    {
        public static ModKeybind ShowStatsKeybind { get; private set; }
        public static ModKeybind OpenRPGMenuKeybind { get; private set; }

        private UserInterface _rpgStatsInterface;
        internal RPGStatsUI _rpgStatsUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                _rpgStatsInterface = new UserInterface();
                _rpgStatsUI = new RPGStatsUI();
                _rpgStatsUI.Initialize();
                // Não definimos o estado aqui para que ele comece invisível
            }
        }

        public override void Unload()
        {
            _rpgStatsInterface = null;
            _rpgStatsUI = null;
        }

        public override void PostSetupContent()
        {
            ShowStatsKeybind = KeybindLoader.RegisterKeybind(Mod, "ShowRPGStats", "R");
            OpenRPGMenuKeybind = KeybindLoader.RegisterKeybind(Mod, "OpenRPGMenu", "M");
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _rpgStatsInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Wolfgodrpg: RPG Stats UI",
                    delegate
                    {
                        if (_rpgStatsUI.IsVisible())
                        {
                            _rpgStatsInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    public class RPGKeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (RPGKeybinds.ShowStatsKeybind.JustPressed)
            {
                var keybindSystem = ModContent.GetInstance<RPGKeybinds>();
                if (keybindSystem._rpgStatsUI.IsVisible())
                {
                    keybindSystem._rpgStatsInterface.SetState(null);
                }
                else
                {
                    keybindSystem._rpgStatsInterface.SetState(keybindSystem._rpgStatsUI);
                }
                keybindSystem._rpgStatsUI.ToggleVisibility();
            }

            if (RPGKeybinds.OpenRPGMenuKeybind.JustPressed)
            {
                RPGMenuController.ToggleMenu();
            }
        }
    }
}
 