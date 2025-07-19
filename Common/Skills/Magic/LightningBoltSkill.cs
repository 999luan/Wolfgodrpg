using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Magic
{
    public class LightningBoltSkill : BaseSkill
    {
        public LightningBoltSkill()
        {
            Name = "Lightning Bolt";
            Description = "Summons a powerful lightning bolt.";
            Cooldown = 180; // 3 seconds
            StaminaCost = 25f;
            Level = 0; // Unlocked at level 5
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply lightning buff
            player.AddBuff(Terraria.ID.BuffID.ManaRegeneration, 300); // 5 seconds
            player.AddBuff(Terraria.ID.BuffID.MagicPower, 300);

            // Visual and sound effects
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 0, Color.Yellow);
            }
            SoundEngine.PlaySound(SoundID.Item10, player.position);

            return true;
        }
    }
} 