using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Magic
{
    public class FireballSkill : BaseSkill
    {
        public FireballSkill()
        {
            Name = "Fireball";
            Description = "Launches a powerful fireball at enemies.";
            Cooldown = 90; // 1.5 seconds
            StaminaCost = 15f;
            Level = 1; // Unlocked by default
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply magic buff
            player.AddBuff(Terraria.ID.BuffID.ManaRegeneration, 300); // 5 seconds
            player.AddBuff(Terraria.ID.BuffID.MagicPower, 300);

            // Visual and sound effects
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Torch,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Orange);
            }
            SoundEngine.PlaySound(SoundID.Item8, player.position);

            return true;
        }
    }
} 