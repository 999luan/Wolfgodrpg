using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Movement
{
    public class DoubleJumpSkill : BaseSkill
    {
        public DoubleJumpSkill()
        {
            Name = "Double Jump";
            Description = "Perform an additional jump in mid-air.";
            Cooldown = 0; // No cooldown for double jump, managed by player state
            StaminaCost = 10;
            Level = 1; // Unlocked by default for testing
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Check if player is in mid-air and hasn't used double jump yet
            if (player.velocity.Y == 0 || modPlayer.usedDoubleJump) return false; // Assuming usedDoubleJump is a field in RPGPlayer

            // Consume stamina
            if (!modPlayer.ConsumeStaminaPercent(StaminaCost)) return false;

            player.velocity.Y = -Player.jumpSpeed * 0.8f;
            // modPlayer.usedDoubleJump = true; // This needs to be set in RPGPlayer's PreUpdateMovement

            // Visual and sound effects
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Cloud,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, Color.White);
            }
            SoundEngine.PlaySound(SoundID.Item24, player.position);

            return true;
        }
    }
}