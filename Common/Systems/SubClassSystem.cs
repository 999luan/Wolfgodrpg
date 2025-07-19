using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Classes.SubClasses;

using Terraria.ModLoader.IO;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema para gerenciar subclasses do jogador
    /// </summary>
    public class SubClassSystem
    {
        // === PROPRIEDADES ===
        public List<PlayerSubClass> SubClasses { get; private set; } = new List<PlayerSubClass>();
        public PlayerSubClass ActiveSubClass { get; private set; }
        
        // === EVENTOS ===
        public event Action OnSubClassChanged;
        public event Action OnSubClassUnlocked;
        public event Action OnTotalLevelChanged;
        
        // === RASTREAMENTO DE MUDANÇAS ===
        private int _previousTotalLevel = 0;
        
        // === CONSTRUTOR ===
        public SubClassSystem()
        {
            InitializeSubClasses();
            _previousTotalLevel = GetTotalLevel();
        }
        
        // === INICIALIZAÇÃO ===
        /// <summary>
        /// Inicializa todas as subclasses disponíveis
        /// </summary>
        private void InitializeSubClasses()
        {
            // Adicionar subclasses disponíveis
            SubClasses.Add(new WarriorSubClass());
            SubClasses.Add(new AcrobatSubClass());
            SubClasses.Add(new ArcherSubClass());
            SubClasses.Add(new MageSubClass());
            SubClasses.Add(new SummonerSubClass());
            SubClasses.Add(new ExplorerSubClass());
            
            // Desbloquear primeira subclasse por padrão
            if (SubClasses.Count > 0)
            {
                SubClasses[0].Unlock();
                SetActiveSubClass(SubClasses[0]);
            }
        }

        public void SaveData(TagCompound tag)
        {
            var subClassTags = new List<TagCompound>();
            foreach (var subClass in SubClasses)
            {
                var scTag = new TagCompound();
                scTag.Add("Name", subClass.Name);
                scTag.Add("Level", subClass.Level);
                scTag.Add("XP", subClass.XP);
                scTag.Add("IsUnlocked", subClass.IsUnlocked);
                subClassTags.Add(scTag);
            }
            tag["SubClasses"] = subClassTags;
        }

        public void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("SubClasses"))
            {
                var subClassTags = tag.Get<List<TagCompound>>("SubClasses");
                foreach (var scTag in subClassTags)
                {
                    var subclassName = scTag.GetString("Name");
                    var subclass = SubClasses.FirstOrDefault(sc => sc.Name == subclassName);
                    if (subclass != null)
                    {
                        subclass.SetLevel(scTag.GetInt("Level"));
                        subclass.SetXP(scTag.GetInt("XP"));
                        subclass.SetUnlocked(scTag.GetBool("IsUnlocked"));
                    }
                }
            }
        }

        
        // === GERENCIAMENTO DE SUBCLASSES ===
        /// <summary>
        /// Define a subclasse ativa
        /// </summary>
        public void SetActiveSubClass(PlayerSubClass subClass)
        {
            if (!SubClasses.Contains(subClass)) return;
            
            ActiveSubClass = subClass;
            OnSubClassChanged?.Invoke();
            
            Main.NewText($"Active subclass: {subClass.Name}", subClass.GetClassColor());
        }
        
        /// <summary>
        /// Desbloqueia uma subclasse
        /// </summary>
        public void UnlockSubClass(string subClassName)
        {
            var subClass = SubClasses.FirstOrDefault(sc => sc.Name == subClassName);
            if (subClass != null && !subClass.IsUnlocked)
            {
                int oldTotalLevel = GetTotalLevel();
                subClass.Unlock();
                OnSubClassUnlocked?.Invoke();
                CheckTotalLevelChange(oldTotalLevel);
            }
        }
        
        /// <summary>
        /// Adiciona XP a uma subclasse específica
        /// </summary>
        public void AddXPToSubClass(string subClassName, int amount)
        {
            var subClass = SubClasses.FirstOrDefault(sc => sc.Name == subClassName);
            if (subClass != null && subClass.IsUnlocked)
            {
                int oldTotalLevel = GetTotalLevel();
                subClass.AddXP(amount);
                CheckTotalLevelChange(oldTotalLevel);
            }
        }
        
        /// <summary>
        /// Adiciona XP à subclasse ativa
        /// </summary>
        public void AddXPToActiveSubClass(int amount)
        {
            if (ActiveSubClass != null && ActiveSubClass.IsUnlocked)
            {
                int oldTotalLevel = GetTotalLevel();
                ActiveSubClass.AddXP(amount);
                CheckTotalLevelChange(oldTotalLevel);
            }
        }
        
        /// <summary>
        /// Verifica se o nível total mudou e dispara o evento se necessário
        /// </summary>
        private void CheckTotalLevelChange(int oldTotalLevel)
        {
            int newTotalLevel = GetTotalLevel();
            if (newTotalLevel != oldTotalLevel)
            {
                OnTotalLevelChanged?.Invoke();
                _previousTotalLevel = newTotalLevel;
            }
        }
        
        // === CÁLCULOS DE ESTATÍSTICAS ===
        /// <summary>
        /// Retorna o nível total de todas as subclasses
        /// </summary>
        public int GetTotalLevel()
        {
            return SubClasses.Where(sc => sc.IsUnlocked).Sum(sc => sc.Level);
        }
        
        /// <summary>
        /// Retorna o XP total de todas as subclasses
        /// </summary>
        public int GetTotalXP()
        {
            return SubClasses.Where(sc => sc.IsUnlocked).Sum(sc => sc.TotalXP);
        }
        
        /// <summary>
        /// Retorna modificadores de stats combinados de todas as subclasses
        /// </summary>
        public Dictionary<string, float> GetCombinedStatModifiers()
        {
            var combinedModifiers = new Dictionary<string, float>();
            
            foreach (var subClass in SubClasses.Where(sc => sc.IsUnlocked))
            {
                var modifiers = subClass.GetStatModifiers();
                
                foreach (var modifier in modifiers)
                {
                    if (combinedModifiers.ContainsKey(modifier.Key))
                    {
                        // Multiplicar modificadores do mesmo tipo
                        combinedModifiers[modifier.Key] *= modifier.Value;
                    }
                    else
                    {
                        combinedModifiers[modifier.Key] = modifier.Value;
                    }
                }
            }
            
            return combinedModifiers;
        }
        
        /// <summary>
        /// Retorna modificadores da subclasse ativa
        /// </summary>
        public Dictionary<string, float> GetActiveSubClassModifiers()
        {
            return ActiveSubClass?.GetStatModifiers() ?? new Dictionary<string, float>();
        }
        
        // === GERENCIAMENTO DE SKILLS ===
        /// <summary>
        /// Usa uma skill da subclasse ativa
        /// </summary>
        public void UseActiveSubClassSkill(int skillIndex)
        {
            if (ActiveSubClass != null)
            {
                ActiveSubClass.UseSkill(skillIndex, Main.LocalPlayer);
            }
        }
        
        /// <summary>
        /// Usa uma skill passiva da subclasse ativa
        /// </summary>
        public void UseActiveSubClassPassiveSkill(int skillIndex)
        {
            if (ActiveSubClass != null)
            {
                ActiveSubClass.UsePassiveSkill(skillIndex, Main.LocalPlayer);
            }
        }
        
        /// <summary>
        /// Atualiza todas as skills de todas as subclasses
        /// </summary>
        public void UpdateAllSkills()
        {
            foreach (var subClass in SubClasses)
            {
                subClass.UpdateSkills(Main.LocalPlayer);
            }
        }
        
        // === ESTATÍSTICAS ===
        /// <summary>
        /// Retorna o total de skills desbloqueadas
        /// </summary>
        public int GetTotalUnlockedSkills()
        {
            return SubClasses.Where(sc => sc.IsUnlocked).Sum(sc => sc.GetUnlockedSkillsCount());
        }
        
        /// <summary>
        /// Retorna o total de skills disponíveis
        /// </summary>
        public int GetTotalAvailableSkills()
        {
            return SubClasses.Sum(sc => sc.GetTotalSkillsCount());
        }
        
        /// <summary>
        /// Retorna a subclasse com maior nível
        /// </summary>
        public PlayerSubClass GetHighestLevelSubClass()
        {
            return SubClasses.Where(sc => sc.IsUnlocked).OrderByDescending(sc => sc.Level).FirstOrDefault();
        }
        
        /// <summary>
        /// Retorna a subclasse com maior XP total
        /// </summary>
        public PlayerSubClass GetHighestXPSubClass()
        {
            return SubClasses.Where(sc => sc.IsUnlocked).OrderByDescending(sc => sc.TotalXP).FirstOrDefault();
        }
        
        // === MÉTODOS DE DEBUG ===
        /// <summary>
        /// Retorna string formatada com status de todas as subclasses
        /// </summary>
        public string GetStatusString()
        {
            string status = $"Total Level: {GetTotalLevel()} | Total XP: {GetTotalXP()}\n";
            
            foreach (var subClass in SubClasses.Where(sc => sc.IsUnlocked))
            {
                status += $"{subClass.GetStatusString()}\n";
            }
            
            return status.TrimEnd('\n');
        }
        
        /// <summary>
        /// Loga o status de todas as subclasses
        /// </summary>
        public void LogStatus()
        {
            Main.NewText($"SubClass System Status:\n{GetStatusString()}", Color.Cyan);
        }
        
        // === MÉTODOS DE RESET ===
        /// <summary>
        /// Reseta todas as subclasses
        /// </summary>
        public void ResetAllSubClasses()
        {
            foreach (var subClass in SubClasses)
            {
                subClass.Reset();
            }
            
            // Re-desbloquear primeira subclasse
            if (SubClasses.Count > 0)
            {
                SubClasses[0].Unlock();
                SetActiveSubClass(SubClasses[0]);
            }
        }
        
        /// <summary>
        /// Reseta uma subclasse específica
        /// </summary>
        public void ResetSubClass(string subClassName)
        {
            var subClass = SubClasses.FirstOrDefault(sc => sc.Name == subClassName);
            if (subClass != null)
            {
                subClass.Reset();
            }
        }
    }
} 