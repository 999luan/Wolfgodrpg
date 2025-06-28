// using Terraria;
// using Terraria.ModLoader;
// using Wolfgodrpg.Common.Players;
// using Terraria.DataStructures; 
// using Microsoft.Xna.Framework;

// namespace Wolfgodrpg.Common.Systems
// {
//     public class RPGFishingProjectile : GlobalProjectile
//     {
//         public override void OnCatchFish(Projectile projectile, Item fish, FishingAttempt fishingAttempt)
//         {
//             if (projectile.owner == Main.myPlayer)
//             {
//                 var player = Main.LocalPlayer;
//                 var rpgPlayer = player.GetModPlayer<RPGPlayer>();
//                 float xpAmount = fish.value * 0.001f; 
//                 xpAmount = System.Math.Max(1f, xpAmount); 
//                 rpgPlayer.GainClassExp("fishing", xpAmount);
//             }
//         }
//     }
// }