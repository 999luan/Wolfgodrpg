using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Movement
{
    public class WallJumpSkill : BaseSkill
    {
        public WallJumpSkill()
        {
            Name = "Wall Jump";
            Description = "Jump off walls to gain height.";
            Cooldown = 0; // No cooldown, managed by player state
            StaminaCost = 10;
            Level = 1; // Unlocked by default for testing
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Check if touching a wall and in mid-air
            if (player.velocity.Y == 0 || !modPlayer.IsTouchingWall(out int side)) return false; // Assuming IsTouchingWall is in RPGPlayer

            // Consume stamina
            if (!modPlayer.ConsumeStaminaPercent(StaminaCost)) return false;

            player.velocity.Y = -Player.jumpSpeed * 0.9f;
            player.velocity.X = 6f * -side;

            // Visual and sound effects
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Stone,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Gray);
            }
            SoundEngine.PlaySound(SoundID.Item24, player.position);

            return true;
        }
    }
}