using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Wolfgodrpg.Common.UI
{
    public class RPGTabButton : UIElement
    {
        private Texture2D _normalTexture;
        private Texture2D _selectedTexture;
        private UIText _buttonText;
        private string _text;
        private string _normalTexturePath;
        private string _selectedTexturePath;
        private bool _isSelected = false;

        public event UIElement.MouseEvent OnClick;

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        public RPGTabButton(string text, string normalTexturePath = "Wolfgodrpg/Assets/UI/ButtonNext", string selectedTexturePath = "Wolfgodrpg/Assets/UI/ButtonPrevious")
        {
            _text = text;
            _normalTexturePath = normalTexturePath;
            _selectedTexturePath = selectedTexturePath;
        }

        public override void OnInitialize()
        {
            _normalTexture = ModContent.Request<Texture2D>(_normalTexturePath).Value;
            _selectedTexture = ModContent.Request<Texture2D>(_selectedTexturePath).Value;

            Width.Set(_normalTexture.Width, 0f);
            Height.Set(_normalTexture.Height, 0f);

            _buttonText = new UIText(_text, 0.9f);
            _buttonText.HAlign = 0.5f;
            _buttonText.VAlign = 0.5f;
            Append(_buttonText);

            // Attach click event
            OnMouseOver += (evt, listeningElement) => {
                Main.PlaySound(Terraria.ID.SoundID.MenuTick);
            };
            OnLeftClick += (evt, listeningElement) => {
                OnClick?.Invoke(evt, listeningElement);
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Texture2D currentTexture = _isSelected ? _selectedTexture : _normalTexture;
            if (currentTexture != null)
            {
                CalculatedStyle dimensions = GetDimensions();
                spriteBatch.Draw(currentTexture, dimensions.ToRectangle(), Color.White);
            }
        }
    }
}
