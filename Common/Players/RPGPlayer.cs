using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using Terraria.Audio;
using Terraria.GameInput;

namespace Wolfgodrpg.Common.Players
{
    public class RPGPlayer : ModPlayer
    {
        // === SISTEMA DE DASH ===
        public int DashCooldown { get; set; }
        public int DashesUsed { get; set; }
        public int DashResetTimer { get; set; }
        public float DashSpeed { get; set; } = 12f;
        public int DashDuration { get; set; } = 15;
        public int DashInvincibilityFrames { get; set; } = 15;
        public int MaxDashes { get; set; } = 1;

        // === SISTEMA DE CLASSES ===
        public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        public HashSet<ClassAbility> UnlockedAbilities = new HashSet<ClassAbility>();

        // === SISTEMA DE VITALS ===
        public float CurrentHunger { get; set; } = 100f;
        public float MaxHunger { get; set; } = 100f;
        public float CurrentSanity { get; set; } = 100f;
        public float MaxSanity { get; set; } = 100f;
        public float CurrentStamina { get; set; } = 100f;
        public float MaxStamina { get; set; } = 100f;

        // === SISTEMA DE REGENERAÇÃO ===
        private float healthRegenRate = 0f;
        private float manaRegenRate = 0f;
        private float staminaRegenRate = 0f;
        private float sanityRegenRate = 0f;
        private float hungerRegenRate = 0f;

        // === TIMERS E DELAYS ===
        public int CombatTimer { get; set; } = 0;
        public int StaminaRegenDelay { get; set; } = 0;

        // === MÉTODOS DE CLASSE ===
        public float GetClassLevel(string className)
        {
            if (ClassLevels.TryGetValue(className.ToLower(), out float level))
                return level;
            return 1f;
        }

        public void GainClassExp(string className, float amount)
        {
            className = className.ToLower();
            if (!ClassExperience.ContainsKey(className))
            {
                ClassExperience[className] = 0f;
                ClassLevels[className] = 1f;
            }

            float oldLevel = ClassLevels[className];
            ClassExperience[className] += amount;

            // Calcular novo nível (1 nível a cada 100 XP)
            float newLevel = 1f + (ClassExperience[className] / 100f);
            ClassLevels[className] = newLevel;

            // Se subiu de nível, verificar habilidades desbloqueadas
            if ((int)newLevel > (int)oldLevel)
            {
                CheckClassAbilities(className, (int)newLevel);
            }
        }

        private void CheckClassAbilities(string className, int level)
        {
            // Verificar habilidades baseadas no nível
            foreach (var ability in RPGClassDefinitions.ActionClasses[className].Milestones.Keys)
            {
                if ((int)ability <= level && !UnlockedAbilities.Contains(ability))
                {
                    UnlockedAbilities.Add(ability);
                    if (Main.myPlayer == Player.whoAmI)
                    {
                        Main.NewText($"Habilidade desbloqueada: {RPGClassDefinitions.ActionClasses[className].Milestones[ability]}!", Color.Yellow);
                    }
                }
            }
        }

        public override void Initialize()
        {
            // Inicializar classes
            ClassLevels["warrior"] = 1f;
            ClassLevels["archer"] = 1f;
            ClassLevels["mage"] = 1f;
            ClassLevels["summoner"] = 1f;
            ClassLevels["acrobat"] = 1f;
            ClassLevels["explorer"] = 1f;
            ClassLevels["engineer"] = 1f;
            ClassLevels["survivalist"] = 1f;
            ClassLevels["blacksmith"] = 1f;
            ClassLevels["alchemist"] = 1f;
            ClassLevels["mystic"] = 1f;

            // Inicializar experiência
            foreach (var className in ClassLevels.Keys)
            {
                ClassExperience[className] = 0f;
            }

            // Inicializar dash
            DashCooldown = 0;
            DashesUsed = 0;
            DashResetTimer = 0;
            DashSpeed = 12f;
            DashDuration = 15;
            DashInvincibilityFrames = 15;
            MaxDashes = 1;

            // Inicializar vitals
            CurrentHunger = MaxHunger;
            CurrentSanity = MaxSanity;
            CurrentStamina = MaxStamina;

            // Inicializar taxas de regeneração
            UpdateRegenRates();
        }

        public override void PreUpdate()
        {
            // Atualizar cooldowns e reset
            if (DashCooldown > 0) DashCooldown--;
            if (DashResetTimer > 0)
            {
                DashResetTimer--;
                if (DashResetTimer == 0) DashesUsed = 0;
            }

            // Atualizar dash baseado no nível do Acrobata
            float acrobatLevel = ClassLevels["acrobat"];
            if (acrobatLevel >= 100f)
            {
                MaxDashes = 3;
                DashSpeed = 16f;
                DashInvincibilityFrames = 30;
            }
            else if (acrobatLevel >= 75f)
            {
                MaxDashes = 2;
                DashSpeed = 14f;
                DashInvincibilityFrames = 25;
            }
            else if (acrobatLevel >= 50f)
            {
                MaxDashes = 2;
                DashSpeed = 14f;
                DashInvincibilityFrames = 20;
            }
            else if (acrobatLevel >= 25f)
            {
                MaxDashes = 1;
                DashSpeed = 12f;
                DashInvincibilityFrames = 15;
            }
            else
            {
                MaxDashes = 1;
                DashSpeed = 12f;
                DashInvincibilityFrames = 15;
            }

            // Atualizar taxas de regeneração
            UpdateRegenRates();

            // Aplicar regeneração
            if (CurrentHunger >= 20f) // Só regenera vida se não estiver com fome crítica
            {
                Player.lifeRegen += (int)(healthRegenRate * 2); // Converter para o sistema do Terraria
            }

            Player.manaRegen += (int)(manaRegenRate * 2); // Converter para o sistema do Terraria

            if (CurrentStamina < MaxStamina && DashCooldown <= 0)
            {
                CurrentStamina = MathHelper.Min(MaxStamina, CurrentStamina + staminaRegenRate);
            }

            if (CurrentSanity < MaxSanity)
            {
                CurrentSanity = MathHelper.Min(MaxSanity, CurrentSanity + sanityRegenRate);
            }

            if (CurrentHunger < MaxHunger)
            {
                CurrentHunger = MathHelper.Min(MaxHunger, CurrentHunger + hungerRegenRate);
            }
        }

        private void UpdateRegenRates()
        {
            // Taxas base
            healthRegenRate = 0.1f;
            manaRegenRate = 0.2f;
            staminaRegenRate = 0.3f;
            sanityRegenRate = 0.1f;
            hungerRegenRate = -0.05f; // Perde fome naturalmente

            // Bônus do Místico
            if (ClassLevels.TryGetValue("mystic", out float mysticLevel))
            {
                float mysticBonus = mysticLevel * 0.02f; // 2% por nível
                healthRegenRate += mysticBonus;
                manaRegenRate += mysticBonus;
                staminaRegenRate += mysticBonus;
                sanityRegenRate += mysticBonus;
                hungerRegenRate += mysticBonus * 0.5f; // Reduz a perda de fome
            }

            // Bônus do Sobrevivente
            if (ClassLevels.TryGetValue("survivalist", out float survivalistLevel))
            {
                float survivalistBonus = survivalistLevel * 0.01f; // 1% por nível
                healthRegenRate += survivalistBonus;
                hungerRegenRate += survivalistBonus * 0.5f;
            }

            // Bônus do Acrobata
            if (ClassLevels.TryGetValue("acrobat", out float acrobatLevel))
            {
                staminaRegenRate += acrobatLevel * 0.01f; // 1% por nível
            }

            // Bônus do Mago
            if (ClassLevels.TryGetValue("mage", out float mageLevel))
            {
                manaRegenRate += mageLevel * 0.01f; // 1% por nível
            }

            // Modificadores baseados nos vitals atuais
            if (CurrentHunger < 20f)
            {
                healthRegenRate = 0f; // Sem regeneração de vida com fome crítica
                staminaRegenRate *= 0.5f; // Stamina regenera mais devagar
            }

            if (CurrentSanity < 30f)
            {
                manaRegenRate *= 0.5f; // Mana regenera mais devagar
                sanityRegenRate *= 0.5f; // Sanidade regenera mais devagar
            }
        }

        public void PerformDash(int direction)
        {
            // Verificar se pode usar dash
            if (DashCooldown > 0 || DashesUsed >= MaxDashes || CurrentStamina < 20f)
                return;

            // Consumir stamina
            CurrentStamina -= 20f;

            // Aplicar velocidade do dash
            Player.velocity.X = direction * DashSpeed;

            // Configurar cooldowns
            DashCooldown = DashDuration;
            DashesUsed++;
            DashResetTimer = 60; // 1 segundo para resetar os dashes

            // Som e efeitos visuais
            SoundEngine.PlaySound(SoundID.Item6 with { Volume = 0.5f, Pitch = 0.0f }, Player.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
            }

            // Dar XP para a classe Acrobata
            if (Main.netMode != NetmodeID.Server)
            {
                float xpGain = 5f;
                if (DashesUsed > 1) xpGain *= 1.5f;
                ClassExperience["acrobat"] += xpGain;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Detectar double-tap usando o sistema vanilla
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] < 15)
            {
                PerformDash(1);
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] < 15)
            {
                PerformDash(-1);
            }
        }
    }
}
 