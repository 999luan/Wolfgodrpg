using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.GlobalItems;
using System;

namespace Wolfgodrpg.Common.Systems
{
    public class RPGDebugSystem : ModSystem
    {
        private const int DEBUG_INTERVAL = 300; // 5 segundos (60 FPS * 5)

        public override void PostUpdateWorld()
        {
            // TEMPORARIAMENTE DESABILITADO PARA EVITAR SPAM DE LOGS
            /*
            // debugCounter++;
            
            // if (debugCounter >= DEBUG_INTERVAL)
            // {
            //     debugCounter = 0;
            //     RunDebugChecks();
            // }
            */
        }

        private void RunDebugChecks()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            
            // Debug: Verificar se as subclasses estão sendo inicializadas
            DebugLog.System("DebugCheck", $"=== DEBUG CHECK ===");
            DebugLog.System("DebugCheck", $"Player: {player.name}");
            DebugLog.System("DebugCheck", $"SubClasses.Count: {rpgPlayer.SubClasses.SubClasses.Count}");
            
            if (rpgPlayer.SubClasses.SubClasses.Count > 0)
            {
                DebugLog.System("DebugCheck", $"Available subclasses: {string.Join(", ", rpgPlayer.SubClasses.SubClasses.Select(sc => sc.Name))}");
                foreach (var subClass in rpgPlayer.SubClasses.SubClasses)
                {
                    DebugLog.System("DebugCheck", $"  {subClass.Name}: Level {subClass.Level}, XP {subClass.XP}");
                }
            }
            else
            {
                DebugLog.Warn("System", "DebugCheck", "SubClasses is empty!");
            }

            // Debug: Verificar se os itens estão funcionando
            var inventory = player.inventory;
            int itemsWithXP = 0;
            int totalItems = 0;
            
            for (int i = 0; i < inventory.Length; i++)
            {
                var item = inventory[i];
                if (item != null && !item.IsAir)
                {
                    totalItems++;
                    var progressiveItem = item.GetGlobalItem<ProgressiveItem>();
                    if (progressiveItem != null && progressiveItem.Experience > 0)
                    {
                        itemsWithXP++;
                        DebugLog.System("DebugCheck", $"Item com XP: {item.Name} - Level {progressiveItem.GetItemLevel()}, XP {progressiveItem.Experience:F1}");
                    }
                }
            }
            
            DebugLog.System("DebugCheck", $"Items in inventory: {totalItems}, Items with XP: {itemsWithXP}");
            DebugLog.System("DebugCheck", $"=== FIM DEBUG CHECK ===");
        }

        // Comando de debug para testar ganho de XP
        public override void PostUpdatePlayers()
        {
            // Teste manual de ganho de XP (remover depois)
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                var player = Main.LocalPlayer;
                if (player?.active == true)
                {
                    var rpgPlayer = player.GetModPlayer<RPGPlayer>();
                    rpgPlayer.AddClassExperience("warrior", (int)50f);
                    DebugLog.Gameplay("Debug", "ManualTest", "XP manual de warrior concedido via F1");
                }
            }
        }
    }
}