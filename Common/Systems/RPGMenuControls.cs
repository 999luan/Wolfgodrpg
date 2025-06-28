using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // Adicionado para SpriteBatch
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Wolfgodrpg.Common.Players; // Adicionado para RPGPlayer
using Terraria.Utilities; // Adicionado para Utils
using Terraria.GameContent; // Adicionado para FontAssets

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

            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Wolfgodrpg: RPG Vitals",
                    delegate
                    {
                        var rpgPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                        SpriteBatch spriteBatch = Main.spriteBatch;
                        Vector2 position = new Vector2(10, 10); // Posição inicial, ajustar conforme necessário

                        // Desenhar Fome
                        string hungerText = $"Fome: {rpgPlayer.CurrentHunger:F0}%";
                        Utils.DrawBorderStringFourWay(spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, hungerText, position.X, position.Y, Color.Orange, Color.Black, Vector2.Zero, 1f);
                        position.Y += Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(hungerText).Y + 2; // Mover para baixo

                        // Desenhar Sanidade
                        string sanityText = $"Sanidade: {rpgPlayer.CurrentSanity:F0}%";
                        Utils.DrawBorderStringFourWay(spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, sanityText, position.X, position.Y, Color.Purple, Color.Black, Vector2.Zero, 1f);
                        position.Y += Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(sanityText).Y + 2; // Mover para baixo

                        // Desenhar Stamina
                        string staminaText = $"Stamina: {rpgPlayer.CurrentStamina:F0}%";
                        Utils.DrawBorderStringFourWay(spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value, staminaText, position.X, position.Y, Color.Yellow, Color.Black, Vector2.Zero, 1f);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}