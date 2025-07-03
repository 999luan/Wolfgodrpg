using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures; // Adicionado para FishingAttempt e AdvancedPopupRequest
using Wolfgodrpg.Common.Players;
using System.Collections.Generic;
using Wolfgodrpg.Common.Systems; // Added for RPGClassActionMapper

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

            // Sistema de dash agora está implementado no RPGPlayer.PreUpdate()
            // para seguir o padrão correto do tModLoader

            // Verificar dano tomado para a classe de Regeneração
            if (Player.statLife < lastHealth)
            {
                int damageTaken = (int)(lastHealth - Player.statLife);
                RPGActionSystem.OnHurt(Player, damageTaken);
                
                // Resetar timer de combate quando toma dano
                DebugLog.Gameplay("Player", "PreUpdate", $"Jogador tomou {damageTaken} de dano - timer de combate resetado");
            }
            lastHealth = Player.statLife;

            // Verificar pulos para XP de Acrobata
            if (Player.justJumped && Player.velocity.Y < 0)
            {
                jumpCount++;
                DebugLog.Gameplay("Player", "Jumping", $"Pulo detectado. Contador: {jumpCount}/50");
                if (jumpCount >= 50) // XP a cada 50 pulos
                {
                    RPGClassActionMapper.MapMovementAction(MovementAction.Jump, 50f);
                    DebugLog.Gameplay("Player", "Jumping", "XP de pulo concedido!");
                    jumpCount = 0;
                }
            }
            
            // Detectar combate baseado em ações do jogador
            if (Player.itemAnimation > 0 || Player.velocity.Length() > 2f)
            {
                DebugLog.Gameplay("Player", "PreUpdate", "Player is in combat.");
            }
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            RPGClassActionMapper.MapTradeAction(TradeAction.BuyItem, item.value);
        }

        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            RPGClassActionMapper.MapTradeAction(TradeAction.SellItem, item.value);
        }

        public override void OnEnterWorld()
        {
            Main.NewText("Bem-vindo ao Wolf God RPG!", new Color(220, 180, 10));
            Main.NewText("Pressione 'M' para o menu RPG e 'R' para stats.", Color.Cyan);
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            float xpAmount = (attempt.rare ? 1f : 0f) * 10f; 
            xpAmount = System.Math.Max(1f, xpAmount); 
            
            if (attempt.rare)
            {
                RPGClassActionMapper.MapFishingAction(FishingAction.CatchRareFish, xpAmount);
            }
            else
            {
                RPGClassActionMapper.MapFishingAction(FishingAction.CatchFish, xpAmount);
            }
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