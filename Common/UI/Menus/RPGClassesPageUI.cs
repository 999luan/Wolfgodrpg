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

namespace Wolfgodrpg.Common.UI.Menus
{
    // Aba de Classes focada em detalhes e habilidades
    public class RPGClassesPageUI : UIElement
    {
        private UIList _classesList;
        private UIScrollbar _classesScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de classes com scrollbar
            _classesList = new UIList();
            _classesList.Width.Set(-25f, 1f);
            _classesList.Height.Set(0, 1f);
            _classesList.ListPadding = 5f;
            Append(_classesList);

            // Scrollbar acoplado à lista
            _classesScrollbar = new UIScrollbar();
            _classesScrollbar.SetView(100f, 1000f);
            _classesScrollbar.Height.Set(0, 1f);
            _classesScrollbar.HAlign = 1f;
            _classesList.SetScrollbar(_classesScrollbar);
            Append(_classesScrollbar);
        }

        // Atualiza o conteúdo da aba de classes
        public void UpdateClasses(RPGPlayer modPlayer)
        {
            _classesList.Clear();
            
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _classesList.Add(new UIText("Jogador não disponível."));
                return;
            }

            if (RPGClassDefinitions.ActionClasses == null || RPGClassDefinitions.ActionClasses.Count == 0)
            {
                _classesList.Add(new UIText("Nenhuma classe definida."));
                return;
            }

            // Cabeçalho
            _classesList.Add(new UIText("🎯 DETALHES DAS CLASSES", 1.1f, true) { TextColor = Color.Gold });

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                var classInfo = classEntry.Value;
                if (classInfo == null) continue;

                float level = modPlayer.ClassLevels.TryGetValue(classEntry.Key, out var lvl) ? lvl : 0f;
                
                _classesList.Add(new DetailedClassEntry(classInfo.Name, level, classInfo.Milestones, classInfo.StatBonuses));
            }
        }

        // Entrada detalhada de classe
        private class DetailedClassEntry : UIElement
        {
            public DetailedClassEntry(string className, float level, Dictionary<ClassAbility, string> milestones, Dictionary<string, float> statBonuses)
            {
                Width.Set(0, 1f);
                Height.Set(120f, 0f);
                SetPadding(5);

                // Nome da classe
                var titleText = new UIText($"{className} - Nível {level:F0}", 1.0f, true);
                titleText.TextColor = Color.Gold;
                Append(titleText);

                float yOffset = 25f;

                // === HABILIDADES DESBLOQUEADAS ===
                if (milestones != null && milestones.Count > 0)
                {
                    var abilitiesHeader = new UIText("✅ Habilidades Desbloqueadas:", 0.8f);
                    abilitiesHeader.Top.Set(yOffset, 0f);
                    abilitiesHeader.TextColor = Color.LightGreen;
                    Append(abilitiesHeader);
                    yOffset += 18f;

                    var unlockedMilestones = milestones.Where(m => (int)m.Key <= level).OrderBy(m => m.Key).ToList();
                    foreach (var milestone in unlockedMilestones)
                    {
                        string abilityText = milestone.Value;
                        if ((int)milestone.Key == 1 && abilityText.ToLower().Contains("dash"))
                        {
                            abilityText = "—";
                        }
                        
                        var abilityTextElement = new UIText($"  Nv.{(int)milestone.Key}: {abilityText}", 0.75f);
                        abilityTextElement.Top.Set(yOffset, 0f);
                        abilityTextElement.TextColor = Color.LightGreen;
                        Append(abilityTextElement);
                        yOffset += 15f;
                    }
                }

                // === BÔNUS DE STATS ===
                if (statBonuses != null && statBonuses.Count > 0)
                {
                    var statsHeader = new UIText("📊 Bônus por Nível:", 0.8f);
                    statsHeader.Top.Set(yOffset, 0f);
                    statsHeader.TextColor = Color.Cyan;
                    Append(statsHeader);
                    yOffset += 18f;

                    foreach (var stat in statBonuses)
                    {
                        string statName = GetStatDisplayName(stat.Key);
                        float bonusPerLevel = stat.Value * 100f;
                        float totalBonus = bonusPerLevel * level;
                        
                        var statText = new UIText($"  +{bonusPerLevel:F1}% {statName} (Total: +{totalBonus:F1}%)", 0.7f);
                        statText.Top.Set(yOffset, 0f);
                        statText.TextColor = Color.LightBlue;
                        Append(statText);
                        yOffset += 12f;
                    }
                }
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
                    "defense" => "Defesa",
                    "maxLife" => "Vida Máxima",
                    "moveSpeed" => "Velocidade",
                    "maxMana" => "Mana Máxima",
                    "miningSpeed" => "Mineração",
                    "luck" => "Sorte",
                    _ => statKey
                };
            }
        }
    }
}
