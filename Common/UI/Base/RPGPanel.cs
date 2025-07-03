using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Wolfgodrpg.Common.UI.Base
{
    public class RPGPanel : UIElement
    {
        private Texture2D _backgroundTexture;
        private readonly string _texturePath;

        // Margens para o 9-slice scaling (ajuste conforme a sua imagem uibg.png)
        // Ex: se a borda tem 12 pixels de largura/altura, use 12.
        private const int BorderSize = 12; 

        public RPGPanel(string texturePath = null)
        {
            _texturePath = texturePath ?? "Wolfgodrpg/Assets/UI/uibg";
        }

        public override void OnInitialize()
        {
            _backgroundTexture = ModContent.Request<Texture2D>(_texturePath).Value;
            if (_backgroundTexture == null)
            {
                Wolfgodrpg.Instance.Logger.Warn($"[RPGPanel] Failed to load background texture from: {_texturePath}");
            }
            else
            {
                Wolfgodrpg.Instance.Logger.Info($"[RPGPanel] Loaded background texture: {_texturePath}, Dimensions: {_backgroundTexture.Width}x{_backgroundTexture.Height}");
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (_backgroundTexture == null)
                return;

            CalculatedStyle dimensions = GetDimensions();
            Rectangle drawRectangle = dimensions.ToRectangle();

            // Implementação básica de 9-slice scaling
            // Ajuste BorderSize conforme a sua textura
            int left = BorderSize;
            int right = _backgroundTexture.Width - BorderSize;
            int top = BorderSize;
            int bottom = _backgroundTexture.Height - BorderSize;

            // Desenha os 9 patches
            // Top-Left
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X, drawRectangle.Y, left, top), new Rectangle(0, 0, left, top), Color.White);
            // Top-Right
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.Right - right, drawRectangle.Y, right, top), new Rectangle(right, 0, right, top), Color.White);
            // Bottom-Left
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X, drawRectangle.Bottom - bottom, left, bottom), new Rectangle(0, bottom, left, bottom), Color.White);
            // Bottom-Right
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.Right - right, drawRectangle.Bottom - bottom, right, bottom), new Rectangle(right, bottom, right, bottom), Color.White);

            // Top
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X + left, drawRectangle.Y, drawRectangle.Width - left - right, top), new Rectangle(left, 0, _backgroundTexture.Width - left - right, top), Color.White);
            // Bottom
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X + left, drawRectangle.Bottom - bottom, drawRectangle.Width - left - right, bottom), new Rectangle(left, bottom, _backgroundTexture.Width - left - right, bottom), Color.White);
            // Left
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X, drawRectangle.Y + top, left, drawRectangle.Height - top - bottom), new Rectangle(0, top, left, _backgroundTexture.Height - top - bottom), Color.White);
            // Right
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.Right - right, drawRectangle.Y + top, right, drawRectangle.Height - top - bottom), new Rectangle(right, top, right, _backgroundTexture.Height - top - bottom), Color.White);

            // Center
            spriteBatch.Draw(_backgroundTexture, new Rectangle(drawRectangle.X + left, drawRectangle.Y + top, drawRectangle.Width - left - right, drawRectangle.Height - top - bottom), new Rectangle(left, top, _backgroundTexture.Width - left - right, _backgroundTexture.Height - top - bottom), Color.White);
        }
    }
}