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

namespace Wolfgodrpg.Common.UI
{
    // Aba de Progresso do menu RPG (Padrão ExampleMod)
    public class RPGProgressPageUI : UIElement
    {
        private UIList _progressList;
        private UIScrollbar _progressScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            
            // Lista de progresso com scrollbar (padrão ExampleMod)
            _progressList = new UIList();
            _progressList.Width.Set(-25f, 1f);
            _progressList.Height.Set(0, 1f);
            _progressList.ListPadding = 5f;
            Append(_progressList);

            // Scrollbar acoplado à lista
            _progressScrollbar = new UIScrollbar();
            _progressScrollbar.SetView(100f, 1000f);
            _progressScrollbar.Height.Set(0, 1f);
            _progressScrollbar.HAlign = 1f;
            _progressList.SetScrollbar(_progressScrollbar);
            Append(_progressScrollbar);
        }

        // Atualiza o conteúdo da aba de progresso
        public void UpdateProgress(RPGPlayer modPlayer)
        {
            _progressList.Clear();
            
            // Verificações robustas de null (padrão ExampleMod)
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _progressList.Add(new UIText("Jogador não disponível."));
                return;
            }

            // Seção: Chefes Derrotados
            AddProgressHeader("CHEFES DERROTADOS");
            AddProgressEntry("Olho de Cthulhu", NPC.downedBoss1);
            AddProgressEntry("Devorador/Cérebro", NPC.downedBoss2);
            AddProgressEntry("Esqueletron", NPC.downedBoss3);
            AddProgressEntry("Rainha Abelha", NPC.downedQueenBee);
            AddProgressEntry("Parede de Carne", Main.hardMode);
            AddProgressEntry("Plantera", NPC.downedPlantBoss);
            AddProgressEntry("Golem", NPC.downedGolemBoss);
            AddProgressEntry("Senhor da Lua", NPC.downedMoonlord);

            // Seção: Eventos Especiais
            AddProgressHeader("EVENTOS ESPECIAIS");
            AddProgressEntry("Invasão Goblin", NPC.downedGoblins);
            AddProgressEntry("Exército do Velho", NPC.downedFrost);
            AddProgressEntry("Invasão Pirata", NPC.downedPirates);
            AddProgressEntry("Invasão Marciana", NPC.downedMartians);

            // Seção: Progresso do Mundo
            AddProgressHeader("PROGRESSO DO MUNDO");
            AddProgressEntry("Modo Difícil", Main.hardMode);
            AddProgressEntry("Modo Difícil Ativo", Main.hardMode);
            
            // Seção: Estatísticas RPG
            AddProgressHeader("ESTATÍSTICAS RPG");
            float totalClassLevels = 0;
            if (modPlayer.ClassExperience != null)
            {
                foreach (var classExp in modPlayer.ClassExperience)
                {
                    totalClassLevels += modPlayer.GetClassLevel(classExp.Key);
                }
            }
            AddProgressEntry("Níveis Totais de Classes", (int)totalClassLevels);
            AddProgressEntry("Fome Atual", $"{modPlayer.CurrentHunger:F1}%");
            AddProgressEntry("Sanidade Atual", $"{modPlayer.CurrentSanity:F1}%");
            AddProgressEntry("Stamina Atual", $"{modPlayer.CurrentStamina:F1}%");
        }

        // Adiciona um cabeçalho de seção
        private void AddProgressHeader(string title)
        {
            _progressList.Add(new ProgressHeader(title));
        }

        // Adiciona uma entrada de progresso (bool)
        private void AddProgressEntry(string name, bool completed)
        {
            _progressList.Add(new ProgressEntry(name, completed ? "✓ Sim" : "✗ Não", completed));
        }

        // Adiciona uma entrada de progresso (int)
        private void AddProgressEntry(string name, int value)
        {
            _progressList.Add(new ProgressEntry(name, value.ToString(), value > 0));
        }

        // Adiciona uma entrada de progresso (string)
        private void AddProgressEntry(string name, string status)
        {
            _progressList.Add(new ProgressEntry(name, status, true));
        }

        // Cabeçalho de seção
        private class ProgressHeader : UIElement
        {
            public ProgressHeader(string title)
            {
                Width.Set(0, 1f);
                Height.Set(25f, 0f);
                
                var text = new UIText($"=== {title} ===", 0.9f);
                text.TextColor = Color.Gold;
                Append(text);
            }
        }

        // Elemento visual para cada entrada de progresso (padrão ExampleMod)
        private class ProgressEntry : UIElement
        {
            public ProgressEntry(string name, string status, bool completed)
            {
                Width.Set(0, 1f);
                Height.Set(20f, 0f);
                SetPadding(2);
                
                var text = new UIText($"{name}: {status}", 0.8f);
                text.TextColor = completed ? Color.LightGreen : Color.LightGray;
                Append(text);
            }
        }
    }
}
