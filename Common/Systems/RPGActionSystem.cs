using System;
using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.ID; // Added for TileID
using System.Collections.Generic; // Added for HashSet
using Wolfgodrpg.Common.Systems; // Added for RPGClassActionMapper

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
        private static int dashesPerformed = 0;
        private static float dashDistance = 0f;

        // Resource Tile IDs for Gathering XP
        // Resource Tile IDs for Gathering XP
        // public static HashSet<int> ResourceTileIDs = new HashSet<int>
        // {
        //     TileID.Copper, TileID.Iron, TileID.Silver, TileID.Gold, TileID.Demonite, TileID.Crimtane, TileID.Meteorite,
        //     TileID.Cobalt, TileID.Palladium, TileID.Mythril, TileID.Orichalcum, TileID.Adamantite, TileID.Titanium,
        //     TileID.Wood, TileID.RichMahogany, TileID.BorealWood, TileID.PalmWood, TileID.Ebonwood, TileID.Shadewood,
        //     TileID.Stone, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Obsidian, TileID.Hellstone
        // };

        public override void PostUpdateWorld()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();

            // Movimento e Dash (Acrobat XP)
            float currentX = player.position.X;
            float currentY = player.position.Y;
            
            if (lastPositionX != 0f && lastPositionY != 0f)
            {
                float distance = Vector2.Distance(
                    new Vector2(lastPositionX, lastPositionY),
                    new Vector2(currentX, currentY)
                );
                
                distanceTraveled += distance;
                
                // Se estiver em dash, acumular distância do dash
                if (player.dash != 0)
                {
                    dashDistance += distance;
                    
                    // A cada 100 unidades de distância em dash, ganha XP de Acrobata
                    if (dashDistance >= 100f)
                    {
                        RPGClassActionMapper.MapMovementAction(MovementAction.Dash, dashDistance);
                        dashDistance = 0f;
                        dashesPerformed++;
                        
                        // Bônus extra a cada 10 dashes
                        if (dashesPerformed >= 10)
                        {
                            RPGClassActionMapper.MapMovementAction(MovementAction.Dash, 250f); // Bônus extra
                            dashesPerformed = 0;
                        }
                    }
                }
                
                // A cada 1000 unidades de distância normal, ganha XP de Acrobata
                if (distanceTraveled >= 1000f)
                {
                    RPGClassActionMapper.MapMovementAction(MovementAction.Walk, distanceTraveled);
                    distanceTraveled = 0f;
                }
            }
            
            lastPositionX = currentX;
            lastPositionY = currentY;

            // Regeneração (Survival XP)
            timeSinceLastRegen += 1f / 60f; // 1/60 = um segundo em ticks
            if (timeSinceLastRegen >= 1f && player.lifeRegen > 0)
            {
                RPGClassActionMapper.MapSurvivalAction(SurvivalAction.RegenerateHealth, player.lifeRegen);
                timeSinceLastRegen = 0f;
            }
        }

        public static void OnBlockMine()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            blocksMined++;

            // A cada 10 blocos minerados
            if (blocksMined >= 10)
            {
                RPGClassActionMapper.MapExplorationAction(ExplorationAction.MineResource, blocksMined);
                blocksMined = 0;
            }
        }

        public static void OnBlockPlace()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            blocksPlaced++;

            // A cada 10 blocos colocados
            if (blocksPlaced >= 10)
            {
                // Criar um item temporário para representar o bloco
                var tempItem = new Item();
                tempItem.SetDefaults(ItemID.DirtBlock); // Item genérico
                RPGClassActionMapper.MapCraftingAction(CraftingAction.CraftBuilding, tempItem);
                blocksPlaced = 0;
            }
        }

        public static void OnCraft(Item item)
        {
            if (item == null || item.IsAir) return;

            // Determinar o tipo de crafting baseado no item
            CraftingAction action = DetermineCraftingAction(item);
            RPGClassActionMapper.MapCraftingAction(action, item);
        }

        /// <summary>
        /// Determina o tipo de ação de crafting baseado no item.
        /// </summary>
        /// <param name="item">Item craftado</param>
        /// <returns>Tipo de ação de crafting</returns>
        private static CraftingAction DetermineCraftingAction(Item item)
        {
            // Verificar se é arma
            if (item.damage > 0 && item.DamageType != DamageClass.Summon)
            {
                return CraftingAction.CraftWeapon;
            }
            
            // Verificar se é armadura
            if (item.defense > 0)
            {
                return CraftingAction.CraftArmor;
            }
            
            // Verificar se é poção
            if (item.buffType > 0 || item.healLife > 0 || item.healMana > 0)
            {
                return CraftingAction.CraftPotion;
            }
            
            // Verificar se é ferramenta
            if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
            {
                return CraftingAction.CraftTool;
            }
            
            // Verificar se é bloco/construção
            if (item.createTile >= 0 || item.createWall >= 0)
            {
                return CraftingAction.CraftBuilding;
            }
            
            // Fallback para construção
            return CraftingAction.CraftBuilding;
        }

        public static void OnHurt(Player player, int damage)
        {
            if (player?.active != true) return;

            // Guerreiro ganha XP por tankar dano
            RPGClassActionMapper.MapCombatAction(CombatAction.TakeDamage, damage, DamageClass.Melee);
        }

        public static void OnKillNPC(NPC npc)
        {
            if (npc == null) return;

            // Dar XP para todos os jogadores próximos
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;

                // Verificar distância
                float distance = Vector2.Distance(player.Center, npc.Center);
                if (distance > 1000f) continue; // 1000 pixels = ~62.5 blocos

                // XP base baseado na vida do NPC
                float baseXP = npc.lifeMax * 0.1f;

                // Bônus para bosses
                if (npc.boss)
                {
                    baseXP *= 10f;
                }

                // Determinar o tipo de dano baseado no último item usado
                DamageClass damageType = DamageClass.Melee; // Fallback
                if (player.HeldItem != null && !player.HeldItem.IsAir)
                {
                    damageType = player.HeldItem.DamageType;
                }

                // Dar XP para a classe correspondente ao tipo de dano
                RPGClassActionMapper.MapCombatAction(CombatAction.KillNPC, (int)baseXP, damageType);
            }
        }

        public static void OnUseItem(Item item)
        {
            if (item == null || item.IsAir) return;

            // Se é uma arma, dar XP baseado no tipo de dano
            if (item.damage > 0)
            {
                RPGClassActionMapper.MapCombatAction(CombatAction.HitNPC, item.damage, item.DamageType);
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
                100f
            );

            rpgPlayer.AddClassExperience("survivor", 5f);
        }
    }
}