using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.UI.HUD;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema para gerenciar a UI de vitais
    /// </summary>
    public class VitalsUISystem : ModSystem
    {
        private UserInterface vitalsUI;
        private VitalsUI vitalsUIState;

        public override void Load()
        {
            vitalsUI = new UserInterface();
            vitalsUIState = new VitalsUI();
            vitalsUI.SetState(vitalsUIState);
        }

        public override void Unload()
        {
            vitalsUI = null;
            vitalsUIState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.gameMenu || Main.LocalPlayer == null) return;
            
            vitalsUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Adicionar a UI de vitais acima da interface do inventário
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex + 1, new LegacyGameInterfaceLayer(
                    "Wolfgodrpg: Vitals UI",
                    delegate
                    {
                        if (Main.gameMenu || Main.LocalPlayer == null) return true;
                        
                        vitalsUI?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
} 