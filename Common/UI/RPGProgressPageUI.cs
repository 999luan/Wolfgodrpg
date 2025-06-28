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

namespace Wolfgodrpg.Common.UI
{
    public class RPGProgressPageUI : UIElement
    {
        private UIElement _progressContainer;
        private bool _visible = false;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            _progressContainer = new UIElement();
            _progressContainer.Width.Set(0, 1f);
            _progressContainer.Height.Set(0, 1f);
            _progressContainer.SetPadding(0);
            Append(_progressContainer);
        }

        public void UpdateProgress(RPGPlayer modPlayer)
        {
            _progressContainer.RemoveAllChildren(); // Clear previous progress entries

            float currentY = 0f;

            // Bosses
            AddProgressEntry("--- Chefes Derrotados ---", "", ref currentY);
            AddProgressEntry("Olho de Cthulhu", NPC.downedBoss1 ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Devorador/Cérebro", NPC.downedBoss2 ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Esqueletron", NPC.downedBoss3 ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Rainha Abelha", NPC.downedQueenBee ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Parede de Carne", Main.hardMode ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Plantera", NPC.downedPlantBoss ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Golem", NPC.downedGolemBoss ? "Sim" : "Não", ref currentY);
            AddProgressEntry("Senhor da Lua", NPC.downedMoonlord ? "Sim" : "Não", ref currentY);
        }

        private void AddProgressEntry(string name, string status, ref float currentY)
        {
            ProgressEntry entry = new ProgressEntry(name, status);
            entry.Top.Set(currentY, 0f);
            entry.Left.Set(0, 0.05f); // Small left padding
            entry.Width.Set(0, 0.9f);
            entry.Height.Set(20f, 0f); // Fixed height for each entry
            _progressContainer.Append(entry);
            currentY += 25f; // Increment Y for the next entry
        }

        public new void Activate()
        {
            _visible = true;
        }

        public new void Deactivate()
        {
            _visible = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!_visible) return;
            base.DrawSelf(spriteBatch);
        }

        private class ProgressEntry : UIElement
        {
            private UIText _entryText;

            public ProgressEntry(string name, string status)
            {
                Width.Set(0, 1f);
                Height.Set(20f, 0f);

                _entryText = new UIText(name + (string.IsNullOrEmpty(status) ? "" : ": " + status), 0.9f);
                _entryText.HAlign = 0f;
                _entryText.VAlign = 0.5f;
                Append(_entryText);
            }
        }
    }
}
