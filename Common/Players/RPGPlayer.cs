using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Systems;
using Terraria.DataStructures; // Para PlayerDeathReason
using Terraria.Localization;

namespace Wolfgodrpg.Common.Players
{
    public class RPGPlayer : ModPlayer
    {
        // === CONSTANTES DE BALANCEAMENTO ===
        private const int MAX_STAT_BONUS = 200;
        private const float MAX_DAMAGE_MULTIPLIER = 3.0f;
        private const float BASE_XP_NEEDED = 100f;
        private const float XP_MULTIPLIER = 1.1f;

        // === STATS BASE ===
        public float BaseHealth = 20f;
        public float BaseMana = 10f;
        public float BaseDefense = 0f;
        public float BaseRegeneration = 0f;
        public float BaseMovementSpeed = 0.8f;
        public float BaseDamageMultiplier = 0.5f;

        // === SISTEMA DE FOME E SANIDADE ===
        public float MaxHunger = 100f;
        public float CurrentHunger = 100f;
        public float MaxSanity = 100f;
        public float CurrentSanity = 100f;
        public float HungerDecayRate = 0.02f;
        public bool CanRegenerateHealth => CurrentHunger > 30f;

        // === SISTEMA DE CLASSES ===
        private Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        private Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        private HashSet<string> UnlockedAbilities = new HashSet<string>();
        private Dictionary<int, Dictionary<string, float>> EquippedItemStats = new Dictionary<int, Dictionary<string, float>>();
        private float ExpBonus = 0f;

        // === CONFIGURAÇÃO ===
        private RPGConfig Config => ModContent.GetInstance<RPGConfig>();

        private Dictionary<string, bool> MovementAbilities = new Dictionary<string, bool>();
        private int customDashCount = 0;
        private float customDashSpeedMultiplier = 1f;
        private bool dashNoStaminaCost = false;
        private bool hasDoubleJump = false;
        private bool hasTripleJump = false;
        private bool hasInfiniteJump = false;
        private int dashTimer = 0;
        private int currentDashCount = 0;

        public override void Initialize()
        {
            InitializeClasses();
            InitializeMovementAbilities();
            ResetStats();
        }

        private void InitializeClasses()
        {
            string[] classes = { "melee", "ranged", "magic", "summoner", "defense" };
            foreach (var className in classes)
            {
                if (!ClassLevels.ContainsKey(className))
                    ClassLevels[className] = 1f;
                if (!ClassExperience.ContainsKey(className))
                    ClassExperience[className] = 0f;
            }
        }

        private void InitializeMovementAbilities()
        {
            MovementAbilities["water_walking"] = false;
            MovementAbilities["lava_immunity"] = false;
            MovementAbilities["flight"] = false;
            MovementAbilities["dash"] = false;
        }

        private void ResetStats()
        {
            // Resetar status base
            BaseHealth = 20f;
            BaseMana = 10f;
            BaseDefense = 0f;
            BaseRegeneration = 0f;
            BaseMovementSpeed = 0.8f;
            BaseDamageMultiplier = 0.5f;

            // Resetar fome e sanidade
            MaxHunger = 100f;
            CurrentHunger = MaxHunger;
            MaxSanity = 100f;
            CurrentSanity = MaxSanity;
            HungerDecayRate = 0.02f;

            // Limpar status de itens equipados
            EquippedItemStats.Clear();
            ExpBonus = 0f;
        }

        public override void PostUpdate()
        {
            if (!Config.EnableHunger && !Config.EnableSanity) return;

            // Atualizar fome
            if (Config.EnableHunger)
            {
                CurrentHunger = Math.Max(0f, CurrentHunger - HungerDecayRate * Config.HungerRate);

                // Efeitos da fome
                if (CurrentHunger < 30f)
                {
                    // Fome crítica: reduz regeneração de vida
                    Player.lifeRegen -= 2;
                }
                else if (CurrentHunger < 50f)
                {
                    // Fome moderada: reduz velocidade
                    Player.moveSpeed *= 0.8f;
                }
            }

            // Atualizar sanidade
            if (Config.EnableSanity)
            {
                UpdateSanity();
            }

            // Aplicar status de itens equipados
            foreach (var stats in GetEquippedItemStats())
            {
                ApplyStats(stats);
            }

            // Aplicar habilidades desbloqueadas
            ApplyUnlockedAbilities();
        }

        private void UpdateSanity()
        {
            // Fatores que afetam a sanidade
            if (Player.ZoneCorrupt || Player.ZoneCrimson)
            {
                CurrentSanity = Math.Max(0f, CurrentSanity - 0.1f * Config.SanityRate);
            }
            else if (Player.ZoneHallow)
            {
                CurrentSanity = Math.Min(MaxSanity, CurrentSanity + 0.05f);
            }

            if (Player.wet && !Player.honeyWet)
            {
                CurrentSanity = Math.Max(0f, CurrentSanity - 0.05f * Config.SanityRate);
            }

            // Efeitos da sanidade
            if (CurrentSanity < 30f)
            {
                // Sanidade crítica: alucinações e dano periódico
                if (Main.rand.NextBool(300))
                {
                    Player.AddBuff(Terraria.ID.BuffID.Confused, 300);
                }
                if (Main.rand.NextBool(600))
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason("Sua mente se despedaça..."), 10, 0);
                }
            }
            else if (CurrentSanity < 50f)
            {
                // Sanidade baixa: reduz precisão e velocidade
                Player.moveSpeed *= 0.9f;
                Player.GetDamage(DamageClass.Generic) *= 0.9f;
            }
        }

        private void ApplyStats(Dictionary<string, float> stats)
        {
            if (stats == null) return;

            foreach (var stat in stats)
            {
                switch (stat.Key)
                {
                    case "damage":
                        Player.GetDamage(DamageClass.Generic) *= 1f + stat.Value;
                        break;
                    case "defense":
                        Player.statDefense += (int)(Player.statDefense * stat.Value);
                        break;
                    case "speed":
                        Player.moveSpeed *= 1f + stat.Value;
                        break;
                    case "crit":
                        Player.GetCritChance(DamageClass.Generic) += stat.Value;
                        break;
                    case "knockback":
                        Player.GetKnockback(DamageClass.Generic) *= 1f + stat.Value;
                        break;
                    case "mana":
                        Player.statManaMax2 += (int)stat.Value;
                        break;
                    case "life":
                        Player.statLifeMax2 += (int)stat.Value;
                        break;
                }
            }
        }

        private void ApplyUnlockedAbilities()
        {
            // Reset all custom movement abilities before applying
            customDashCount = 0;
            customDashSpeedMultiplier = 1f;
            dashNoStaminaCost = false;
            hasDoubleJump = false;
            hasTripleJump = false;
            hasInfiniteJump = false;

            foreach (var ability in UnlockedAbilities)
            {
                switch (ability)
                {
                    case "water_walking":
                        MovementAbilities["water_walking"] = true;
                        break;
                    case "lava_immunity":
                        MovementAbilities["lava_immunity"] = true;
                        break;
                    case "flight":
                        MovementAbilities["flight"] = true;
                        break;
                    case "dash": // Base dash from vanilla
                        MovementAbilities["dash"] = true;
                        break;
                    // Movement Class Milestones
                    case "movement_25": // Dash mais rápido
                        customDashSpeedMultiplier = 1.2f; // Example value
                        break;
                    case "movement_50": // Dash duplo
                        // This milestone now means the player has enough level for 2 dashes (1 base + 1 extra)
                        break;
                    case "movement_75": // Dash não gasta stamina (assuming a stamina system)
                        dashNoStaminaCost = true;
                        break;
                    case "movement_100": // Dash infinito
                        customDashCount = int.MaxValue; // Effectively infinite
                        break;
                    // Jumping Class Milestones
                    case "jumping_25": // Pulo mais alto (handled by jumpHeight stat bonus)
                        break;
                    case "jumping_50": // Pulo duplo
                        hasDoubleJump = true;
                        break;
                    case "jumping_75": // Pulo triplo
                        hasTripleJump = true;
                        break;
                    case "jumping_100": // Pulo infinito
                        hasInfiniteJump = true;
                        break;
                }
            }
        }

        private void UpdateMovementAbilities()
        {
            // Apply dash speed
            Player.dashSpeed *= customDashSpeedMultiplier;

            // Apply custom dashes (needs more complex logic for multiple dashes)
            // For now, just enable vanilla dash if any custom dash is available
            if (customDashCount > 0)
            {
                Player.dash = 1; // Enable vanilla dash
            }

            // Apply jump abilities
            if (hasDoubleJump)
            {
                Player.jumpAgain = true; // Allows one extra jump
            }
            if (hasTripleJump)
            {
                Player.jumpAgain = true; // Allows multiple extra jumps (needs more complex logic)
                Player.extraJumps = 2; // Example: allows 2 extra jumps
            }
            if (hasInfiniteJump)
            {
                Player.jumpAgain = true; // Allows infinite jumps
                Player.extraJumps = int.MaxValue; // Effectively infinite
            }

            // Stamina cost for dash (if a stamina system is implemented)
            // if (dashNoStaminaCost) { /* Prevent stamina consumption for dash */ }
        }

        public void GainClassExp(string className, float amount)
        {
            if (!ClassExperience.ContainsKey(className)) return;

            // Aplicar multiplicadores de XP
            float finalAmount = amount;
            finalAmount *= Config.ExpMultiplier;
            finalAmount *= (1f + ExpBonus);

            // Multiplicador de progressão
            if (Main.hardMode) finalAmount *= 1.5f;
            if (NPC.downedMoonlord) finalAmount *= 2f;

            // Adicionar XP
            ClassExperience[className] += finalAmount;

            // Calcular novo nível
            float oldLevel = ClassLevels[className];
            float newLevel = 1f;
            float totalXpNeeded = BASE_XP_NEEDED;
            while (ClassExperience[className] >= totalXpNeeded)
            {
                newLevel++;
                totalXpNeeded *= XP_MULTIPLIER;
            }

            // Atualizar nível se aumentou
            if (newLevel > oldLevel)
            {
                ClassLevels[className] = newLevel;
                Main.NewText($"Nível de {className} aumentou para {newLevel}!", Color.Yellow);
            }
        }

        public void UnlockAbility(string ability)
        {
            if (!UnlockedAbilities.Contains(ability))
            {
                UnlockedAbilities.Add(ability);
                Main.NewText($"Nova habilidade desbloqueada: {ability}!", Color.LightBlue);
            }
        }

        public bool HasUnlockedAbility(string ability)
        {
            return UnlockedAbilities.Contains(ability);
        }

        public float GetClassLevel(string className)
        {
            return ClassLevels.ContainsKey(className) ? ClassLevels[className] : 1f;
        }

        public float GetExpBonus()
        {
            return ExpBonus;
        }

        public void SetExpBonus(float bonus)
        {
            ExpBonus = Math.Max(0f, bonus);
        }

        public void AddItemStats(Item item, Dictionary<string, float> stats)
        {
            if (item == null || stats == null) return;

            if (!EquippedItemStats.ContainsKey(item.type))
            {
                EquippedItemStats[item.type] = new Dictionary<string, float>();
            }

            foreach (var stat in stats)
            {
                EquippedItemStats[item.type][stat.Key] = stat.Value;
            }
        }

        public void RemoveItemStats(Item item)
        {
            if (item == null) return;

            if (EquippedItemStats.ContainsKey(item.type))
            {
                EquippedItemStats.Remove(item.type);
            }
        }

        public IEnumerable<Dictionary<string, float>> GetEquippedItemStats()
        {
            return EquippedItemStats.Values;
        }

        public override void OnRespawn()
        {
            // Restaurar fome e sanidade ao respawnar
            if (Config.EnableHunger)
                CurrentHunger = MaxHunger;
            if (Config.EnableSanity)
                CurrentSanity = MaxSanity;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            // Se configurado para não perder XP na morte, não fazer nada
            if (!Config.KeepXPOnDeath)
            {
                // Perder uma porcentagem do XP atual
                foreach (var className in ClassExperience.Keys)
                {
                    ClassExperience[className] *= 0.9f; // Perder 10% do XP
                }
            }

            // Chamar a implementação base
            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void SaveData(TagCompound tag)
        {
            if (tag == null) return;

            // Salvar níveis e experiência das classes
            var classLevels = new List<string>();
            var classExp = new List<string>();
            foreach (var kvp in ClassLevels)
            {
                classLevels.Add($"{kvp.Key}:{kvp.Value}");
                classExp.Add($"{kvp.Key}:{ClassExperience[kvp.Key]}");
            }
            tag["ClassLevels"] = classLevels;
            tag["ClassExperience"] = classExp;

            // Salvar habilidades desbloqueadas
            tag["UnlockedAbilities"] = new List<string>(UnlockedAbilities);

            // Salvar status base
            tag["BaseHealth"] = BaseHealth;
            tag["BaseMana"] = BaseMana;
            tag["BaseDefense"] = BaseDefense;
            tag["BaseRegeneration"] = BaseRegeneration;
            tag["BaseMovementSpeed"] = BaseMovementSpeed;
            tag["BaseDamageMultiplier"] = BaseDamageMultiplier;

            // Salvar fome e sanidade
            tag["CurrentHunger"] = CurrentHunger;
            tag["CurrentSanity"] = CurrentSanity;

            // Salvar bônus de XP
            tag["ExpBonus"] = ExpBonus;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag == null) return;

            // Carregar níveis e experiência das classes
            var classLevels = tag.GetList<string>("ClassLevels");
            var classExp = tag.GetList<string>("ClassExperience");
            if (classLevels != null)
            {
                foreach (var entry in classLevels)
                {
                    var parts = entry.Split(':');
                    if (parts.Length == 2 && float.TryParse(parts[1], out float level))
                    {
                        ClassLevels[parts[0]] = level;
                    }
                }
            }
            if (classExp != null)
            {
                foreach (var entry in classExp)
                {
                    var parts = entry.Split(':');
                    if (parts.Length == 2 && float.TryParse(parts[1], out float exp))
                    {
                        ClassExperience[parts[0]] = exp;
                    }
                }
            }

            // Carregar habilidades desbloqueadas
            var unlockedAbilities = tag.GetList<string>("UnlockedAbilities");
            if (unlockedAbilities != null)
            {
                UnlockedAbilities = new HashSet<string>(unlockedAbilities);
            }

            // Carregar status base
            BaseHealth = tag.Get<float>("BaseHealth");
            BaseMana = tag.Get<float>("BaseMana");
            BaseDefense = tag.Get<float>("BaseDefense");
            BaseRegeneration = tag.Get<float>("BaseRegeneration");
            BaseMovementSpeed = tag.Get<float>("BaseMovementSpeed");
            BaseDamageMultiplier = tag.Get<float>("BaseDamageMultiplier");

            // Carregar fome e sanidade
            CurrentHunger = tag.Get<float>("CurrentHunger");
            CurrentSanity = tag.Get<float>("CurrentSanity");

            // Carregar bônus de XP
            ExpBonus = tag.Get<float>("ExpBonus");
        }

        public override void PreUpdate()
        {
            base.PreUpdate();

            // Aplicar habilidades de movimento
            UpdateMovementAbilities();

            if (MovementAbilities["water_walking"])
            {
                Player.waterWalk = true;
            }

            if (MovementAbilities["lava_immunity"])
            {
                Player.lavaImmune = true;
            }

            if (MovementAbabilities["flight"])
            {
                Player.wingsLogic = 1;
            }

            if (MovementAbilities["dash"])
            {
                Player.dash = 1;
            }
        }
    }
} 