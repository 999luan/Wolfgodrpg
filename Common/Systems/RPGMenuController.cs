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
            if (_menuInterface != null) return;

            _menuInterface = new UserInterface();
            _menuUI = new SimpleRPGMenu();
            _menuUI.Activate();
        }

        public static void Unload()
        {
            _menuInterface = null;
            _menuUI = null;
        }

        public static void Update(GameTime gameTime)
        {
            _menuInterface?.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuInterface?.CurrentState != null)
            {
                _menuInterface.Draw(spriteBatch, gameTime);
            }
        }

        public static void ToggleMenu()
        {
            if (_menuInterface?.CurrentState == null)
            {
                _menuInterface?.SetState(_menuUI);
            }
            else
            {
                _menuInterface?.SetState(null);
            }
        }
    }
}