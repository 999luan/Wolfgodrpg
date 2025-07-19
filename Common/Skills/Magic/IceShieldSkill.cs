using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Magic
{
    public class IceShieldSkill : BaseSkill
    {
        public IceShieldSkill()
        {
            Name = "Ice Shield";
            Description = "Creates a protective ice barrier.";
            Cooldown = 240; // 4 seconds
            StaminaCost = 20f;
            Level = 0; // Unlocked at level 3
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply ice shield buff
            player.AddBuff(Terraria.ID.BuffID.Ironskin, 600); // 10 seconds
            player.AddBuff(Terraria.ID.BuffID.Endurance, 600);

            // Visual and sound effects
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Ice,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, Color.Cyan);
            }
            SoundEngine.PlaySound(SoundID.Item25, player.position);

            return true;
        }
    }
} 