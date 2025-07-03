using Terraria;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Utils
{
    public static class RPGUtils
    {
        public static RPGPlayer GetLocalRPGPlayer()
        {
            if (Main.LocalPlayer?.active == true)
            {
                return Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            }
            return null;
        }

        public static bool IsValidLocalPlayer()
        {
            return Main.LocalPlayer?.active == true;
        }
    }
}
