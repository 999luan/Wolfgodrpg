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
    // Aba de Status do menu RPG (Padrão ExampleMod)
    public class RPGStatsPageUI : UIElement
    {
        private UIList _statsList;
        private UIScrollbar _statsScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de stats com scrollbar (padrão ExampleMod)
            _statsList = new UIList();
            _statsList.Width.Set(-25f, 1f);
            _statsList.Height.Set(0, 1f);
            _statsList.ListPadding = 5f;
            Append(_statsList);

            // Scrollbar acoplado à lista
            _statsScrollbar = new UIScrollbar();
            _statsScrollbar.SetView(100f, 1000f);
            _statsScrollbar.Height.Set(0, 1f);
            _statsScrollbar.HAlign = 1f;
            _statsList.SetScrollbar(_statsScrollbar);
            Append(_statsScrollbar);
        }

        // Atualiza o conteúdo da aba de status
        public void UpdateStats(RPGPlayer modPlayer)
        {
            _statsList.Clear();
            
            // Verificações robustas de null (padrão ExampleMod)
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _statsList.Add(new UIText("Jogador não disponível."));
                DebugLog.UI("UpdateStats", "Jogador não disponível para exibir stats");
                return;
            }

            var player = modPlayer.Player;
            var stats = RPGCalculations.CalculateTotalStats(modPlayer);
            
            DebugLog.UI("UpdateStats", $"Stats calculados: {stats.Count} entradas");

            // Seção: Informações Básicas
            _statsList.Add(new StatsSectionHeader("INFORMAÇÕES BÁSICAS"));
            _statsList.Add(new StatsEntry($"Vida: {player.statLife} / {player.statLifeMax2}"));
            _statsList.Add(new StatsEntry($"Mana: {player.statMana} / {player.statManaMax2}"));
            _statsList.Add(new StatsEntry($"Defesa: {player.statDefense}"));
            _statsList.Add(new StatsEntry($"Velocidade: {player.moveSpeed:F2}"));

            // Seção: Vitals RPG
            _statsList.Add(new StatsSectionHeader("VITALS RPG"));
            _statsList.Add(new StatsEntry($"Fome: {modPlayer.CurrentHunger:F1}% / {modPlayer.MaxHunger:F1}%"));
            _statsList.Add(new StatsEntry($"Sanidade: {modPlayer.CurrentSanity:F1}% / {modPlayer.MaxSanity:F1}%"));
            _statsList.Add(new StatsEntry($"Stamina: {modPlayer.CurrentStamina:F1}% / {modPlayer.MaxStamina:F1}%"));

            // Seção: Classes e Habilidades
            _statsList.Add(new StatsSectionHeader("CLASSES E HABILIDADES"));
            if (RPGClassDefinitions.ActionClasses != null)
            {
                foreach (var classEntry in RPGClassDefinitions.ActionClasses)
                {
                    string classKey = classEntry.Key;
                    var classInfo = classEntry.Value;
                    if (classInfo == null) continue;

                    float level = modPlayer.GetClassLevel(classKey);
                    float currentExp = 0;
                    modPlayer.ClassExperience.TryGetValue(classKey, out currentExp);
                    float nextLevelExp = 100 * (level + 1);
                    
                    _statsList.Add(new StatsEntry($"{classInfo.Name}: Nv.{level:F0} XP:{currentExp:F0}/{nextLevelExp:F0}"));
                    
                    // Mostrar habilidades desbloqueadas
                    if (classInfo.Milestones != null)
                    {
                        foreach (var milestone in classInfo.Milestones)
                        {
                            if (level >= milestone.Key)
                            {
                                string abilityText = milestone.Value;
                                // Dash aparece como "—" no nível 1 conforme checklist
                                if (milestone.Key == 1 && abilityText.ToLower().Contains("dash"))
                                {
                                    abilityText = "—";
                                }
                                _statsList.Add(new StatsEntry($"  ✓ {abilityText}", Color.LightGreen));
                            }
                        }
                    }
                }
            }

            // Seção: Bônus de Equipamentos
            if (stats != null && stats.Count > 0)
            {
                _statsList.Add(new StatsSectionHeader("BÔNUS DE EQUIPAMENTOS"));
                foreach (var stat in stats)
                {
                    string statName = GetStatDisplayName(stat.Key);
                    _statsList.Add(new StatsEntry($"{statName}: +{stat.Value:F1}"));
                }
                DebugLog.UI("UpdateStats", $"Exibindo {stats.Count} bônus de equipamentos");
            }
            else
            {
                _statsList.Add(new StatsSectionHeader("BÔNUS DE EQUIPAMENTOS"));
                _statsList.Add(new StatsEntry("Nenhum bônus ativo"));
                DebugLog.UI("UpdateStats", "Nenhum bônus de equipamento encontrado");
            }
        }

        // Cabeçalho de seção
        private class StatsSectionHeader : UIElement
        {
            public StatsSectionHeader(string title)
            {
                Width.Set(0, 1f);
                Height.Set(25f, 0f);
                
                var text = new UIText($"=== {title} ===", 0.9f);
                text.TextColor = Color.Gold;
                Append(text);
            }
        }

        // Entrada de stat individual
        private class StatsEntry : UIElement
        {
            public StatsEntry(string statText, Color? color = null)
            {
                Width.Set(0, 1f);
                Height.Set(20f, 0f);
                
                var text = new UIText(statText, 0.8f);
                text.TextColor = color ?? Color.White;
                Append(text);
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
