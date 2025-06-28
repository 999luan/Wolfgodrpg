using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; // Adicionado para SpriteBatch
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.UI;

namespace Wolfgodrpg.Common.Systems
{
    // Static menu controller class
    public static class RPGMenuController
    {
        private static UserInterface _menuInterface;
        private static SimpleRPGMenu _menuUI;

        public static void Initialize()
        {
            if (_menuInterface == null)
            {
                _menuInterface = new UserInterface();
                _menuUI = new SimpleRPGMenu();
                _menuUI.Initialize();
            }
        }

        public static void Unload()
        {
            _menuInterface = null;
            _menuUI = null;
        }

        public static void Update(GameTime gameTime)
        {
            if (_menuUI != null && _menuUI.IsVisible())
            {
                _menuInterface.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (_menuUI != null && _menuUI.IsVisible())
            {
                _menuInterface.Draw(spriteBatch, new GameTime());
            }
        }

        public static void ToggleMenu()
        {
            if (_menuUI == null) return;

            if (_menuUI.IsVisible())
            {
                _menuUI.Hide();
                _menuInterface?.SetState(null);
            }
            else
            {
                _menuUI.Show();
                _menuInterface?.SetState(_menuUI);
            }
        }

        
    }
}