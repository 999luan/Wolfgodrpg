using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Movement
{
    /// <summary>
    /// Skill de wall jump que permite pular contra paredes.
    /// </summary>
    public class WallJumpSkill : BaseSkill
    {
        /// <summary>
        /// Força do wall jump (multiplicador da velocidade de pulo normal).
        /// </summary>
        public float JumpForce { get; set; } = 0.9f;

        /// <summary>
        /// Multiplicador da velocidade horizontal no wall jump.
        /// </summary>
        public float HorizontalForce { get; set; } = 0.5f;

        public WallJumpSkill()
        {
            Name = "Wall Jump";
            Description = "Salta contra paredes.\nConsome 10% de Stamina.";
            Cooldown = 15; // 0.25 segundos
            Level = 0; // Desbloqueada no nível 4
            StaminaCost = 10f; // 10% da stamina
        }

        protected override bool OnActivate(Player player)
        {
            // Verificar se está tocando uma parede
            bool touchingLeftWall = Collision.SolidCollision(player.position + new Vector2(-8, 0), 1, player.height);
            bool touchingRightWall = Collision.SolidCollision(player.position + new Vector2(player.width + 8, 0), 1, player.height);

            if (!touchingLeftWall && !touchingRightWall) return false;

            // Determinar direção do wall jump
            int jumpDirection = 0;
            if (touchingLeftWall) jumpDirection = 1; // Pular para direita
            else if (touchingRightWall) jumpDirection = -1; // Pular para esquerda

            // Aplicar wall jump
            player.velocity.Y = -Player.jumpSpeed * JumpForce;
            player.velocity.X = jumpDirection * Player.jumpSpeed * HorizontalForce;
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            
            // Efeito visual (partículas)
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.Stone, 
                            Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Gray);
            }
            
            return true;
        }

        public override void Update(Player player)
        {
            base.Update(player);
            // Wall jump não precisa de atualização específica
        }

        /// <summary>
        /// Verifica se o wall jump está disponível para uso.
        /// </summary>
        public override bool IsAvailable
        {
            get
            {
                if (!base.IsAvailable) return false;
                
                var player = Main.LocalPlayer;
                if (player == null) return false;
                
                // Verificar se está tocando uma parede
                bool touchingLeftWall = Collision.SolidCollision(player.position + new Vector2(-8, 0), 1, player.height);
                bool touchingRightWall = Collision.SolidCollision(player.position + new Vector2(player.width + 8, 0), 1, player.height);
                
                return touchingLeftWall || touchingRightWall;
            }
        }

        /// <summary>
        /// Obtém uma descrição formatada da skill para UI.
        /// </summary>
        public override string GetDisplayDescription()
        {
            string desc = base.GetDisplayDescription();
            
            var player = Main.LocalPlayer;
            if (player != null)
            {
                bool touchingLeftWall = Collision.SolidCollision(player.position + new Vector2(-8, 0), 1, player.height);
                bool touchingRightWall = Collision.SolidCollision(player.position + new Vector2(player.width + 8, 0), 1, player.height);
                
                if (touchingLeftWall || touchingRightWall)
                    desc += "\n[Disponível]";
                else
                    desc += "\n[Indisponível - Sem parede]";
            }
                
            return desc;
        }
    }
} 