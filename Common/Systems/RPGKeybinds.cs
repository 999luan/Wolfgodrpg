using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria; // Adicionado para Main
using Wolfgodrpg.Common.UI;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGKeybinds : ModSystem
    {
        
        public static ModKeybind OpenRPGMenuKeybind { get; private set; }

        

        public override void PostSetupContent()
        {
            
            OpenRPGMenuKeybind = KeybindLoader.RegisterKeybind(Mod, "OpenRPGMenu", "M");
        }

        
    }

    public class RPGKeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            

            if (RPGKeybinds.OpenRPGMenuKeybind.JustPressed)
            {
                RPGMenuController.ToggleMenu();
            }
        }
    }
}
 