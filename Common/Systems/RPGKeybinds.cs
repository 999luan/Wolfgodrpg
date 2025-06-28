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

        private UserInterface _quickStatsInterface;
        internal QuickStatsUI _quickStatsUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                _quickStatsInterface = new UserInterface();
                _quickStatsUI = new QuickStatsUI();
                _quickStatsUI.Initialize();
                _quickStatsInterface.SetState(_quickStatsUI);
            }
        }

        public override void Unload()
        {
            _quickStatsInterface = null;
            _quickStatsUI = null;
        }

        public override void PostSetupContent()
        {
            ShowStatsKeybind = KeybindLoader.RegisterKeybind(Mod, "ShowRPGStats", "R");
            OpenRPGMenuKeybind = KeybindLoader.RegisterKeybind(Mod, "OpenRPGMenu", "M");
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _quickStatsInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Wolfgodrpg: Quick Stats",
                    delegate
                    {
                        if (_quickStatsInterface?.CurrentState != null)
                        {
                            _quickStatsInterface.Draw(Main.spriteBatch, new GameTime());
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
                ModContent.GetInstance<RPGKeybinds>()._quickStatsUI.ToggleVisibility();
            }

            if (RPGKeybinds.OpenRPGMenuKeybind.JustPressed)
            {
                RPGMenuController.ToggleMenu();
            }

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && 
                Main.oldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.E))
            {
                RPGMenuController.NextPage();
            }
            else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) && 
                    Main.oldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                RPGMenuController.PreviousPage();
            }
        }
    }
} 