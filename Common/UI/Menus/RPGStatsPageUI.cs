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
    // Aba de Status simplificada e organizada
    public class RPGStatsPageUI : UIElement
    {
        private UIList _statsList;
        private UIScrollbar _statsScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de stats com scrollbar
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
            
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _statsList.Add(new UIText("Jogador não disponível."));
                return;
            }

            var player = modPlayer.Player;
            var stats = RPGCalculations.CalculateTotalStats(modPlayer);

            // === SEÇÃO PRINCIPAL: CLASSES E PROGRESSO ===
            _statsList.Add(new StatsSectionHeader("🎯 CLASSES E PROGRESSO", Color.Gold));
            
            if (RPGClassDefinitions.ActionClasses != null)
            {
                foreach (var classEntry in RPGClassDefinitions.ActionClasses)
                {
                    string classKey = classEntry.Key;
                    var classInfo = classEntry.Value;
                    if (classInfo == null) continue;

                    float level = modPlayer.ClassLevels.TryGetValue(classKey, out var lvl) ? lvl : 0f;
                    float currentExp = 0;
                    modPlayer.ClassExperience.TryGetValue(classKey, out currentExp);
                    float nextLevelExp = 100 * (level + 1);
                    float progressPercent = nextLevelExp > 0 ? (currentExp / nextLevelExp * 100f) : 0f;
                    
                    // Entrada principal da classe
                    _statsList.Add(new ClassEntry(classInfo.Name, level, currentExp, nextLevelExp, progressPercent, classInfo.Milestones, level));
                }
            }

            // === SEÇÃO SECUNDÁRIA: VITALS ===
            _statsList.Add(new StatsSectionHeader("💓 VITALS", Color.LightCoral));
            _statsList.Add(new StatsEntry($"❤️ Vida: {player.statLife} / {player.statLifeMax2}", GetVitalColor(modPlayer.CurrentHunger)));
            _statsList.Add(new StatsEntry($"🍖 Fome: {modPlayer.CurrentHunger:F1}%", GetVitalColor(modPlayer.CurrentHunger)));
            _statsList.Add(new StatsEntry($"🧠 Sanidade: {modPlayer.CurrentSanity:F1}%", GetVitalColor(modPlayer.CurrentSanity)));
            _statsList.Add(new StatsEntry($"⚡ Stamina: {modPlayer.CurrentStamina:F1}%", GetVitalColor(modPlayer.CurrentStamina)));

            // === SEÇÃO TERCIÁRIA: STATS BÁSICOS ===
            _statsList.Add(new StatsSectionHeader("⚔️ STATS BÁSICOS", Color.LightBlue));
            _statsList.Add(new StatsEntry($"🛡️ Defesa: {player.statDefense}"));
            _statsList.Add(new StatsEntry($"🏃 Velocidade: {player.moveSpeed:F2}"));
            _statsList.Add(new StatsEntry($"🔮 Mana: {player.statMana} / {player.statManaMax2}"));
        }

        // Cor baseada no valor do vital
        private Color GetVitalColor(float value)
        {
            if (value > 70f) return Color.LightGreen;
            if (value > 30f) return Color.Yellow;
            return Color.Red;
        }

        // Cabeçalho de seção
        private class StatsSectionHeader : UIElement
        {
            public StatsSectionHeader(string title, Color color)
            {
                Width.Set(0, 1f);
                Height.Set(30f, 0f);
                
                var text = new UIText(title, 1.0f, true);
                text.TextColor = color;
                Append(text);
            }
        }

        // Entrada de stat individual
        private class StatsEntry : UIElement
        {
            public StatsEntry(string statText, Color? color = null)
            {
                Width.Set(0, 1f);
                Height.Set(22f, 0f);
                
                var text = new UIText(statText, 0.85f);
                text.TextColor = color ?? Color.White;
                Append(text);
            }
        }

        // Entrada de classe com progresso visual
        private class ClassEntry : UIElement
        {
            public ClassEntry(string className, float level, float currentExp, float nextLevelExp, float progressPercent, Dictionary<ClassAbility, string> milestones, float currentLevel)
            {
                Width.Set(0, 1f);
                Height.Set(60f, 0f);
                SetPadding(3);

                // Nome da classe e nível
                var headerText = new UIText($"{className} - Nv.{level:F0}", 0.9f);
                headerText.TextColor = Color.Gold;
                Append(headerText);

                // Barra de progresso visual
                var progressText = new UIText($"XP: {currentExp:F0}/{nextLevelExp:F0} ({progressPercent:F1}%)", 0.75f);
                progressText.Top.Set(18f, 0f);
                progressText.TextColor = progressPercent >= 80f ? Color.LightGreen : Color.LightBlue;
                Append(progressText);

                // Próxima habilidade
                if (milestones != null)
                {
                    var nextMilestone = milestones.FirstOrDefault(m => (int)m.Key > currentLevel);
                    if (!string.IsNullOrEmpty(nextMilestone.Value))
                    {
                        int levelsNeeded = (int)nextMilestone.Key - (int)currentLevel;
                        var nextText = new UIText($"Próximo: {nextMilestone.Value} (Nv.{(int)nextMilestone.Key})", 0.7f);
                        nextText.Top.Set(35f, 0f);
                        nextText.TextColor = Color.Yellow;
                        Append(nextText);
                    }
                }
            }
        }
    }
}
