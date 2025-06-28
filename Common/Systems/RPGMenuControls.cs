using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.UI;
using System.Collections.Generic;
using Wolfgodrpg.Common.UI;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGMenuControls : ModSystem
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                RPGMenuController.Initialize();
            }
        }

        public override void Unload()
        {
            RPGMenuController.Unload();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            RPGMenuController.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Wolfgodrpg: RPG Stats",
                    delegate
                    {
                        RPGMenuController.Draw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    public static class RPGMenuController
    {
        private static UserInterface _menuInterface;
        private static RPGStatsUI _statsUI;

        public static void Initialize()
        {
            if (!Main.dedServ)
            {
                _menuInterface = new UserInterface();
                _statsUI = new RPGStatsUI();
                _statsUI.Initialize();
                _menuInterface.SetState(_statsUI);
            }
        }

        public static void Unload()
        {
            _menuInterface = null;
            _statsUI = null;
        }

        public static void Update(GameTime gameTime)
        {
            _menuInterface?.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            _menuInterface?.Draw(spriteBatch, new GameTime());
        }
    }
} 