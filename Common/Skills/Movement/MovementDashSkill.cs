using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Movement
{
    /// <summary>
    /// Skill de dash que permite movimento rápido na horizontal.
    /// </summary>
    public class MovementDashSkill : BaseSkill
    {
        /// <summary>
        /// Velocidade do dash em pixels por frame.
        /// </summary>
        public float DashSpeed { get; set; } = 12f;

        /// <summary>
        /// Duração do dash em frames.
        /// </summary>
        public int DashDuration { get; set; } = 15;

        /// <summary>
        /// Frames de invencibilidade durante o dash.
        /// </summary>
        public int InvincibilityFrames { get; set; } = 15;

        /// <summary>
        /// Timer atual do dash.
        /// </summary>
        private int dashTimer = 0;

        /// <summary>
        /// Direção atual do dash.
        /// </summary>
        private Vector2 dashDirection = Vector2.Zero;

        public MovementDashSkill()
        {
            Name = "Dash";
            Description = "Dá um impulso rápido para frente.\nConsome 10% de Stamina.";
            Cooldown = 30; // 0.5 segundos
            Level = 1; // Começa desbloqueada
            StaminaCost = 10f; // 10% da stamina
        }

        protected override bool OnActivate(Player player)
        {
            // Determinar direção do dash
            int dashDir = 0;
            if (player.controlLeft) dashDir = -1;
            else if (player.controlRight) dashDir = 1;

            if (dashDir == 0) return false; // Sem direção

            // Aplicar dash
            dashDirection = new Vector2(dashDir, 0);
            dashTimer = DashDuration;
            
            // Aplicar velocidade
            player.velocity.X = DashSpeed * dashDir;
            
            // Aplicar invencibilidade
            player.immune = true;
            player.immuneTime = InvincibilityFrames;
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            
            return true;
        }

        public override void Update(Player player)
        {
            base.Update(player);

            // Atualizar timer do dash
            if (dashTimer > 0)
            {
                dashTimer--;
                
                // Manter velocidade durante o dash
                if (dashDirection.X != 0)
                {
                    player.velocity.X = DashSpeed * dashDirection.X;
                }
                
                // Resetar quando o dash terminar
                if (dashTimer <= 0)
                {
                    dashDirection = Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Verifica se o jogador está atualmente fazendo dash.
        /// </summary>
        public bool IsDashing => dashTimer > 0;

        /// <summary>
        /// Obtém a direção atual do dash.
        /// </summary>
        public Vector2 GetDashDirection() => dashDirection;

        /// <summary>
        /// Obtém o tempo restante do dash em frames.
        /// </summary>
        public int GetDashTimeRemaining() => dashTimer;
    }
} 