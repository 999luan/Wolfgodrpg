using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;

namespace Wolfgodrpg.Common.UI.Base
{
    public class RPGTabButton : UIElement
    {
        private Asset<Texture2D> _normalTextureAsset;
        private Asset<Texture2D> _selectedTextureAsset;
        private UIText _buttonText;
        private string _text;
        private string _normalTexturePath;
        private string _selectedTexturePath;
        private bool _isSelected = false;
        private bool _dimensionsSet = false;

        public event UIElement.MouseEvent OnClick;

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        public string Text => _text;

        public RPGTabButton(string text, string normalTexturePath = "Wolfgodrpg/Assets/UI/ButtonNext", string selectedTexturePath = "Wolfgodrpg/Assets/UI/ButtonPrevious")
        {
            _text = text;
            _normalTexturePath = normalTexturePath;
            _selectedTexturePath = selectedTexturePath;
        }

        public override void OnInitialize()
        {
            _normalTextureAsset = ModContent.Request<Texture2D>(_normalTexturePath);
            _selectedTextureAsset = ModContent.Request<Texture2D>(_selectedTexturePath);

            _buttonText = new UIText(_text, 0.9f);
            _buttonText.HAlign = 0.5f;
            _buttonText.VAlign = 0.5f;
            Append(_buttonText);

            // Attach click event
            OnMouseOver += (evt, listeningElement) => {
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.MenuTick);
            };
            OnLeftClick += (evt, listeningElement) => {
                OnClick?.Invoke(evt, listeningElement);
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_dimensionsSet && _normalTextureAsset.IsLoaded)
            {
                Width.Set(_normalTextureAsset.Value.Width, 0f);
                Height.Set(_normalTextureAsset.Value.Height, 0f);
                _dimensionsSet = true;
                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (_dimensionsSet && _normalTextureAsset.IsLoaded && _selectedTextureAsset.IsLoaded)
            {
                Texture2D currentTexture = _isSelected ? _selectedTextureAsset.Value : _normalTextureAsset.Value;
                CalculatedStyle dimensions = GetDimensions();
                spriteBatch.Draw(currentTexture, dimensions.ToRectangle(), Color.White);
            }
        }
    }
}
