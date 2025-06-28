using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Wolfgodrpg.Common.Players;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGHooks : ModSystem
    {
        // Este arquivo agora serve principalmente para ganchos que não se encaixam em outros lugares.
    }

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
                }
            }

            // Verificar dano tomado para a classe de Regeneração
            if (Player.statLife < lastHealth)
            {
                int damageTaken = (int)(lastHealth - Player.statLife);
                RPGActionSystem.OnHurt(Player, damageTaken);
            }
            lastHealth = Player.statLife;

            // Verificar pulos para XP de Jumping
            if (Player.justJumped && Player.velocity.Y < 0)
            {
                jumpCount++;
                if (jumpCount >= 50) // XP a cada 50 pulos
                {
                    rpgPlayer.GainClassExp("jumping", 10f);
                    jumpCount = 0;
                }
            }
        }

        public override void PostBuyItem(Item item, int price, int shopId)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = price * 0.0001f;
            rpgPlayer.GainClassExp("merchant", System.Math.Max(0.1f, xpAmount));
        }

        public override void PostSellItem(Item item, int price, int shopId)
        {
            var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
            float xpAmount = price * 0.0001f;
            rpgPlayer.GainClassExp("merchant", System.Math.Max(0.1f, xpAmount));
        }

        public override void OnEnterWorld()
        {
            Main.NewText("Bem-vindo ao Wolf God RPG!", new Color(220, 180, 10));
            Main.NewText("Pressione 'M' para o menu RPG e 'R' para stats.", Color.Cyan);
        }

        // Hook para dar XP ao colher plantas
        public override bool PreKillTile(int i, int j, int type, ref bool fail)
        {
            Tile tile = Main.tile[i, j];
            if (tile != null && tile.HasTile)
            {
                // Se for uma planta madura (ex: girassol, daybloom)
                if (TileID.Sets.HarvestablePlants[type])
                {
                    var rpgPlayer = Player.GetModPlayer<RPGPlayer>();
                    rpgPlayer.GainClassExp("farming", 5f); // XP por colher
                }
            }
            return base.PreKillTile(i, j, type, ref fail);
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
 