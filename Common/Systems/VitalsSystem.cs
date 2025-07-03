using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Systems
{
    public class VitalsSystem : ModSystem
    {
        // Constantes de controle
        private const float HUNGER_DECAY_RATE = 0.01f; // Por segundo
        private const float SANITY_REGEN_RATE = 0.05f; // Por segundo, durante o dia
        private const float SANITY_LOSS_RATE = 0.1f;  // Por segundo, em combate prolongado
        private const float STAMINA_REGEN_RATE = 15f; // Por segundo
        private const int COMBAT_TIMER_THRESHOLD = 180; // 3 minutos em segundos (3 * 60)

        public override void PostUpdateEverything()
        {
            if (!Main.gameMenu)
            {
                foreach (var player in Main.player)
                {
                    if (player.active && !player.dead)
                    {
                        var modPlayer = player.GetModPlayer<RPGPlayer>();
                        var config = ModContent.GetInstance<ConfigSystem>();

                        // --- LÓGICA DE STAMINA ---
                        if (modPlayer.StaminaRegenDelay > 0)
                        {
                            modPlayer.StaminaRegenDelay--;
                        }
                        else
                        {
                            float oldStamina = modPlayer.CurrentStamina;
                            modPlayer.CurrentStamina = System.Math.Min(modPlayer.MaxStamina, modPlayer.CurrentStamina + (STAMINA_REGEN_RATE / 60f));
                            if (modPlayer.CurrentStamina > oldStamina)
                            {
                                DebugLog.Player("PostUpdatePlayers", $"Stamina regenerada: {oldStamina:F1} -> {modPlayer.CurrentStamina:F1}");
                            }
                        }

                        // --- LÓGICA DE FOME ---
                        if (config.EnableHunger)
                        {
                            float oldHunger = modPlayer.CurrentHunger;
                            modPlayer.CurrentHunger -= (HUNGER_DECAY_RATE / 60f) * config.HungerRate;
                            if (modPlayer.CurrentHunger < 0) modPlayer.CurrentHunger = 0;

                            // Penalidade: Sem regeneração de vida se a fome for muito baixa
                            if (modPlayer.CurrentHunger < 20f)
                            {
                                player.lifeRegen = 0;
                                player.lifeRegenTime = 0;
                            }
                            
                            if (modPlayer.CurrentHunger < oldHunger)
                            {
                                DebugLog.Player("PostUpdatePlayers", $"Fome diminuída: {oldHunger:F1} -> {modPlayer.CurrentHunger:F1}");
                            }
                        }

                        // --- LÓGICA DE SANIDADE ---
                        if (config.EnableSanity)
                        {
                            float oldSanity = modPlayer.CurrentSanity;
                            
                            // Regenera de dia e fora de combate
                            if (Main.dayTime && modPlayer.CombatTimer > COMBAT_TIMER_THRESHOLD)
                            {
                                modPlayer.CurrentSanity += (SANITY_REGEN_RATE / 60f);
                                DebugLog.Player("PostUpdatePlayers", $"Sanidade regenerando: {oldSanity:F1} -> {modPlayer.CurrentSanity:F1} (fora de combate)");
                            }

                            // Perde em combate prolongado
                            if (modPlayer.CombatTimer >= COMBAT_TIMER_THRESHOLD)
                            {
                                modPlayer.CurrentSanity -= (SANITY_LOSS_RATE / 60f) * config.SanityRate;
                                DebugLog.Player("PostUpdatePlayers", $"Sanidade diminuindo: {oldSanity:F1} -> {modPlayer.CurrentSanity:F1} (em combate)");
                            }

                            if (modPlayer.CurrentSanity < 0) modPlayer.CurrentSanity = 0;
                            if (modPlayer.CurrentSanity > modPlayer.MaxSanity) modPlayer.CurrentSanity = modPlayer.MaxSanity;

                            // Penalidades da sanidade baixa
                            if (modPlayer.CurrentSanity < 30f)
                            {
                                player.GetDamage(DamageClass.Generic) *= 0.8f; // 20% menos dano
                                if (Main.rand.NextBool(900)) // A cada ~15 segundos
                                {
                                    player.AddBuff(Terraria.ID.BuffID.Confused, 240);
                                    DebugLog.Player("PostUpdatePlayers", "Sanidade baixa: jogador confuso");
                                }
                            }
                        }

                        // Atualiza o cronômetro de combate (aumenta fora de combate, reseta em combate)
                        modPlayer.CombatTimer++;
                        
                        // Log periódico do estado dos vitals
                        if (Main.GameUpdateCount % 3600 == 0) // A cada 60 segundos
                        {
                            DebugLog.Player("PostUpdatePlayers", $"Estado dos vitals - Hunger: {modPlayer.CurrentHunger:F1}%, Sanity: {modPlayer.CurrentSanity:F1}%, Stamina: {modPlayer.CurrentStamina:F1}%, CombatTimer: {modPlayer.CombatTimer}");
                        }
                    }
                }
            }
        }
    }
}
