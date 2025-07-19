using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Combat
{
    public class RapidFireSkill : BaseSkill
    {
        public RapidFireSkill()
        {
            Name = "Rapid Fire";
            Description = "Increases attack speed for a short duration.";
            Cooldown = 180; // 3 seconds
            StaminaCost = 20f;
            Level = 0; // Unlocked at level 3
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply rapid fire buff
            player.AddBuff(Terraria.ID.BuffID.Swiftness, 600); // 10 seconds
            player.AddBuff(Terraria.ID.BuffID.AmmoBox, 600);

            // Visual and sound effects
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Torch,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, Color.Orange);
            }
            SoundEngine.PlaySound(SoundID.Item24, player.position);

            return true;
        }
    }
} 