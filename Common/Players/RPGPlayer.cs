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

        private int _lastLife;
        private bool _wasWellFed;

        // Configuração
        private RPGConfig Config => ModContent.GetInstance<RPGConfig>();

        public override void Initialize()
        {
            DebugLog.Player("Initialize", "Inicializando RPGPlayer");
            InitializeClasses();
            DebugLog.Player("Initialize", "RPGPlayer inicializado com sucesso");
        }

        private void InitializeClasses()
        {
            DebugLog.Player("InitializeClasses", "Inicializando classes do jogador");
            foreach (var className in RPGClassDefinitions.ActionClasses.Keys)
            {
                if (!ClassLevels.ContainsKey(className))
                    ClassLevels[className] = 1f;
                if (!ClassExperience.ContainsKey(className))
                    ClassExperience[className] = 0f;
                
                DebugLog.Player("InitializeClasses", $"Classe '{className}' inicializada - Level: {ClassLevels[className]}, XP: {ClassExperience[className]}");
            }
        }

        public override void PostUpdate()
        {
            // Atualizar timer de combate
            if (CombatTimer > 0)
            {
                CombatTimer--;
                if (CombatTimer == 0)
                {
                    DebugLog.Player("PostUpdate", "Timer de combate expirado - jogador saiu do combate");
                }
            }

            // Verificar mudanças de status
            CheckStatusChanges();
        }

        private void CheckStatusChanges()
        {
            // Verificar mudanças de fome
            if (CurrentHunger < 20f && _wasWellFed)
            {
                DebugLog.Player("CheckStatusChanges", $"Fome crítica detectada: {CurrentHunger:F1}% - regeneração de vida desabilitada");
                _wasWellFed = false;
            }
            else if (CurrentHunger >= 20f && !_wasWellFed)
            {
                DebugLog.Player("CheckStatusChanges", $"Fome normalizada: {CurrentHunger:F1}% - regeneração de vida reabilitada");
                _wasWellFed = true;
            }

            // Verificar mudanças de sanidade
            if (CurrentSanity < 30f)
            {
                DebugLog.Player("CheckStatusChanges", $"Sanidade baixa: {CurrentSanity:F1}% - efeitos negativos podem ocorrer");
            }
        }

        public float GetClassLevel(string className)
        {
            if (!ClassLevels.ContainsKey(className))
            {
                DebugLog.Warn("Player", "GetClassLevel", $"Classe '{className}' não encontrada, retornando 1");
                return 1f;
            }
            return ClassLevels[className];
        }

        public void GainClassExp(string className, float amount)
        {
            if (!ClassExperience.ContainsKey(className)) 
            {
                DebugLog.Error("Player", "GainClassExp", $"Classe '{className}' não encontrada no dicionário de experiência");
                return;
            }

            float oldXP = ClassExperience[className];
            float oldLevel = ClassLevels[className];
            
            float finalAmount = amount * Config.ExpMultiplier;
            if (Main.hardMode) finalAmount *= 1.5f;
            if (NPC.downedMoonlord) finalAmount *= 2f;

            ClassExperience[className] += finalAmount;
            float newXP = ClassExperience[className];

            DebugLog.Gameplay("Player", "GainClassExp", $"Classe: {className}, XP ganho: {finalAmount:F1}, XP total: {newXP:F1}, Level atual: {oldLevel:F1}");

            float newLevel = CalculateLevelFromXP(ClassExperience[className]);

            if (newLevel > oldLevel)
            {
                ClassLevels[className] = newLevel;
                DebugLog.Gameplay("Player", "GainClassExp", $"LEVEL UP! Classe: {className}, Level: {oldLevel:F1} -> {newLevel:F1}");
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
            DebugLog.Player("CheckForNewAbilities", $"Verificando habilidades para classe '{className}' no nível {level:F1}");
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
                DebugLog.Gameplay("Player", "UnlockAbility", $"Nova habilidade desbloqueada: {ability}");
                Main.NewText($"Nova habilidade desbloqueada: {ability}!", Color.LightBlue);
            }
            else
            {
                DebugLog.Player("UnlockAbility", $"Habilidade '{ability}' já estava desbloqueada");
            }
        }

        public bool HasUnlockedAbility(string ability)
        {
            return UnlockedAbilities.Contains(ability);
        }

        // === SISTEMA DE STAMINA ===
        public bool ConsumeStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                float oldStamina = CurrentStamina;
                CurrentStamina -= amount;
                StaminaRegenDelay = 60; // 1 segundo de delay
                
                DebugLog.Player("ConsumeStamina", $"Stamina consumida: {amount:F1}, Stamina restante: {CurrentStamina:F1}/{MaxStamina:F1}");
                return true;
            }
            
            DebugLog.Player("ConsumeStamina", $"Stamina insuficiente: necessário {amount:F1}, disponível {CurrentStamina:F1}");
            return false;
        }

        public void RestoreStamina(float amount)
        {
            float oldStamina = CurrentStamina;
            CurrentStamina = System.Math.Min(MaxStamina, CurrentStamina + amount);
            
            if (CurrentStamina > oldStamina)
            {
                DebugLog.Player("RestoreStamina", $"Stamina restaurada: {amount:F1}, Stamina atual: {CurrentStamina:F1}/{MaxStamina:F1}");
            }
        }

        // === SAVE/LOAD ===
        public override void SaveData(TagCompound tag)
        {
            DebugLog.Player("SaveData", "Salvando dados do RPGPlayer");
            
            // Salvar classes
            tag["classLevels"] = ClassLevels;
            tag["classExperience"] = ClassExperience;
            tag["unlockedAbilities"] = UnlockedAbilities.ToList();
            
            // Salvar vitals
            tag["currentHunger"] = CurrentHunger;
            tag["maxHunger"] = MaxHunger;
            tag["currentSanity"] = CurrentSanity;
            tag["maxSanity"] = MaxSanity;
            tag["currentStamina"] = CurrentStamina;
            tag["maxStamina"] = MaxStamina;
            
            DebugLog.Player("SaveData", "Dados do RPGPlayer salvos com sucesso");
        }

        public override void LoadData(TagCompound tag)
        {
            DebugLog.Player("LoadData", "Carregando dados do RPGPlayer");
            
            try
            {
                // Carregar classes
                ClassLevels = tag.Get<Dictionary<string, float>>("classLevels") ?? new Dictionary<string, float>();
                ClassExperience = tag.Get<Dictionary<string, float>>("classExperience") ?? new Dictionary<string, float>();
                var unlockedAbilitiesList = tag.Get<List<string>>("unlockedAbilities") ?? new List<string>();
                UnlockedAbilities = new HashSet<string>(unlockedAbilitiesList);
                
                // Carregar vitals
                CurrentHunger = tag.GetFloat("currentHunger", 100f);
                MaxHunger = tag.GetFloat("maxHunger", 100f);
                CurrentSanity = tag.GetFloat("currentSanity", 100f);
                MaxSanity = tag.GetFloat("maxSanity", 100f);
                CurrentStamina = tag.GetFloat("currentStamina", 100f);
                MaxStamina = tag.GetFloat("maxStamina", 100f);
                
                DebugLog.Player("LoadData", $"Dados carregados - Classes: {ClassLevels.Count}, Habilidades: {UnlockedAbilities.Count}, Vitals: Hunger={CurrentHunger:F1}, Sanity={CurrentSanity:F1}, Stamina={CurrentStamina:F1}");
            }
            catch (System.Exception ex)
            {
                DebugLog.Error("Player", "LoadData", "Erro ao carregar dados do RPGPlayer", ex);
                // Reinicializar com valores padrão em caso de erro
                InitializeClasses();
            }
        }

        public override void OnEnterWorld()
        {
            DebugLog.Player("OnEnterWorld", $"Jogador '{Player.name}' entrou no mundo");
            DebugLog.Player("OnEnterWorld", $"Status inicial - Hunger: {CurrentHunger:F1}%, Sanity: {CurrentSanity:F1}%, Stamina: {CurrentStamina:F1}%");
        }

        public override void OnRespawn()
        {
            DebugLog.Player("OnRespawn", $"Jogador '{Player.name}' respawnou");
            // Restaurar vitals ao respawnar
            CurrentHunger = MaxHunger;
            CurrentSanity = MaxSanity;
            CurrentStamina = MaxStamina;
            DebugLog.Player("OnRespawn", "Vitals restaurados ao respawnar");
        }
    }
}
 