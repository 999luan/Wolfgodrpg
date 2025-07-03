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
using Wolfgodrpg.Common.Systems; // Added for RPGClassActionMapper

namespace Wolfgodrpg.Common.GlobalClasses
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
                
                DebugLog.NPC("SetDefaults", $"NPC '{npc.FullName}' inicializado - Life: {originalLife}, Damage: {originalDamage}, Elite: {IsElite}");
            }

            var config = ModContent.GetInstance<ConfigSystem>();

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

            DebugLog.NPC("OnSpawn", $"NPC '{npc.FullName}' spawnou - Life: {npc.lifeMax}, Damage: {npc.damage}, Boss: {npc.boss}");

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
            
            DebugLog.NPC("OnSpawn", $"NPC '{npc.FullName}' finalizado - Life: {npc.lifeMax}, Damage: {npc.damage}, Elite: {IsElite}, HealthMult: {HealthMultiplier:F2}, DamageMult: {DamageMultiplier:F2}");
        }

        // === CALCULAR NÍVEL MÉDIO DOS JOGADORES ===
        private float GetAveragePlayerLevel(Vector2 position)
        {
            var nearbyPlayers = Main.player.Where(p => p.active && !p.dead && 
                Vector2.Distance(p.Center, position) < 2000f).ToList();
            
            if (!nearbyPlayers.Any()) 
            {
                DebugLog.NPC("GetAveragePlayerLevel", "Nenhum jogador próximo encontrado, usando nível 1");
                return 1f;
            }
            
            float totalLevel = 0f;
            foreach (var player in nearbyPlayers)
            {
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                float level = 0f;
                modPlayer.ClassLevels.TryGetValue("melee", out level);
                totalLevel += level;
                modPlayer.ClassLevels.TryGetValue("ranged", out level);
                totalLevel += level;
                modPlayer.ClassLevels.TryGetValue("magic", out level);
                totalLevel += level;
                modPlayer.ClassLevels.TryGetValue("summoner", out level);
                totalLevel += level;
                modPlayer.ClassLevels.TryGetValue("defense", out level);
                totalLevel += level;
            }
            
            float averageLevel = totalLevel / (nearbyPlayers.Count * 5f);
            DebugLog.NPC("GetAveragePlayerLevel", $"Nível médio dos jogadores próximos: {averageLevel:F2} (de {nearbyPlayers.Count} jogadores)");
            return averageLevel;
        }

        // === APLICAR SCALING ===
        private void ApplyProgressionScaling(NPC npc, float playerLevel)
        {
            float levelMultiplier = 1.0f + (playerLevel - 1) * 0.05f;
            float progressionMultiplier = GetWorldProgressionMultiplier();
            
            HealthMultiplier = levelMultiplier * progressionMultiplier;
            DamageMultiplier = Math.Min(levelMultiplier * progressionMultiplier, 2.0f);
            
            int oldLife = npc.lifeMax;
            int oldDamage = npc.damage;
            
            npc.lifeMax = (int)(npc.lifeMax * HealthMultiplier);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * DamageMultiplier);
            
            DebugLog.NPC("ApplyProgressionScaling", $"Scaling aplicado - LevelMult: {levelMultiplier:F2}, ProgressionMult: {progressionMultiplier:F2}, Health: {oldLife}->{npc.lifeMax}, Damage: {oldDamage}->{npc.damage}");
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
            
            DebugLog.NPC("GetWorldProgressionMultiplier", $"Multiplicador de progressão do mundo: {multiplier:F2}");
            return multiplier;
        }

        // === TORNAR ELITE ===
        private void MakeElite(NPC npc)
        {
            IsElite = true;
            HealthMultiplier *= ELITE_HEALTH_MULTIPLIER;
            DamageMultiplier *= ELITE_DAMAGE_MULTIPLIER;
            
            int oldLife = npc.lifeMax;
            int oldDamage = npc.damage;
            
            npc.lifeMax = (int)(npc.lifeMax * ELITE_HEALTH_MULTIPLIER);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * ELITE_DAMAGE_MULTIPLIER);
            npc.scale *= 1.1f;
            npc.color = Color.Gold;
            
            DebugLog.NPC("MakeElite", $"NPC '{npc.FullName}' tornado ELITE - Health: {oldLife}->{npc.lifeMax}, Damage: {oldDamage}->{npc.damage}");
        }

        // === SCALING DE BOSS ===
        private void ApplyBossScaling(NPC npc, float playerLevel)
        {
            float bossMultiplier = 1.0f + (playerLevel - 1) * 0.08f;
            float progressionMultiplier = GetWorldProgressionMultiplier();
            
            HealthMultiplier = bossMultiplier * progressionMultiplier * BOSS_HEALTH_MULTIPLIER;
            DamageMultiplier = Math.Min(bossMultiplier * progressionMultiplier * BOSS_DAMAGE_MULTIPLIER, 2.5f);
            
            int oldLife = npc.lifeMax;
            int oldDamage = npc.damage;
            
            npc.lifeMax = (int)(npc.lifeMax * HealthMultiplier);
            npc.life = npc.lifeMax;
            npc.damage = (int)(npc.damage * DamageMultiplier);
            
            DebugLog.NPC("ApplyBossScaling", $"Boss '{npc.FullName}' escalado - BossMult: {bossMultiplier:F2}, Health: {oldLife}->{npc.lifeMax}, Damage: {oldDamage}->{npc.damage}");
        }

        // === MODIFICAR DANO RECEBIDO ===
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            string className = RPGClassActionMapper.MapDamageTypeToClass(item.DamageType);

            // Apply damage bonus based on class level
            modifiers.SourceDamage *= 1f + (modPlayer.ClassLevels.TryGetValue(className, out var dmgLevel) ? dmgLevel : 0f) * 0.01f;
            RPGClassActionMapper.MapCombatAction(CombatAction.HitNPC, item.damage, item.DamageType);

            if (IsElite)
            {
                modifiers.FinalDamage *= 0.5f;
            }
            
            float levelDifference = GetNPCLevel(npc) - (modPlayer.ClassLevels.TryGetValue(className, out var diffLevel) ? diffLevel : 0f);
            float levelModifier = 1.0f + (levelDifference * 0.02f);
            levelModifier = Math.Max(0.5f, Math.Min(levelModifier, 1.5f));
            
            modifiers.FinalDamage *= levelModifier;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            var config = ModContent.GetInstance<ConfigSystem>();
            if (Main.player[projectile.owner].active)
            {
                var player = Main.player[projectile.owner];
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                string className = RPGClassActionMapper.MapDamageTypeToClass(projectile.DamageType);

                // Apply damage bonus based on class level
                modifiers.SourceDamage *= 1f + (modPlayer.ClassLevels.TryGetValue(className, out var projLevel) ? projLevel : 0f) * 0.01f;
                RPGClassActionMapper.MapCombatAction(CombatAction.HitNPC, projectile.damage, projectile.DamageType);
            }
        }

        // === MODIFICAR DANO CAUSADO ===
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            var modPlayer = target.GetModPlayer<RPGPlayer>();
            
            float damageReduction = 1.0f;
            float defenseLevel = modPlayer.ClassLevels.TryGetValue("defense", out var defLevel) ? defLevel : 0f;
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
            
            DebugLog.NPC("OnKill", $"NPC '{npc.FullName}' morto - BaseExp: {baseExp:F1}, Elite: {IsElite}, Boss: {npc.boss}");
            
            int playersGainedExp = 0;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && 
                    Vector2.Distance(player.Center, npc.Center) < 2000f)
                {
                    var modPlayer = player.GetModPlayer<RPGPlayer>();
                    float meleeExp = baseExp * 0.3f;
                    float rangedExp = baseExp * 0.2f;
                    float magicExp = baseExp * 0.2f;
                    float summonerExp = baseExp * 0.2f;
                    float defenseExp = baseExp * 0.1f;
                    
                    modPlayer.AddClassExperience("warrior", meleeExp);
                    modPlayer.AddClassExperience("archer", rangedExp);
                    modPlayer.AddClassExperience("mage", magicExp);
                    modPlayer.AddClassExperience("summoner", summonerExp);
                    modPlayer.AddClassExperience("warrior", defenseExp); // Defesa vai para guerreiro
                    
                    playersGainedExp++;
                    
                    DebugLog.Gameplay("NPC", "OnKill", $"Jogador '{player.name}' ganhou XP - Melee: {meleeExp:F1}, Ranged: {rangedExp:F1}, Magic: {magicExp:F1}, Summoner: {summonerExp:F1}, Defense: {defenseExp:F1}");
                }
            }
            
            DebugLog.NPC("OnKill", $"XP distribuído para {playersGainedExp} jogadores");

            if (IsElite)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Enchanted_Gold, 0f, 0f, 150, Color.Gold);
                }
                DebugLog.NPC("OnKill", "Efeitos visuais de elite aplicados");
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
            modPlayer.AddClassExperience("warrior", defenseXP);
            
            DebugLog.Gameplay("NPC", "OnHitPlayer", $"Jogador '{target.name}' atingido por '{npc.FullName}' - Dano: {hurtInfo.Damage}, XP de defesa: {defenseXP:F1}");

            // Reduzir sanidade em áreas perigosas
            if (npc.boss || npc.type == NPCID.EaterofSouls || npc.type == NPCID.Crimera)
            {
                float oldSanity = modPlayer.CurrentSanity;
                modPlayer.CurrentSanity = Math.Max(0f, modPlayer.CurrentSanity - 5f);
                
                if (modPlayer.CurrentSanity < oldSanity)
                {
                    DebugLog.Player("OnHitPlayer", $"Sanidade reduzida por NPC perigoso: {oldSanity:F1} -> {modPlayer.CurrentSanity:F1}");
                }
            }
        }

        public static bool CanDefeatBoss(string bossName, RPGPlayer rpgPlayer)
        {
            float totalLevel = 0f;
            totalLevel += rpgPlayer.ClassLevels.TryGetValue("melee", out var t1) ? t1 : 0f;
            totalLevel += rpgPlayer.ClassLevels.TryGetValue("ranged", out var t2) ? t2 : 0f;
            totalLevel += rpgPlayer.ClassLevels.TryGetValue("magic", out var t3) ? t3 : 0f;
            totalLevel += rpgPlayer.ClassLevels.TryGetValue("summoner", out var t4) ? t4 : 0f;
            
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
