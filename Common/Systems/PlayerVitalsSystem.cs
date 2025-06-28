using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Systems
{
    public class PlayerVitalsSystem : ModSystem
    {
        // Constantes de controle
        private const float HUNGER_DECAY_RATE = 0.01f; // Por segundo
        private const float SANITY_REGEN_RATE = 0.05f; // Por segundo, durante o dia
        private const float SANITY_LOSS_RATE = 0.1f;  // Por segundo, em combate prolongado
        private const float STAMINA_REGEN_RATE = 15f; // Por segundo
        private const int COMBAT_TIMER_THRESHOLD = 180; // 3 minutos em segundos (3 * 60)

        public override void PostUpdatePlayers()
        {
            if (Main.gamePaused || !Main.LocalPlayer.active || Main.LocalPlayer.dead) return;

            var player = Main.LocalPlayer;
            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            var config = ModContent.GetInstance<RPGConfig>();

            // --- LÓGICA DE STAMINA ---
            if (rpgPlayer.StaminaRegenDelay > 0)
            {
                rpgPlayer.StaminaRegenDelay--;
            }
            else
            {
                rpgPlayer.CurrentStamina = System.Math.Min(rpgPlayer.MaxStamina, rpgPlayer.CurrentStamina + (STAMINA_REGEN_RATE / 60f));
            }

            // --- LÓGICA DE FOME ---
            if (config.EnableHunger)
            {
                rpgPlayer.CurrentHunger -= (HUNGER_DECAY_RATE / 60f) * config.HungerRate;
                if (rpgPlayer.CurrentHunger < 0) rpgPlayer.CurrentHunger = 0;

                // Penalidade: Sem regeneração de vida se a fome for muito baixa
                if (rpgPlayer.CurrentHunger < 20f)
                {
                    player.lifeRegen = 0;
                    player.lifeRegenTime = 0;
                }
            }

            // --- LÓGICA DE SANIDADE ---
            if (config.EnableSanity)
            {
                // Regenera de dia e fora de combate
                if (Main.dayTime && rpgPlayer.CombatTimer < COMBAT_TIMER_THRESHOLD)
                {
                    rpgPlayer.CurrentSanity += (SANITY_REGEN_RATE / 60f);
                }

                // Perde em combate prolongado
                if (rpgPlayer.CombatTimer >= COMBAT_TIMER_THRESHOLD)
                {
                    rpgPlayer.CurrentSanity -= (SANITY_LOSS_RATE / 60f) * config.SanityRate;
                }

                if (rpgPlayer.CurrentSanity < 0) rpgPlayer.CurrentSanity = 0;
                if (rpgPlayer.CurrentSanity > rpgPlayer.MaxSanity) rpgPlayer.CurrentSanity = rpgPlayer.MaxSanity;

                // Penalidades da sanidade baixa
                if (rpgPlayer.CurrentSanity < 30f)
                {
                    player.GetDamage(DamageClass.Generic) *= 0.8f; // 20% menos dano
                    if (Main.rand.NextBool(900)) // A cada ~15 segundos
                    {
                        player.AddBuff(Terraria.ID.BuffID.Confused, 240);
                    }
                }
            }

            // Atualiza o cronômetro de combate (aumenta fora de combate, reseta em combate)
            rpgPlayer.CombatTimer++;
        }
    }
}
