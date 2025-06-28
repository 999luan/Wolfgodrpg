using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;

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
                    "Wolfgodrpg: RPG Menu", // Nome da camada da interface
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
}