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
        private int debugCounter = 0;
        private const int DEBUG_INTERVAL = 300; // 5 segundos (60 FPS * 5)

        public override void PostUpdateWorld()
        {
            debugCounter++;
            
            if (debugCounter >= DEBUG_INTERVAL)
            {
                debugCounter = 0;
                RunDebugChecks();
            }
        }

        private void RunDebugChecks()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            
            // Debug: Verificar se as classes estão sendo inicializadas
            DebugLog.System("DebugCheck", $"=== DEBUG CHECK ===");
            DebugLog.System("DebugCheck", $"Jogador: {player.name}");
            DebugLog.System("DebugCheck", $"ClassLevels.Count: {rpgPlayer.ClassLevels.Count}");
            DebugLog.System("DebugCheck", $"ClassExperience.Count: {rpgPlayer.ClassExperience.Count}");
            
            if (rpgPlayer.ClassLevels.Count > 0)
            {
                DebugLog.System("DebugCheck", $"Classes disponíveis: {string.Join(", ", rpgPlayer.ClassLevels.Keys)}");
                foreach (var kvp in rpgPlayer.ClassLevels)
                {
                    float xp = rpgPlayer.ClassExperience.TryGetValue(kvp.Key, out float exp) ? exp : 0f;
                    DebugLog.System("DebugCheck", $"  {kvp.Key}: Level {kvp.Value:F1}, XP {xp:F1}");
                }
            }
            else
            {
                DebugLog.Warn("System", "DebugCheck", "ClassLevels está vazio!");
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
            
            DebugLog.System("DebugCheck", $"Itens no inventário: {totalItems}, Itens com XP: {itemsWithXP}");
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
                    rpgPlayer.GainClassExp("melee", 50f);
                    DebugLog.Gameplay("Debug", "ManualTest", "XP manual de melee concedido via F1");
                }
            }
        }
    }
}