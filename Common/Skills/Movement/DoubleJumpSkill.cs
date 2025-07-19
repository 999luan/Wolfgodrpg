using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Movement
{
    /// <summary>
    /// Skill de double jump que permite pular novamente no ar.
    /// </summary>
    public class DoubleJumpSkill : BaseSkill
    {
        /// <summary>
        /// Indica se o jogador já usou o double jump.
        /// </summary>
        public bool HasUsedDoubleJump { get; private set; } = false;

        /// <summary>
        /// Força do double jump (multiplicador da velocidade de pulo normal).
        /// </summary>
        public float JumpForce { get; set; } = 0.8f;

        public DoubleJumpSkill()
        {
            Name = "Double Jump";
            Description = "Permite pular novamente no ar.\nConsome 10% de Stamina.";
            Cooldown = 0; // Sem cooldown, mas só pode usar uma vez por pulo
            Level = 0; // Desbloqueada no nível 3
            StaminaCost = 10f; // 10% da stamina
        }

        protected override bool OnActivate(Player player)
        {
            // Verificar se está no ar e não usou o double jump ainda
            if (player.velocity.Y == 0 || HasUsedDoubleJump) return false;

            // Aplicar double jump
            player.velocity.Y = -Player.jumpSpeed * JumpForce;
            HasUsedDoubleJump = true;
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            
            // Efeito visual (partículas)
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.Cloud, 
                            Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, Color.White);
            }
            
            return true;
        }

        public override void Update(Player player)
        {
            base.Update(player);

            // Resetar o double jump quando tocar o chão
            if (player.velocity.Y == 0)
            {
                HasUsedDoubleJump = false;
            }
        }

        /// <summary>
        /// Verifica se o double jump está disponível para uso.
        /// </summary>
        public override bool IsAvailable
        {
            get
            {
                if (!base.IsAvailable) return false;
                if (HasUsedDoubleJump) return false;
                
                var player = Main.LocalPlayer;
                if (player == null) return false;
                
                return player.velocity.Y != 0;
            }
        }

        /// <summary>
        /// Obtém uma descrição formatada da skill para UI.
        /// </summary>
        public override string GetDisplayDescription()
        {
            string desc = base.GetDisplayDescription();
            
            if (HasUsedDoubleJump)
                desc += "\n[Usado]";
            else if (IsAvailable)
                desc += "\n[Disponível]";
            else
                desc += "\n[Indisponível]";
                
            return desc;
        }
    }
} 