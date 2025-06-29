using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Classes;

namespace Wolfgodrpg.Common.UI
{
    // Aba de Status do menu RPG
    public class RPGStatsPageUI : UIElement
    {
        private UIElement _statsContainer;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            _statsContainer = new UIElement();
            _statsContainer.Width.Set(0, 1f);
            _statsContainer.Height.Set(0, 1f);
            _statsContainer.SetPadding(10);
            Append(_statsContainer);
        }

        // Atualiza o conteúdo da aba de status
        public void UpdateStats(RPGPlayer modPlayer)
        {
            _statsContainer.RemoveAllChildren();
            if (modPlayer == null || modPlayer.Player == null) return;

            var player = modPlayer.Player;
            var stats = RPGCalculations.CalculateTotalStats(modPlayer);
            StringBuilder sb = new StringBuilder();

            // Informações básicas
            sb.AppendLine($"=== INFORMAÇÕES BÁSICAS ===");
            sb.AppendLine($"Vida: {player.statLife} / {player.statLifeMax2}");
            sb.AppendLine($"Mana: {player.statMana} / {player.statManaMax2}");
            sb.AppendLine($"Defesa: {player.statDefense}");
            sb.AppendLine($"Velocidade: {player.moveSpeed:F2}");
            sb.AppendLine("");

            // Vitals do RPG
            sb.AppendLine($"=== VITALS RPG ===");
            sb.AppendLine($"Fome: {modPlayer.CurrentHunger:F1}% / {modPlayer.MaxHunger:F1}%");
            sb.AppendLine($"Sanidade: {modPlayer.CurrentSanity:F1}% / {modPlayer.MaxSanity:F1}%");
            sb.AppendLine($"Stamina: {modPlayer.CurrentStamina:F1}% / {modPlayer.MaxStamina:F1}%");
            sb.AppendLine("");

            // Classes e experiência
            sb.AppendLine($"=== CLASSES ===");
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string classKey = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(classKey);
                float currentExp = 0;
                modPlayer.ClassExperience.TryGetValue(classKey, out currentExp);
                float nextLevelExp = 100 * (level + 1);
                sb.AppendLine($"{classInfo.Name}: Nv.{level:F0} XP:{currentExp:F0}/{nextLevelExp:F0}");
            }
            sb.AppendLine("");

            // Stats calculados
            if (stats.Count > 0)
            {
                sb.AppendLine($"=== BÔNUS DE EQUIPAMENTOS ===");
                foreach (var stat in stats)
                {
                    string statName = GetStatDisplayName(stat.Key);
                    sb.AppendLine($"{statName}: +{stat.Value:F1}");
                }
            }

            var text = new UIText(sb.ToString());
            text.Width.Set(0, 1f);
            text.Height.Set(0, 1f);
            _statsContainer.Append(text);
        }

        // Nome amigável para cada stat
        private string GetStatDisplayName(string statKey)
        {
            return statKey switch
            {
                "meleeDamage" => "Dano Corpo a Corpo",
                "rangedDamage" => "Dano à Distância",
                "magicDamage" => "Dano Mágico",
                "minionDamage" => "Dano de Servos",
                "critChance" => "Chance Crítica",
                "meleeCrit" => "Crítico Corpo a Corpo",
                "rangedCrit" => "Crítico à Distância",
                "magicCrit" => "Crítico Mágico",
                "meleeSpeed" => "Velocidade Corpo a Corpo",
                "defense" => "Defesa",
                "maxLife" => "Vida Máxima",
                "lifeRegen" => "Regeneração de Vida",
                "damageReduction" => "Redução de Dano",
                "moveSpeed" => "Velocidade de Movimento",
                "jumpHeight" => "Altura do Pulo",
                "maxMana" => "Mana Máxima",
                "manaRegen" => "Regeneração de Mana",
                "manaCost" => "Redução de Custo de Mana",
                "minionSlots" => "Slots de Servos",
                "miningSpeed" => "Velocidade de Mineração",
                "luck" => "Sorte",
                "expGain" => "Ganho de Experiência",
                _ => statKey
            };
        }
    }
}
