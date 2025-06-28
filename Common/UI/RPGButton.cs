using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;

namespace Wolfgodrpg.Common.UI
{
    public class RPGButton : UIElement
    {
        private Texture2D _buttonTexture;
        private UIText _buttonText;
        private string _texturePath;
        private string _text;

        public RPGButton(string text, string texturePath = "Wolfgodrpg/Assets/UI/ButtonNext")
        {
            _text = text;
            _texturePath = texturePath;
        }

        public override void OnInitialize()
        {
            _buttonTexture = ModContent.Request<Texture2D>(_texturePath).Value;
            Width.Set(_buttonTexture.Width, 0f);
            Height.Set(_buttonTexture.Height, 0f);

            _buttonText = new UIText(_text, 0.9f);
            _buttonText.HAlign = 0.5f;
            _buttonText.VAlign = 0.5f;
            Append(_buttonText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_buttonTexture != null)
            {
                CalculatedStyle dimensions = GetDimensions();
                spriteBatch.Draw(_buttonTexture, dimensions.ToRectangle(), Color.White);
            }
        }
    }
}