using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures; // Adicionado para FishingAttempt e AdvancedPopupRequest
using Wolfgodrpg.Common.Players;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGPlayerHooks : ModPlayer
    {
        private float lastHealth;
        private int jumpCount = 0;

        public override void Initialize()
        {
            lastHealth = Player.statLife;
        }

        public override void PreUpdate()
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();

            // --- LÓGICA DE DASH COM STAMINA ---
            if (Player.dash > 0 && Player.dashDelay == 0)
            {
                float staminaCost = 25f; // Custo base de stamina para o dash
                if (!rpgPlayer.ConsumeStamina(staminaCost))
                {
                    Player.dash = 0; // Cancela o dash se não houver stamina
                    DebugLog.Gameplay("Player", "Dash", $"Dash cancelado por falta de stamina");
                }
                else
                {
                    DebugLog.Gameplay("Player", "Dash", $"Dash executado, stamina consumida: {staminaCost}");
                }
            }

            // Dash vertical manual (duplo-toque para cima/baixo)
            if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[0] > 0 && Player.doubleTapCardinalTimer[0] < 15)
            {
                float staminaCost = 25f;
                if (rpgPlayer.ConsumeStamina(staminaCost))
                {
                    Player.velocity.Y = -Player.jumpSpeed * 2f;
                    DebugLog.Gameplay("Player", "Dash", "Dash vertical para cima executado");
                }
            }
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[1] > 0 && Player.doubleTapCardinalTimer[1] < 15)
            {
                float staminaCost = 25f;
                if (rpgPlayer.ConsumeStamina(staminaCost))
                {
                    Player.velocity.Y = Player.jumpSpeed * 2f;
                    DebugLog.Gameplay("Player", "Dash", "Dash vertical para baixo executado");
                }
            }

            // Verificar dano tomado para a classe de Regeneração
            if (Player.statLife < lastHealth)
            {
                int damageTaken = (int)(lastHealth - Player.statLife);
                RPGActionSystem.OnHurt(Player, damageTaken);
                
                // Resetar timer de combate quando toma dano
                rpgPlayer.CombatTimer = 0;
                DebugLog.Gameplay("Player", "PreUpdate", $"Jogador tomou {damageTaken} de dano - timer de combate resetado");
            }
            lastHealth = Player.statLife;

            // Verificar pulos para XP de Jumping
            if (Player.justJumped && Player.velocity.Y < 0)
            {
                jumpCount++;
                DebugLog.Gameplay("Player", "Jumping", $"Pulo detectado. Contador: {jumpCount}/50");
                if (jumpCount >= 50) // XP a cada 50 pulos
                {
                    rpgPlayer.GainClassExp("jumping", 10f);
                    DebugLog.Gameplay("Player", "Jumping", "XP de pulo concedido!");
                    jumpCount = 0;
                }
            }
            
            // Detectar combate baseado em ações do jogador
            if (Player.itemAnimation > 0 || Player.velocity.Length() > 2f)
            {
                rpgPlayer.CombatTimer = 0; // Resetar timer quando está ativo
            }
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = item.value * 0.0001f;
            rpgPlayer.GainClassExp("merchant", System.Math.Max(0.1f, xpAmount));
        }

        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = item.value * 0.0001f;
            rpgPlayer.GainClassExp("merchant", System.Math.Max(0.1f, xpAmount));
        }

        public override void OnEnterWorld()
        {
            Main.NewText("Bem-vindo ao Wolf God RPG!", new Color(220, 180, 10));
            Main.NewText("Pressione 'M' para o menu RPG e 'R' para stats.", Color.Cyan);
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = (attempt.rare ? 1f : 0f) * 10f; 
            xpAmount = System.Math.Max(1f, xpAmount); 
            rpgPlayer.GainClassExp("fishing", xpAmount);
        }
    }

    public class RPGGlobalNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (npc.lifeMax > 5 && !npc.friendly)
            {
                RPGActionSystem.OnKillNPC(npc);
            }
        }
    }
}

 