using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq; // Adicionado para o ToList()
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Systems;
using Terraria.DataStructures; // Para PlayerDeathReason

namespace Wolfgodrpg.Common.Players
{
    public class RPGPlayer : ModPlayer
    {
        // Constantes de balanceamento
        private const float BASE_XP_NEEDED = 100f;
        private const float XP_MULTIPLIER = 1.1f;

        // Sistema de Classes
        public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        public HashSet<string> UnlockedAbilities = new HashSet<string>();

        // Vitals do Jogador (Fome, Sanidade, Stamina)
        public float CurrentHunger = 100f;
        public float MaxHunger = 100f;
        public float CurrentSanity = 100f;
        public float MaxSanity = 100f;
        public float CurrentStamina = 100f;
        public float MaxStamina = 100f;
        public int StaminaRegenDelay = 0;
        public int CombatTimer = 0; // Cronômetro para rastrear tempo em combate

        // Configuração
        private RPGConfig Config => ModContent.GetInstance<RPGConfig>();

        public override void Initialize()
        {
            InitializeClasses();
        }

        private void InitializeClasses()
        {
            foreach (var className in RPGClassDefinitions.ActionClasses.Keys)
            {
                if (!ClassLevels.ContainsKey(className))
                    ClassLevels[className] = 1f;
                if (!ClassExperience.ContainsKey(className))
                    ClassExperience[className] = 0f;
            }
        }

        public override void PostUpdate()
        {
            var totalStats = RPGCalculations.CalculateTotalStats(this);
            RPGCalculations.ApplyStatsToPlayer(Player, totalStats);
            ApplySpecialAbilities();
        }

        private void ApplySpecialAbilities()
        {
            Player.waterWalk = false;
            Player.lavaImmune = false;
            Player.wingsLogic = 0;
            Player.dash = 0;
            Player.jumpAgain = false;

            foreach (var ability in UnlockedAbilities)
            {
                switch (ability)
                {
                    case "movement_50": // Dash Duplo (Exemplo)
                        if (Player.dashType == 0) Player.dash = 1;
                        break;
                    case "jumping_50": // Pulo Duplo
                        Player.jumpAgain = true;
                        break;
                }
            }
        }

        // Novo método para consumir Stamina
        public bool ConsumeStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                CurrentStamina -= amount;
                StaminaRegenDelay = 30; // Atraso de meio segundo para regenerar
                return true;
            }
            return false;
        }

        public void GainClassExp(string className, float amount)
        {
            if (!ClassExperience.ContainsKey(className)) return;

            float finalAmount = amount * Config.ExpMultiplier;
            if (Main.hardMode) finalAmount *= 1.5f;
            if (NPC.downedMoonlord) finalAmount *= 2f;

            ClassExperience[className] += finalAmount;

            float oldLevel = ClassLevels[className];
            float newLevel = CalculateLevelFromXP(ClassExperience[className]);

            if (newLevel > oldLevel)
            {
                ClassLevels[className] = newLevel;
                Main.NewText($"Nível de {RPGClassDefinitions.ActionClasses[className].Name} aumentou para {newLevel:F0}!", Color.Yellow);
                CheckForNewAbilities(className, newLevel);
            }
        }

        private float CalculateLevelFromXP(float xp)
        {
            float level = 1;
            float xpForNextLevel = BASE_XP_NEEDED;
            while (xp >= xpForNextLevel)
            {
                xp -= xpForNextLevel;
                level++;
                xpForNextLevel *= XP_MULTIPLIER;
            }
            return level;
        }

        private void CheckForNewAbilities(string className, float level)
        {
            var milestones = RPGClassDefinitions.ActionClasses[className].Milestones;
            foreach (var milestone in milestones)
            {
                if (level >= milestone.Key)
                {
                    string abilityId = $"{className}_{milestone.Key}";
                    UnlockAbility(abilityId);
                }
            }
        }

        public void UnlockAbility(string ability)
        {
            if (UnlockedAbilities.Add(ability))
            {
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Ao acertar um NPC, reseta o cronômetro de combate
            CombatTimer = 0;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            // Ao ser atingido, reseta o cronômetro de combate
            CombatTimer = 0;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (!Config.KeepXPOnDeath)
            {
                foreach (var className in ClassExperience.Keys.ToList())
                {
                    ClassExperience[className] *= 0.9f; // Perde 10% do XP
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            var classLevels = new List<string>();
            var classExp = new List<string>();
            foreach (var kvp in ClassLevels)
            {
                classLevels.Add($"{kvp.Key}:{kvp.Value}");
                classExp.Add($"{kvp.Key}:{ClassExperience[kvp.Key]}");
            }
            tag["ClassLevels"] = classLevels;
            tag["ClassExperience"] = classExp;
            tag["UnlockedAbilities"] = new List<string>(UnlockedAbilities);
            tag["CurrentHunger"] = CurrentHunger;
            tag["CurrentSanity"] = CurrentSanity;
        }

        public override void LoadData(TagCompound tag)
        {
            var classLevels = tag.GetList<string>("ClassLevels");
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

            var classExp = tag.GetList<string>("ClassExperience");
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

            var unlockedAbilities = tag.GetList<string>("UnlockedAbilities");
            if (unlockedAbilities != null)
            {
                UnlockedAbilities = new HashSet<string>(unlockedAbilities);
            }

            CurrentHunger = tag.GetFloat("CurrentHunger");
            CurrentSanity = tag.GetFloat("CurrentSanity");
        }
    }
}
 