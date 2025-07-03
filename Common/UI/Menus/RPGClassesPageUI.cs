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
using Wolfgodrpg.Common.Classes;
using System.Linq;

namespace Wolfgodrpg.Common.UI.Menus
{
    // Aba de Classes do menu RPG (Padrão ExampleMod)
    public class RPGClassesPageUI : UIElement
    {
        private UIList _classesList;
        private UIScrollbar _classesScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de classes com scrollbar (padrão ExampleMod)
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
            
            // Verificações robustas de null (padrão ExampleMod)
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

            // Layout adaptativo: se há muitas classes (>6), usar layout compacto
            bool useCompactLayout = RPGClassDefinitions.ActionClasses.Count > 6;
            float entryHeight = useCompactLayout ? 60f : 80f;

            bool foundClasses = false;
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                var classInfo = classEntry.Value;
                if (classInfo == null) continue;

                float level = modPlayer.ClassLevels.TryGetValue(classEntry.Key, out var lvl) ? lvl : 0f;
                float currentExp = 0;
                modPlayer.ClassExperience.TryGetValue(classEntry.Key, out currentExp);
                float nextLevelExp = 100 * (level + 1);
                
                _classesList.Add(new ClassEntry(classInfo.Name, level, currentExp, nextLevelExp, classInfo.Milestones, classInfo.StatBonuses, useCompactLayout));
                foundClasses = true;
            }

            if (!foundClasses)
            {
                _classesList.Add(new UIText("Nenhuma classe encontrada."));
            }
        }

        // Elemento visual para cada classe (padrão ExampleMod)
        private class ClassEntry : UIElement
        {
            public ClassEntry(string className, float level, float currentExp, float nextLevelExp, Dictionary<ClassAbility, string> milestones, Dictionary<string, float> statBonuses, bool compact)
            {
                Width.Set(0, 1f);
                Height.Set(compact ? 80f : 140f, 0f); // Aumentar altura para mais informações
                SetPadding(5);

                // Informações principais da classe
                var text = new UIText($"{className}: Nv.{level:F0} XP:{currentExp:F0}/{nextLevelExp:F0}", compact ? 0.8f : 0.9f);
                text.TextColor = Color.Gold;
                Append(text);
                
                float yOffset = compact ? 15f : 20f;

                // === STATS GANHOS POR NÍVEL (SEMPRE MOSTRAR) ===
                if (statBonuses != null && statBonuses.Count > 0)
                {
                    var statsHeaderText = new UIText("📊 Bônus por Nível:", compact ? 0.6f : 0.7f);
                    statsHeaderText.Top.Set(yOffset, 0f);
                    statsHeaderText.TextColor = Color.Cyan;
                    Append(statsHeaderText);
                    yOffset += compact ? 10f : 12f;
                    
                    int statsShown = 0;
                    foreach (var stat in statBonuses)
                    {
                        if (statsShown >= (compact ? 2 : 4)) break; // Limitar stats mostrados
                        
                        string statName = GetStatDisplayName(stat.Key);
                        float bonusPerLevel = stat.Value * 100f; // Converter para porcentagem
                        float totalBonus = bonusPerLevel * level; // Bônus total atual
                        
                        var statText = new UIText($"  +{bonusPerLevel:F1}% {statName} (Total: +{totalBonus:F1}%)", compact ? 0.55f : 0.65f);
                        statText.Top.Set(yOffset, 0f);
                        statText.TextColor = Color.LightGreen;
                        Append(statText);
                        yOffset += compact ? 8f : 10f;
                        statsShown++;
                    }
                    yOffset += compact ? 3f : 5f;
                }

                // === HABILIDADES DESBLOQUEADAS E PRÓXIMAS ===
                if (milestones != null && milestones.Count > 0)
                {
                    // Cabeçalho de habilidades
                    var abilitiesHeaderText = new UIText("🎯 Habilidades:", compact ? 0.6f : 0.7f);
                    abilitiesHeaderText.Top.Set(yOffset, 0f);
                    abilitiesHeaderText.TextColor = Color.Orange;
                    Append(abilitiesHeaderText);
                    yOffset += compact ? 10f : 12f;
                    
                    // Mostrar habilidades já desbloqueadas (últimas 2)
                    var unlockedMilestones = milestones.Where(m => (int)m.Key <= level).OrderBy(m => m.Key).ToList();
                    if (unlockedMilestones.Any())
                    {
                        foreach (var milestone in unlockedMilestones.TakeLast(compact ? 1 : 2))
                        {
                            string abilityText = milestone.Value;
                            // Dash aparece como "—" no nível 1 (requisito do checklist)
                            if ((int)milestone.Key == 1 && abilityText.ToLower().Contains("dash"))
                            {
                                abilityText = "—";
                            }
                            
                            var unlockedText = new UIText($"  ✅ Nv.{(int)milestone.Key}: {abilityText}", compact ? 0.55f : 0.6f);
                            unlockedText.Top.Set(yOffset, 0f);
                            unlockedText.TextColor = Color.LightGreen;
                            Append(unlockedText);
                            yOffset += compact ? 8f : 10f;
                        }
                    }
                    
                    // Mostrar próxima habilidade
                    var nextMilestone = milestones.FirstOrDefault(m => (int)m.Key > level);
                    if (!string.IsNullOrEmpty(nextMilestone.Value))
                    {
                        string abilityText = nextMilestone.Value;
                        int levelsNeeded = (int)nextMilestone.Key - (int)level;
                        
                        var nextText = new UIText($"  🔒 Nv.{(int)nextMilestone.Key}: {abilityText} ({levelsNeeded} níveis)", compact ? 0.55f : 0.6f);
                        nextText.Top.Set(yOffset, 0f);
                        nextText.TextColor = Color.Yellow;
                        Append(nextText);
                        yOffset += compact ? 8f : 10f;
                    }
                    
                    yOffset += compact ? 3f : 5f;
                }

                // === BARRA DE PROGRESSO VISUAL ===
                float progressPercent = (currentExp / nextLevelExp * 100f);
                var progressText = new UIText($"📈 Progresso: {progressPercent:F1}%", compact ? 0.6f : 0.7f);
                progressText.Top.Set(yOffset, 0f);
                progressText.TextColor = progressPercent >= 80f ? Color.Gold : Color.LightBlue;
                Append(progressText);
                
                // Mostrar XP necessário para próximo nível
                if (!compact)
                {
                    float xpNeeded = nextLevelExp - currentExp;
                    var xpNeededText = new UIText($"XP para próximo nível: {xpNeeded:F0}", 0.6f);
                    xpNeededText.Top.Set(yOffset + 12f, 0f);
                    xpNeededText.TextColor = Color.Cyan;
                    Append(xpNeededText);
                }
            }
            
            // Nome amigável para cada stat
            private string GetStatDisplayName(string statKey)
            {
                return statKey switch
                {
                    "moveSpeed" => "Velocidade",
                    "acceleration" => "Aceleração", 
                    "jumpHeight" => "Altura do Pulo",
                    "fallDamageReduction" => "Redução Dano Queda",
                    "staminaRegen" => "Regen. Stamina",
                    "meleeDamage" => "Dano Corpo a Corpo",
                    "rangedDamage" => "Dano à Distância",
                    "magicDamage" => "Dano Mágico",
                    "minionDamage" => "Dano de Servos",
                    "critChance" => "Chance Crítica",
                    "meleeCrit" => "Crítico Corpo a Corpo",
                    "rangedCrit" => "Crítico à Distância", 
                    "magicCrit" => "Crítico Mágico",
                    "meleeSpeed" => "Vel. Ataque",
                    "defense" => "Defesa",
                    "maxLife" => "Vida Máxima",
                    "lifeRegen" => "Regen. Vida",
                    "miningSpeed" => "Vel. Mineração",
                    "buildSpeed" => "Vel. Construção",
                    "fishingPower" => "Poder de Pesca",
                    "gatherSpeed" => "Vel. Coleta",
                    "catchRate" => "Taxa de Captura",
                    "rareFish" => "Peixe Raro",
                    "crateLuck" => "Sorte de Baú",
                    "gemFind" => "Encontrar Gemas",
                    "oreQuality" => "Qualidade Minério",
                    "materialSave" => "Economizar Material",
                    "buildQuality" => "Qualidade Construção",
                    "doubleHarvest" => "Coleta Dupla",
                    "rareFind" => "Encontrar Raro",
                    "manaCost" => "Redução Custo Mana",
                    "manaRegen" => "Regen. Mana",
                    "maxMana" => "Mana Máxima",
                    "minionSlots" => "Slots de Servos",
                    "minionKnockback" => "Knockback Servos",
                    "minionRange" => "Alcance Servos",
                    "ammoSave" => "Economizar Munição",
                    "rangedSpeed" => "Vel. Projétil",
                    "lifeSteal" => "Roubo de Vida",
                    "meleeKnockback" => "Knockback Corpo a Corpo",
                    _ => statKey
                };
            }
        }
    }
}
