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
    // Aba de Progresso do menu RPG
    public class RPGProgressPageUI : UIElement
    {
        private UIList _progressList;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            _progressList = new UIList();
            _progressList.Width.Set(0, 1f);
            _progressList.Height.Set(0, 1f);
            _progressList.ListPadding = 5f;
            Append(_progressList);
        }

        // Só popula conteúdo via método público
        public void UpdateProgress(RPGPlayer modPlayer)
        {
            _progressList.Clear();
            if (modPlayer == null || modPlayer.Player == null) return;

            AddProgressEntry("--- Chefes Derrotados ---", "");
            AddProgressEntry("Olho de Cthulhu", NPC.downedBoss1 ? "Sim" : "Não");
            AddProgressEntry("Devorador/Cérebro", NPC.downedBoss2 ? "Sim" : "Não");
            AddProgressEntry("Esqueletron", NPC.downedBoss3 ? "Sim" : "Não");
            AddProgressEntry("Rainha Abelha", NPC.downedQueenBee ? "Sim" : "Não");
            AddProgressEntry("Parede de Carne", Main.hardMode ? "Sim" : "Não");
            AddProgressEntry("Plantera", NPC.downedPlantBoss ? "Sim" : "Não");
            AddProgressEntry("Golem", NPC.downedGolemBoss ? "Sim" : "Não");
            AddProgressEntry("Senhor da Lua", NPC.downedMoonlord ? "Sim" : "Não");
        }

        // Adiciona uma entrada visual de progresso
        private void AddProgressEntry(string name, string status)
        {
            _progressList.Add(new ProgressEntry(name, status));
        }

        // Elemento visual para cada entrada de progresso
        private class ProgressEntry : UIElement
        {
            public ProgressEntry(string name, string status)
            {
                Width.Set(0, 1f);
                Height.Set(20f, 0f);
                SetPadding(2);
                var text = new UIText(name + (string.IsNullOrEmpty(status) ? "" : ": " + status), 0.9f);
                Append(text);
            }
        }
    }
}
