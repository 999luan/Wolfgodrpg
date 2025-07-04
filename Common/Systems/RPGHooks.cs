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

            // Verificar dano tomado para proficiência de armadura
            if (Player.statLife < lastHealth)
            {
                int damageTaken = (int)(lastHealth - Player.statLife);
                RPGActionSystem.OnHurt(Player, damageTaken);
                
                // Chamar método de proficiência de armadura
                if (rpgPlayer != null)
                {
                    rpgPlayer.OnPlayerDamaged(damageTaken);
                }
                
                // Resetar timer de combate quando toma dano
                DebugLog.Gameplay("Player", "PreUpdate", $"Player took {damageTaken} damage - combat timer reset");
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

    // Hook global para proficiência de armas
    public class RPGWeaponProficiencyHooks : GlobalItem
    {
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(item, player, target, hit, damageDone);
            
            // Chamar método de proficiência de arma
            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer != null)
            {
                rpgPlayer.OnHitNPC(item, target, hit, damageDone);
            }
        }
    }

    public class RPGProjectileProficiencyHooks : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);

            // Só processa se o projétil foi disparado por um player
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;
            Player player = Main.player[projectile.owner];
            if (player == null || !player.active)
                return;

            // Tenta identificar a arma usada para disparar o projétil
            Item heldItem = player.HeldItem;
            if (heldItem == null || heldItem.IsAir)
                return;

            // Só processa se for projétil de arma (não magia, não minion, etc)
            // Exemplo: Terra Blade é melee, mas dispara projétil
            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer != null)
            {
                // Detecta tipo de arma do item que disparou o projétil
                var weaponType = rpgPlayer.GetWeaponType(heldItem);
                if (weaponType != WeaponType.None)
                {
                    float xpGained = damageDone * 0.05f;
                    rpgPlayer.AddWeaponProficiencyXP(weaponType, xpGained);
                }
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