using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills
{
    /// <summary>
    /// Classe base para todas as skills do sistema RPG.
    /// Fornece funcionalidades básicas como cooldown, nível e ativação.
    /// </summary>
    public abstract class BaseSkill
    {
        /// <summary>
        /// Nome da skill para exibição na UI.
        /// </summary>
        public string Name { get; set; } = "Unknown Skill";

        /// <summary>
        /// Descrição da skill para tooltips.
        /// </summary>
        public string Description { get; set; } = "No description available.";

        /// <summary>
        /// Nível atual da skill (0 = bloqueada, 1+ = desbloqueada).
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// Cooldown em frames (60 frames = 1 segundo).
        /// </summary>
        public int Cooldown { get; set; } = 0;

        /// <summary>
        /// Timer atual do cooldown em frames.
        /// </summary>
        public int CooldownTimer { get; set; } = 0;

        /// <summary>
        /// Custo de stamina em porcentagem (0-100).
        /// </summary>
        public float StaminaCost { get; set; } = 0f;

        /// <summary>
        /// Verifica se a skill está desbloqueada (nível > 0).
        /// </summary>
        public bool IsUnlocked => Level > 0;

        /// <summary>
        /// Verifica se a skill está disponível para uso (sem cooldown e com stamina).
        /// </summary>
        public virtual bool IsAvailable
        {
            get
            {
                if (!IsUnlocked) return false;
                if (CooldownTimer > 0) return false;
                
                // Verificar stamina se houver custo
                if (StaminaCost > 0f)
                {
                    var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                    if (modPlayer == null) return false;
                    return modPlayer.CurrentStamina >= StaminaCost;
                }
                
                return true;
            }
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        protected BaseSkill()
        {
        }

        /// <summary>
        /// Ativa a skill. Deve ser implementado pelas classes derivadas.
        /// </summary>
        /// <param name="player">Jogador que está ativando a skill</param>
        /// <returns>True se a skill foi ativada com sucesso</returns>
        public virtual bool Activate(Player player)
        {
            if (!IsAvailable) return false;

            // Consumir stamina se necessário
            if (StaminaCost > 0f)
            {
                var modPlayer = player.GetModPlayer<RPGPlayer>();
                if (modPlayer == null) return false;
                
                if (!modPlayer.ConsumeStaminaPercent(StaminaCost))
                    return false;
            }

            // Aplicar cooldown
            if (Cooldown > 0)
                CooldownTimer = Cooldown;

            // Chamar implementação específica
            return OnActivate(player);
        }

        /// <summary>
        /// Implementação específica da ativação da skill.
        /// Deve ser implementado pelas classes derivadas.
        /// </summary>
        /// <param name="player">Jogador que está ativando a skill</param>
        /// <returns>True se a skill foi ativada com sucesso</returns>
        protected abstract bool OnActivate(Player player);

        /// <summary>
        /// Atualiza a skill a cada frame.
        /// </summary>
        /// <param name="player">Jogador</param>
        public virtual void Update(Player player)
        {
            if (CooldownTimer > 0)
                CooldownTimer--;
        }

        /// <summary>
        /// Aumenta o nível da skill.
        /// </summary>
        /// <param name="newLevel">Novo nível</param>
        public virtual void SetLevel(int newLevel)
        {
            Level = newLevel;
        }

        /// <summary>
        /// Obtém o progresso do cooldown como porcentagem (0-100).
        /// </summary>
        public float GetCooldownProgress()
        {
            if (Cooldown <= 0) return 0f;
            return MathHelper.Clamp((float)CooldownTimer / Cooldown * 100f, 0f, 100f);
        }

        /// <summary>
        /// Obtém o tempo restante do cooldown em segundos.
        /// </summary>
        public float GetCooldownTimeRemaining()
        {
            return CooldownTimer / 60f; // 60 frames = 1 segundo
        }

        /// <summary>
        /// Obtém uma descrição formatada da skill para UI.
        /// </summary>
        public virtual string GetDisplayDescription()
        {
            string desc = Description;
            
            if (StaminaCost > 0f)
                desc += $"\nCusto: {StaminaCost:F0}% Stamina";
                
            if (Cooldown > 0)
                desc += $"\nCooldown: {Cooldown / 60f:F1}s";
                
            if (!IsUnlocked)
                desc += "\n[Bloqueada]";
            else if (CooldownTimer > 0)
                desc += $"\n[Cooldown: {GetCooldownTimeRemaining():F1}s]";
                
            return desc;
        }
    }
} 