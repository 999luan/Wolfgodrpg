using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // Adicionado para SpriteBatch
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Wolfgodrpg.Common.Players; // Adicionado para RPGPlayer
using Terraria.Utilities; // Adicionado para Utils
using Terraria.GameContent; // Adicionado para FontAssets
using Terraria.UI.Chat;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGMenuControls : ModSystem
    {
        public override void PostSetupContent()
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
                        RPGMenuController.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
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
                        Vector2 position = new Vector2(Main.screenWidth - 50, Main.screenHeight - 50); // Posição inicial, canto inferior direito

                        // Desenhar Stamina (desenhar de baixo para cima para alinhar ao fundo)
                        string staminaText = $"Stamina: {rpgPlayer.CurrentStamina:F0}%";
                        Vector2 staminaSize = FontAssets.MouseText.Value.MeasureString(staminaText);
                        position.X -= staminaSize.X; // Ajustar X para alinhar à direita
                        position.Y -= staminaSize.Y; // Ajustar Y para alinhar ao fundo
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, staminaText, position, Color.Yellow, 0f, Vector2.Zero, Vector2.One);

                        // Desenhar Sanidade
                        string sanityText = $"Sanidade: {rpgPlayer.CurrentSanity:F0}%";
                        Vector2 sanitySize = FontAssets.MouseText.Value.MeasureString(sanityText);
                        position.X = Main.screenWidth - 20 - sanitySize.X; // Resetar X e ajustar
                        position.Y -= sanitySize.Y + 2; // Mover para cima
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, sanityText, position, Color.Purple, 0f, Vector2.Zero, Vector2.One);

                        // Desenhar Fome
                        string hungerText = $"Fome: {rpgPlayer.CurrentHunger:F0}%";
                        Vector2 hungerSize = FontAssets.MouseText.Value.MeasureString(hungerText);
                        position.X = Main.screenWidth - 20 - hungerSize.X; // Resetar X e ajustar
                        position.Y -= hungerSize.Y + 2; // Mover para cima
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, hungerText, position, Color.Orange, 0f, Vector2.Zero, Vector2.One);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}