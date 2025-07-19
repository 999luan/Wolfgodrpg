using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Wolfgodrpg.Common.UI;

namespace Wolfgodrpg.Common.Systems
{
    public class WolfgodUISystem : ModSystem
    {
        internal UserInterface MasterUserInterface;
        internal MasterUIState MasterUIState;

        public override void Load()
        {
            MasterUserInterface = new UserInterface();
            MasterUIState = new MasterUIState();
            // Do not set the state here to prevent loading errors.
        }

        public void ShowMasterUI()
        {
            MasterUserInterface?.SetState(MasterUIState);
        }

        public void HideMasterUI()
        {
            MasterUserInterface?.SetState(null);
        }

        public void ToggleMasterUI()
        {
            if (MasterUserInterface?.CurrentState == null)
            {
                ShowMasterUI();
            }
            else
            {
                HideMasterUI();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (MasterUserInterface?.CurrentState != null)
            {
                MasterUserInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Wolfgod RPG: Master UI",
                    delegate
                    {
                        if (MasterUserInterface?.CurrentState != null)
                        {
                            MasterUserInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
