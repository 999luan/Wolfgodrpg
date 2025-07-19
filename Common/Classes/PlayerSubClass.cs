using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Classes
{
    /// <summary>
    /// Classe base abstrata para todas as subclasses do jogador
    /// </summary>
    public abstract class PlayerSubClass
    {
        // === PROPRIEDADES BÁSICAS ===
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Icon { get; protected set; } = "⚔️"; // Emoji padrão
        
        public int Level { get; protected set; } = 1;
        public int XP { get; protected set; } = 0;
        public int TotalXP { get; protected set; } = 0;
        
        // === SKILLS ===
        public List<BaseSkill> Skills { get; private set; } = new List<BaseSkill>();
        public List<BaseSkill> PassiveSkills { get; private set; } = new List<BaseSkill>();
        public List<BaseSkill> ActiveSkills { get; private set; } = new List<BaseSkill>();
        
        // === EVENTOS ===
        public event Action OnLevelUp;
        public event Action OnXPChanged;
        public event Action OnSkillUnlocked;
        
        // === ESTADO ===
        public bool IsActive { get; set; } = false;
        public bool IsUnlocked { get; set; } = false;
        
        // === CONSTRUTOR ===
        protected PlayerSubClass()
        {
            InitializeSkills();
        }
        
        // === MÉTODOS ABSTRATOS ===
        /// <summary>
        /// Inicializa as skills específicas da subclasse
        /// </summary>
        protected abstract void InitializeSkills();
        
        /// <summary>
        /// Retorna os modificadores de stats da subclasse
        /// </summary>
        public abstract Dictionary<string, float> GetStatModifiers();
        
        /// <summary>
        /// Retorna a cor temática da subclasse
        /// </summary>
        public abstract Color GetClassColor();
        
        // === MÉTODOS DE PROGRESSÃO ===
        /// <summary>
        /// Adiciona XP à subclasse
        /// </summary>
        public virtual void AddXP(int amount)
        {
            if (!IsUnlocked) return;
            
            XP += amount;
            TotalXP += amount;
            OnXPChanged?.Invoke();
            
            // Verificar level up
            while (XP >= XPToNextLevel())
            {
                XP -= XPToNextLevel();
                LevelUp();
            }
        }
        
        /// <summary>
        /// Calcula XP necessário para o próximo nível
        /// </summary>
        protected virtual int XPToNextLevel()
        {
            return 100 + (Level * 50) + (Level * Level * 10);
        }
        
        /// <summary>
        /// Executa o level up da subclasse
        /// </summary>
        protected virtual void LevelUp()
        {
            Level++;
            OnLevelUp?.Invoke();
            
            // Desbloquear skills baseado no nível
            UnlockSkillsAtLevel(Level);
            
            // Notificar o jogador
            Main.NewText($"{Name} reached level {Level}!", GetClassColor());
            SoundEngine.PlaySound(SoundID.Item37, Main.LocalPlayer.position);
        }
        
        /// <summary>
        /// Desbloqueia skills baseado no nível
        /// </summary>
        protected virtual void UnlockSkillsAtLevel(int level)
        {
            var skillsToUnlock = Skills.Where(s => s.Level == 0 && GetSkillUnlockLevel(s) == level).ToList();
            
            foreach (var skill in skillsToUnlock)
            {
                skill.SetLevel(1); // Desbloquear a skill
                OnSkillUnlocked?.Invoke();
                
                Main.NewText($"New {Name} skill unlocked: {skill.Name}!", GetClassColor());
                SoundEngine.PlaySound(SoundID.Item4, Main.LocalPlayer.position);
            }
        }
        
        /// <summary>
        /// Retorna o nível necessário para desbloquear uma skill
        /// </summary>
        protected virtual int GetSkillUnlockLevel(BaseSkill skill)
        {
            // Implementação padrão - pode ser sobrescrita pelas subclasses
            return 1; // Todas as skills desbloqueadas no nível 1 por padrão
        }
        
        // === MÉTODOS DE SKILLS ===
        /// <summary>
        /// Usa uma skill da subclasse
        /// </summary>
        public virtual void UseSkill(int skillIndex, Player player)
        {
            if (skillIndex < 0 || skillIndex >= ActiveSkills.Count) return;
            
            var skill = ActiveSkills[skillIndex];
            if (skill.IsAvailable)
            {
                skill.Activate(player);
            }
        }
        
        /// <summary>
        /// Usa uma skill passiva da subclasse
        /// </summary>
        public virtual void UsePassiveSkill(int skillIndex, Player player)
        {
            if (skillIndex < 0 || skillIndex >= PassiveSkills.Count) return;
            
            var skill = PassiveSkills[skillIndex];
            if (skill.IsAvailable)
            {
                skill.Activate(player);
            }
        }
        
        /// <summary>
        /// Adiciona uma skill à subclasse
        /// </summary>
        protected void AddSkill(BaseSkill skill)
        {
            Skills.Add(skill);
            
            // Classificar como ativa ou passiva baseado no nome ou tipo
            if (skill.Name.ToLower().Contains("passive") || skill.Name.ToLower().Contains("buff"))
            {
                PassiveSkills.Add(skill);
            }
            else
            {
                ActiveSkills.Add(skill);
            }
        }
        
        /// <summary>
        /// Atualiza todas as skills da subclasse
        /// </summary>
        public virtual void UpdateSkills(Player player)
        {
            foreach (var skill in Skills)
            {
                skill.Update(player);
            }
        }
        
        // === MÉTODOS DE ESTATÍSTICAS ===
        /// <summary>
        /// Retorna o progresso de XP atual (0-1)
        /// </summary>
        public float GetXPProgress()
        {
            if (XPToNextLevel() == 0) return 1f;
            return (float)XP / XPToNextLevel();
        }
        
        /// <summary>
        /// Retorna se a subclasse pode subir de nível
        /// </summary>
        public bool CanLevelUp()
        {
            return XP >= XPToNextLevel();
        }
        
        /// <summary>
        /// Retorna o total de skills desbloqueadas
        /// </summary>
        public int GetUnlockedSkillsCount()
        {
            return Skills.Count(s => s.IsUnlocked);
        }
        
        /// <summary>
        /// Retorna o total de skills disponíveis
        /// </summary>
        public int GetTotalSkillsCount()
        {
            return Skills.Count;
        }
        
        public void SetLevel(int level) => Level = level;
        public void SetXP(int xp) => XP = xp;
        public void SetUnlocked(bool unlocked) => IsUnlocked = unlocked;

        // === MÉTODOS DE DEBUG ===
        /// <summary>
        /// Retorna string formatada com status da subclasse
        /// </summary>
        public string GetStatusString()
        {
            return $"{Name} Lv.{Level} | XP: {XP}/{XPToNextLevel()} | Skills: {GetUnlockedSkillsCount()}/{GetTotalSkillsCount()}";
        }
        
        /// <summary>
        /// Loga o status atual da subclasse
        /// </summary>
        public void LogStatus()
        {
            Main.NewText($"SubClass Status: {GetStatusString()}", GetClassColor());
        }
        
        // === MÉTODOS DE RESET ===
        /// <summary>
        /// Reseta a subclasse para o estado inicial
        /// </summary>
        public virtual void Reset()
        {
            Level = 1;
            XP = 0;
            TotalXP = 0;
            IsActive = false;
            
            foreach (var skill in Skills)
            {
                skill.SetLevel(0); // Bloquear todas as skills
                skill.CooldownTimer = 0; // Resetar cooldowns
            }
        }
        
        /// <summary>
        /// Desbloqueia a subclasse
        /// </summary>
        public virtual void Unlock()
        {
            IsUnlocked = true;
            Main.NewText($"{Name} subclass unlocked!", GetClassColor());
            SoundEngine.PlaySound(SoundID.Item4, Main.LocalPlayer.position);
        }
    }
} 