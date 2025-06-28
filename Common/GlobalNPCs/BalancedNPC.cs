using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.GlobalNPCs
{
    public class BalancedNPC : GlobalNPC
    {
        // === PROPRIEDADES ===
        public bool IsElite { get; private set; }
        public float HealthMultiplier { get; set; } = 1.0f;
        public float DamageMultiplier { get; set; } = 1.0f;
        
        // === CONSTANTES ===
        private const float ELITE_SPAWN_CHANCE = 0.05f;
        private const float ELITE_HEALTH_MULTIPLIER = 2.0f;
        private const float ELITE_DAMAGE_MULTIPLIER = 1.5f;
        private const float ELITE_DEFENSE_MULTIPLIER = 1.5f;
        private const float ELITE_KNOCKBACK_MULTIPLIER = 1.2f;
        private const float BOSS_HEALTH_MULTIPLIER = 1.5f;
        private const float BOSS_DAMAGE_MULTIPLIER = 1.3f;

        public override bool InstancePerEntity => true;

        private float originalLife;
        private float originalDamage;
        private bool isInitialized = false;

        public override void SetDefaults(NPC npc)
        {
            if (!isInitialized)
            {
                originalLife = npc.lifeMax;
                originalDamage = npc.damage;
                IsElite = Main.rand.NextFloat() < ELITE_SPAWN_CHANCE;
                isInitialized = true;
            }

            var config = ModContent.GetInstance<RPGConfig>();

            // Ajustar vida e dano baseado na configuração
            npc.lifeMax = (int)(originalLife * config.MonsterHealthMultiplier);
            npc.damage = (int)(originalDamage * config.MonsterDamageMultiplier);

            // Ajustar baseado na progressão do jogo
            if (Main.hardMode)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                npc.damage = (int)(npc.damage * 1.5f);
            }
            if (NPC.downedMoonlord)
            {
                npc.lifeMax = (int)(npc.lifeMax * 2f);
                npc.damage = (int)(npc.damage * 2f);
            }

            if (IsElite)
            {
                npc.lifeMax = (int)(npc.lifeMax * ELITE_HEALTH_MULTIPLIER);
                npc.defense = (int)(npc.defense * ELITE_DEFENSE_MULTIPLIER);
                npc.knockBackResist *= ELITE_KNOCKBACK_MULTIPLIER;
            }
        }

        // === SPAWN DO NPC ===
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.friendly || npc.lifeMax <= 5 || npc.townNPC) return;

            float averagePlayerLevel = GetAveragePlayerLevel(npc.Center);
            ApplyProgressionScaling(npc, averagePlayerLevel);
            
            if (!npc.boss && Main.rand.NextFloat() < ELITE_SPAWN_CHANCE)
            {
                MakeElite(npc);
            }
            
            if (npc.boss)
            {
                ApplyBossScaling(npc, averagePlayerLevel);
            }
        }

        // === CALCULAR NÍVEL MÉDIO DOS JOGADORES ===
        private float GetAveragePlayerLevel(Vector2 position)
        {
            var nearbyPlayers = Main.player.Where(p => p.active && !p.dead && 
                Vector2.Distance(p.Center, position) < 2000f).ToList();
            
            if (!nearbyPlayers.Any()) return 1f;
            
            float totalLevel = 0f;
            foreach (var player in nearbyPlayers)
            {
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                totalLevel += modPlayer.GetClassLevel("melee");
                totalLevel += modPlayer.GetClassLevel("ranged");
                totalLevel += modPlayer.GetClassLevel("magic");
                totalLevel += modPlayer.GetClassLevel("summoner");
                totalLevel += modPlayer.GetClassLevel("defense");
            }
            
            return totalLevel / (nearbyPlayers.Count * 5f);
        }

        // === APLICAR SCALING ===
        private void ApplyProgressionScaling(NPC npc, float playerLevel)
        {
            float levelMultiplier = 1.0f + (playerLevel - 1) * 0.05f;
            float progressionMultiplier = GetWorldProgressionMultiplier();
            
            HealthMultiplier = levelMultiplier * progressionMultiplier;
            DamageMultiplier = Math.Min(levelMultiplier * progressionMultiplier, 2.0f);
            
            npc.lifeMax = (int)(npc.lifeMax * HealthMultiplier);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * DamageMultiplier);
        }

        // === MULTIPLICADOR DE PROGRESSÃO ===
        private float GetWorldProgressionMultiplier()
        {
            float multiplier = 1.0f;
            
            if (Main.hardMode) multiplier *= 1.5f;
            if (NPC.downedMoonlord) multiplier *= 2.0f;
            else if (NPC.downedGolemBoss) multiplier *= 1.8f;
            else if (NPC.downedPlantBoss) multiplier *= 1.6f;
            else if (NPC.downedMechBossAny) multiplier *= 1.4f;
            
            return multiplier;
        }

        // === TORNAR ELITE ===
        private void MakeElite(NPC npc)
        {
            IsElite = true;
            HealthMultiplier *= ELITE_HEALTH_MULTIPLIER;
            DamageMultiplier *= ELITE_DAMAGE_MULTIPLIER;
            
            npc.lifeMax = (int)(npc.lifeMax * ELITE_HEALTH_MULTIPLIER);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * ELITE_DAMAGE_MULTIPLIER);
            npc.scale *= 1.1f;
            npc.color = Color.Gold;
        }

        // === SCALING DE BOSS ===
        private void ApplyBossScaling(NPC npc, float playerLevel)
        {
            float bossMultiplier = 1.0f + (playerLevel - 1) * 0.08f;
            float progressionMultiplier = GetWorldProgressionMultiplier();
            
            HealthMultiplier = bossMultiplier * progressionMultiplier * BOSS_HEALTH_MULTIPLIER;
            DamageMultiplier = Math.Min(bossMultiplier * progressionMultiplier * BOSS_DAMAGE_MULTIPLIER, 2.5f);
            
            npc.lifeMax = (int)(npc.lifeMax * HealthMultiplier);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * DamageMultiplier);
        }

        // === MODIFICAR DANO RECEBIDO ===
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            string damageClass = item.DamageType.ToString().ToLower();

            // Apply damage bonus based on class level
            modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel(damageClass) * 0.01f);
            modPlayer.GainClassExp(damageClass, item.damage * 0.1f);

            if (IsElite)
            {
                modifiers.FinalDamage *= 0.5f;
            }
            
            float levelDifference = GetNPCLevel(npc) - modPlayer.GetClassLevel(damageClass);
            float levelModifier = 1.0f + (levelDifference * 0.02f);
            levelModifier = Math.Max(0.5f, Math.Min(levelModifier, 1.5f));
            
            modifiers.FinalDamage *= levelModifier;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].active)
            {
                var player = Main.player[projectile.owner];
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                string damageClass = projectile.DamageType.ToString().ToLower();

                // Apply damage bonus based on class level
                modifiers.SourceDamage *= 1f + (modPlayer.GetClassLevel(damageClass) * 0.01f);
                modPlayer.GainClassExp(damageClass, projectile.damage * 0.1f);
            }
        }

        // === MODIFICAR DANO CAUSADO ===
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            var modPlayer = target.GetModPlayer<RPGPlayer>();
            
            float damageReduction = 1.0f;
            float defenseLevel = modPlayer.GetClassLevel("defense");
            damageReduction -= defenseLevel * 0.01f;
            
            damageReduction = Math.Max(0.3f, Math.Min(damageReduction, 1.0f));
            modifiers.FinalDamage *= damageReduction;

            if (IsElite)
            {
                modifiers.FinalDamage *= ELITE_DAMAGE_MULTIPLIER;
            }

            if (Main.hardMode)
            {
                modifiers.FinalDamage *= 1.2f;
            }

            if (NPC.downedMoonlord)
                modifiers.FinalDamage *= 1.5f;
            else if (NPC.downedGolemBoss)
                modifiers.FinalDamage *= 1.4f;
            else if (NPC.downedPlantBoss)
                modifiers.FinalDamage *= 1.3f;
            else if (NPC.downedMechBossAny)
                modifiers.FinalDamage *= 1.2f;
        }

        // === CALCULAR NÍVEL DO NPC ===
        private float GetNPCLevel(NPC npc)
        {
            float level = (npc.lifeMax / 100f) + (npc.damage / 20f);
            return Math.Max(1f, level);
        }

        // === DISTRIBUIR EXPERIÊNCIA ===
        public override void OnKill(NPC npc)
        {
            if (npc.friendly || npc.lifeMax <= 5) return;
            
            float baseExp = npc.lifeMax * 0.1f + npc.damage * 0.5f;
            
            if (IsElite) baseExp *= 1.5f;
            if (npc.boss) baseExp *= 3.0f;
            
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && 
                    Vector2.Distance(player.Center, npc.Center) < 2000f)
                {
                    var modPlayer = player.GetModPlayer<RPGPlayer>();
                    modPlayer.GainClassExp("melee", baseExp * 0.3f);
                    modPlayer.GainClassExp("ranged", baseExp * 0.2f);
                    modPlayer.GainClassExp("magic", baseExp * 0.2f);
                    modPlayer.GainClassExp("summoner", baseExp * 0.2f);
                    modPlayer.GainClassExp("defense", baseExp * 0.1f);
                }
            }

            if (IsElite)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Enchanted_Gold, 0f, 0f, 150, Color.Gold);
                }
            }

            RPGActionSystem.OnKillNPC(npc);
        }

        // === EFEITOS VISUAIS ===
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsElite && Main.rand.NextBool(3))
            {
                Vector2 position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height);
                Dust dust = Dust.NewDustDirect(position, 0, 0, DustID.GoldCoin, 0f, 0f, 100, default(Color), 0.8f);
                dust.velocity *= 0.3f;
                dust.noGravity = true;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (IsElite)
            {
                drawColor = Color.Lerp(drawColor, Color.Gold, 0.25f);
                
                if (Main.rand.NextBool(5))
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Enchanted_Gold, 0f, -2f, 0, Color.Gold, 0.5f);
                }
            }
        }

        public override void AI(NPC npc)
        {
            if (npc.boss)
            {
                // Efeitos visuais para bosses
                if (Main.rand.NextBool(30))
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Enchanted_Gold, 0f, 0f, 150, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            var modPlayer = target.GetModPlayer<RPGPlayer>();

            // Dar XP de defesa quando recebe dano
            float defenseXP = hurtInfo.Damage * 0.2f;
            modPlayer.GainClassExp("defense", defenseXP);

            // Reduzir sanidade em áreas perigosas
            if (npc.boss || npc.type == NPCID.EaterofSouls || npc.type == NPCID.Crimera)
            {
                modPlayer.CurrentSanity = Math.Max(0f, modPlayer.CurrentSanity - 5f);
            }
        }

        public static bool CanDefeatBoss(string bossName, RPGPlayer rpgPlayer)
        {
            float totalLevel = rpgPlayer.GetClassLevel("melee") + rpgPlayer.GetClassLevel("ranged") + 
                             rpgPlayer.GetClassLevel("magic") + rpgPlayer.GetClassLevel("summoner");
            
            switch (bossName.ToLower())
            {
                case "eye":
                    return totalLevel >= 20;
                case "eater":
                case "brain":
                    return totalLevel >= 40;
                case "skeletron":
                    return totalLevel >= 60;
                case "wall":
                    return totalLevel >= 80;
                case "mechs":
                    return totalLevel >= 120;
                case "plantera":
                    return totalLevel >= 160;
                case "golem":
                    return totalLevel >= 200;
                case "cultist":
                    return totalLevel >= 240;
                case "moonlord":
                    return totalLevel >= 300;
                default:
                    return true;
            }
        }
    }
} 