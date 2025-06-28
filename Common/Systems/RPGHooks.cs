using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Wolfgodrpg.Common.Players;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGHooks : ModSystem
    {
        public override void Load()
        {
            // Não precisa registrar hooks aqui, vamos usar os do ModPlayer
        }

        public override void Unload()
        {
            // Não precisa desregistrar hooks
        }
    }

    public class RPGPlayerHooks : ModPlayer
    {
        private float lastHealth;
        private int jumpCount = 0;

        public override void Initialize()
        {
            lastHealth = Player.statLife;
        }

        public override void PreUpdate()
        {
            // Verificar mineração e construção
            if (Player.tileTargetX != -1 && Player.tileTargetY != -1)
            {
                if (Player.controlUseItem)
                {
                    // Check for gathering XP
                    Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
                    if (tile != null && tile.HasTile && RPGActionSystem.ResourceTileIDs.Contains(tile.TileType))
                    {
                        var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
                        rpgPlayer.GainClassExp("gathering", 0.1f); // Small amount of XP per resource mined
                    }
                    RPGActionSystem.OnBlockMine();
                }
                if (Player.controlUseTile)
                {
                    RPGActionSystem.OnBlockPlace();
                }
            }

            // Verificar uso de itens
            Item item = Player.HeldItem;
            if (item != null && !item.IsAir)
            {
                // Se for comida
                if (item.consumable && item.buffType > 0 && 
                    Main.buffNoTimeDisplay[item.buffType] && !item.potion)
                {
                    RPGActionSystem.OnEatFood(item);
                }
                // Se for poção
                else if (item.potion)
                {
                    RPGActionSystem.OnUseItem(item);
                }
                // Se for arma
                else if (item.damage > 0 && Player.itemAnimation > 0)
                {
                    RPGActionSystem.OnUseItem(item);
                }
            }

            // Verificar dano tomado
            if (Player.statLife < lastHealth)
            {
                int damageTaken = (int)(lastHealth - Player.statLife);
                RPGActionSystem.OnHurt(Player, damageTaken);
            }
            lastHealth = Player.statLife;

            // Verificar pulos para XP de Jumping
            if (Player.justJumped && Player.velocity.Y < 0) // Detect a jump
            {
                jumpCount++;
                if (jumpCount >= 100) // Gain XP every 100 jumps
                {
                    var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
                    rpgPlayer.GainClassExp("jumping", 5f); // 5 XP per 100 jumps
                    jumpCount = 0;
                }
            }
        }

        public override void PostBuyItem(Item item, int price, int shopId)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = price * 0.0001f; // Example: 0.01% of item price as XP
            xpAmount = System.Math.Max(0.1f, xpAmount); // Minimum 0.1 XP
            rpgPlayer.GainClassExp("merchant", xpAmount);
        }

        public override void PostSellItem(Item item, int price, int shopId)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = price * 0.0001f; // Example: 0.01% of item price as XP
            xpAmount = System.Math.Max(0.1f, xpAmount); // Minimum 0.1 XP
            rpgPlayer.GainClassExp("merchant", xpAmount);
        }

        public override void OnEnterWorld()
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            Main.NewText("=== WOLF GOD RPG ===", Color.Gold);
            Main.NewText("Pressione 'M' para abrir o menu RPG!", Color.Yellow);
            Main.NewText("Pressione 'R' para ver stats rápidos!", Color.Yellow);
            Main.NewText($"Fome: {rpgPlayer.CurrentHunger:F0}% | Sanidade: {rpgPlayer.CurrentSanity:F0}%", Color.Orange);
        }
    }

    public class RPGGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private Dictionary<string, float> randomStats;
        private int rarityLevel;

        public override void SetDefaults(Item item)
        {
            // Resetar status aleatórios
            randomStats = null;
            rarityLevel = 0;
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            // Gerar status aleatórios baseados na raridade
            if (item.damage > 0 || item.defense > 0)
            {
                GenerateRandomStats(item);
            }
        }

        private void GenerateRandomStats(Item item)
        {
            var config = ModContent.GetInstance<RPGConfig>();
            if (!config.RandomStats) return;

            randomStats = new Dictionary<string, float>();
            rarityLevel = item.rare + 1;

            // Número de status baseado na raridade
            int numStats = item.rare switch
            {
                <= 0 => 0, // Comum: sem status
                <= 2 => 1, // Incomum: 1 status
                <= 4 => 2, // Raro: 2 status
                <= 6 => 3, // Muito Raro: 3 status
                _ => 4     // Lendário: 4 status
            };

            // Lista de possíveis status
            var possibleStats = new List<(string name, float min, float max)>
            {
                ("damage", 0.01f, 0.05f),
                ("defense", 0.01f, 0.05f),
                ("speed", 0.01f, 0.05f),
                ("crit", 1f, 5f),
                ("knockback", 0.01f, 0.05f),
                ("mana", 5f, 20f),
                ("life", 5f, 20f)
            };

            // Gerar status aleatórios
            for (int i = 0; i < numStats; i++)
            {
                if (possibleStats.Count == 0) break;

                int index = Main.rand.Next(possibleStats.Count);
                var stat = possibleStats[index];
                possibleStats.RemoveAt(index);

                float value = Main.rand.NextFloat(stat.min, stat.max) * config.ItemStatMultiplier;
                randomStats[stat.name] = value;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (randomStats != null && randomStats.Count > 0)
            {
                // Adicionar status aleatórios ao tooltip
                foreach (var stat in randomStats)
                {
                    string text = stat.Key switch
                    {
                        "damage" => $"+{(stat.Value * 100):F0}% Dano",
                        "defense" => $"+{(stat.Value * 100):F0}% Defesa",
                        "speed" => $"+{(stat.Value * 100):F0}% Velocidade",
                        "crit" => $"+{stat.Value:F0} Chance de Crítico",
                        "knockback" => $"+{(stat.Value * 100):F0}% Empurrão",
                        "mana" => $"+{stat.Value:F0} Mana",
                        "life" => $"+{stat.Value:F0} Vida",
                        _ => $"+{stat.Value:F0} {stat.Key}"
                    };

                    var line = new TooltipLine(Mod, $"RPGStat_{stat.Key}", text)
                    {
                        IsModifier = true
                    };
                    tooltips.Add(line);
                }
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            if (randomStats != null && randomStats.Count > 0)
            {
                // Aplicar status ao equipar o item
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                modPlayer.AddItemStats(item, randomStats);
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (randomStats != null && randomStats.Count > 0 && item.favorited)
            {
                // Aplicar status se o item estiver favoritado (como amuleto)
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                modPlayer.AddItemStats(item, randomStats);
            }
        }
    }

    public class RPGGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void OnKill(NPC npc)
        {
            if (npc.boss)
            {
                // Dar XP extra para todas as classes ao matar um boss
                foreach (var player in Main.player)
                {
                    if (player.active && !player.dead)
                    {
                        var modPlayer = player.GetModPlayer<RPGPlayer>();
                        modPlayer.GainClassExp("melee", 1000f);
                        modPlayer.GainClassExp("ranged", 1000f);
                        modPlayer.GainClassExp("magic", 1000f);
                        modPlayer.GainClassExp("summoner", 1000f);
                        modPlayer.GainClassExp("defense", 1000f);
                    }
                }
            }
            else
            {
                // XP normal para classes de combate
                foreach (var player in Main.player)
                {
                    if (player.active && !player.dead)
                    {
                        var modPlayer = player.GetModPlayer<RPGPlayer>();
                        float xp = npc.lifeMax * 0.1f;
                        modPlayer.GainClassExp("melee", xp);
                        modPlayer.GainClassExp("ranged", xp);
                        modPlayer.GainClassExp("magic", xp);
                        modPlayer.GainClassExp("summoner", xp);
                        modPlayer.GainClassExp("defense", xp * 0.5f);
                    }
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();

            // Aplicar bônus de dano baseado no nível da classe
            if (item.DamageType == DamageClass.Melee)
            {
                modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("melee") * 0.01f);
                modPlayer.GainClassExp("melee", item.damage * 0.1f);
            }
            else if (item.DamageType == DamageClass.Ranged)
            {
                modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("ranged") * 0.01f);
                modPlayer.GainClassExp("ranged", item.damage * 0.1f);
            }
            else if (item.DamageType == DamageClass.Magic)
            {
                modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("magic") * 0.01f);
                modPlayer.GainClassExp("magic", item.damage * 0.1f);
            }
            else if (item.DamageType == DamageClass.Summon)
            {
                modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("summoner") * 0.01f);
                modPlayer.GainClassExp("summoner", item.damage * 0.1f);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].active)
            {
                var player = Main.player[projectile.owner];
                var modPlayer = player.GetModPlayer<RPGPlayer>();

                // Aplicar bônus de dano baseado no nível da classe
                if (projectile.DamageType == DamageClass.Melee)
                {
                    modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("melee") * 0.01f);
                    modPlayer.GainClassExp("melee", projectile.damage * 0.1f);
                }
                else if (projectile.DamageType == DamageClass.Ranged)
                {
                    modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("ranged") * 0.01f);
                    modPlayer.GainClassExp("ranged", projectile.damage * 0.1f);
                }
                else if (projectile.DamageType == DamageClass.Magic)
                {
                    modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("magic") * 0.01f);
                    modPlayer.GainClassExp("magic", projectile.damage * 0.1f);
                }
                else if (projectile.DamageType == DamageClass.Summon)
                {
                    modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel("summoner") * 0.01f);
                    modPlayer.GainClassExp("summoner", projectile.damage * 0.1f);
                }
            }
        }
    }
} 