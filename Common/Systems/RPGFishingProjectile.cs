using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGFishingProjectile : GlobalProjectile
    {
        public override void OnCatchFish(Projectile projectile, Item fish, int itemDrop)
        {
            if (projectile.owner == Main.myPlayer)
            {
                var player = Main.LocalPlayer;
                var rpgPlayer = player.GetModPlayer<RPGPlayer>();
                // Grant fishing XP based on fish rarity or value
                float xpAmount = fish.value * 0.001f; // Example: 0.1% of fish value as XP
                xpAmount = System.Math.Max(1f, xpAmount); // Minimum 1 XP
                rpgPlayer.GainClassExp("fishing", xpAmount);
            }
        }
    }
}