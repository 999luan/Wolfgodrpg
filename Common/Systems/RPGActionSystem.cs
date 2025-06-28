using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.ID; // Added for TileID
using System.Collections.Generic; // Added for HashSet

namespace Wolfgodrpg.Common.Systems
{
    public class RPGActionSystem : ModSystem
    {
        private static float lastPositionX = 0f;
        private static float lastPositionY = 0f;
        private static float distanceTraveled = 0f;
        private static int blocksMined = 0;
        private static int blocksPlaced = 0;
        private static float timeSinceLastRegen = 0f;

        // Resource Tile IDs for Gathering XP
        public static HashSet<int> ResourceTileIDs = new HashSet<int>
        {
            TileID.Copper, TileID.Iron, TileID.Silver, TileID.Gold, TileID.Demonite, TileID.Crimtane, TileID.Meteorite,
            TileID.Cobalt, TileID.Palladium, TileID.Mythril, TileID.Orichalcum, TileID.Adamantite, TileID.Titanium,
            TileID.Wood, TileID.RichMahogany, TileID.BorealWood, TileID.PalmWood, TileID.Ebonwood, TileID.Shadewood,
            TileID.Stone, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Obsidian, TileID.Hellstone
        };

        public override void PostUpdateWorld()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();

            // Movimento (Explorer XP)
            float currentX = player.position.X;
            float currentY = player.position.Y;
            
            if (lastPositionX != 0f && lastPositionY != 0f)
            {
                float distance = Vector2.Distance(
                    new Vector2(lastPositionX, lastPositionY),
                    new Vector2(currentX, currentY)
                );
                
                distanceTraveled += distance;
                
                // A cada 1000 unidades de distância, ganha XP de Explorer
                if (distanceTraveled >= 1000f)
                {
                    rpgPlayer.GainClassExp("movement", 10f);
                    distanceTraveled = 0f;
                }
            }
            
            lastPositionX = currentX;
            lastPositionY = currentY;

            // Regeneração (Survivor XP)
            timeSinceLastRegen += 1f / 60f; // 1/60 = um segundo em ticks
            if (timeSinceLastRegen >= 1f && player.lifeRegen > 0)
            {
                rpgPlayer.GainClassExp("defense", 1f);
                timeSinceLastRegen = 0f;
            }
        }

        public static void OnBlockMine()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            blocksMined++;

            // A cada 10 blocos minerados
            if (blocksMined >= 10)
            {
                rpgPlayer.GainClassExp("mining", 15f);
                blocksMined = 0;
            }
        }

        public static void OnBlockPlace()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            blocksPlaced++;

            // A cada 10 blocos colocados
            if (blocksPlaced >= 10)
            {
                rpgPlayer.GainClassExp("building", 20f);
                blocksPlaced = 0;
            }
        }

        public static void OnCraft(Item item)
        {
            if (item == null || item.IsAir) return;

            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<RPGPlayer>();

            // XP base baseado no valor do item
            float baseXP = item.value * 0.01f;

            // Give XP to building class
            modPlayer.GainClassExp("building", baseXP);
        }

        public static void OnHurt(Player player, int damage)
        {
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            float expAmount = damage * 0.5f; // Metade do dano como XP

            rpgPlayer.GainClassExp("defense", expAmount);
        }

        public static void OnKillNPC(NPC npc)
        {
            if (npc == null) return;

            // Dar XP para todos os jogadores próximos
            foreach (var player in Main.player)
            {
                if (!player.active || player.dead) continue;

                // Verificar distância
                float distance = Vector2.Distance(player.Center, npc.Center);
                if (distance > 1000f) continue; // 1000 pixels = ~62.5 blocos

                var modPlayer = player.GetModPlayer<RPGPlayer>();

                // XP base baseado na vida do NPC
                float baseXP = npc.lifeMax * 0.1f;

                // Bônus para bosses
                if (npc.boss)
                {
                    baseXP *= 10f;
                }

                // Dar XP para todas as classes
                modPlayer.GainClassExp("melee", baseXP);
                modPlayer.GainClassExp("ranged", baseXP);
                modPlayer.GainClassExp("magic", baseXP);
                modPlayer.GainClassExp("summoner", baseXP);
                modPlayer.GainClassExp("defense", baseXP * 0.5f);
                modPlayer.GainClassExp("bestiary", baseXP * 0.2f); // Add XP for bestiary
            }
        }

        public static void OnUseItem(Item item)
        {
            if (item == null || item.IsAir) return;

            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<RPGPlayer>();

            // XP base baseado no dano do item
            float baseXP = item.damage * 0.1f;

            // Dar XP baseado no tipo de dano
            if (item.DamageType == DamageClass.Melee)
            {
                modPlayer.GainClassExp("melee", baseXP);
            }
            else if (item.DamageType == DamageClass.Ranged)
            {
                modPlayer.GainClassExp("ranged", baseXP);
            }
            else if (item.DamageType == DamageClass.Magic)
            {
                modPlayer.GainClassExp("magic", baseXP);
            }
            else if (item.DamageType == DamageClass.Summon)
            {
                modPlayer.GainClassExp("summoner", baseXP);
            }
        }

        public static void OnEatFood(Item item)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            
            // Recuperar fome baseado no preço do item
            float hungerRecovery = MathHelper.Clamp(item.value * 0.01f, 10f, 50f);
            rpgPlayer.CurrentHunger = MathHelper.Clamp(
                rpgPlayer.CurrentHunger + hungerRecovery,
                0f,
                rpgPlayer.MaxHunger
            );

            rpgPlayer.GainClassExp("defense", 5f);
        }
    }
} 