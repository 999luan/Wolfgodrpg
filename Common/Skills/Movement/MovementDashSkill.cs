using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Movement
{
    public class MovementDashSkill : BaseSkill
    {
        public MovementDashSkill()
        {
            Name = "Dash";
            Description = "Perform a quick dash in the direction of movement.";
            Cooldown = 60; // 1 second
            StaminaCost = 10; // Example cost
            Level = 1; // Unlocked by default for testing
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Get input direction
            Vector2 direction = Vector2.Zero;
            if (player.controlUp) direction.Y -= 1;
            if (player.controlDown) direction.Y += 1;
            if (player.controlLeft) direction.X -= 1;
            if (player.controlRight) direction.X += 1;

            if (direction == Vector2.Zero) return false; // No direction, no dash

            direction.Normalize();

            // Apply dash velocity
            player.velocity = direction * modPlayer.DashSpeed;

            // Set invincibility frames
            player.immune = true;
            player.immuneTime = modPlayer.DashInvincibilityFrames;

            // Play sound and create dust effects
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(
                    player.position + new Vector2(Main.rand.Next(player.width), Main.rand.Next(player.height)),
                    0, 0, DustID.Smoke,
                    -direction.X * 3f, -direction.Y * 3f,
                    150, Color.White, 1.5f
                );
            }

            // Reset dash state in RPGPlayer
            modPlayer.DashCooldown = Cooldown; // Use skill's cooldown
            modPlayer.DashesUsed++;
            modPlayer.DashResetTimer = 180; // Example reset timer

            return true;
        }
    }
}