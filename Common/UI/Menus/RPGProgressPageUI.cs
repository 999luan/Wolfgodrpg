using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.UI.Design;

namespace Wolfgodrpg.Common.UI.Menus
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
            
            // Lista de progresso com scrollbar (padrão oficial tModLoader)
            _progressList = new UIList();
            _progressList.Width.Set(-25f, 1f);
            _progressList.Height.Set(0, 1f);
            _progressList.ListPadding = 5f;
            Append(_progressList);

            // Scrollbar acoplado à lista (padrão oficial tModLoader)
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
            
            // Robust null checks (ExampleMod standard)
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _progressList.Add(new UIText("Player not available."));
                return;
            }

            // Section: Defeated Bosses
            _progressList.Add(new ProgressSectionCard("DEFEATED BOSSES", "👑"));
            _progressList.Add(new ProgressCard("Eye of Cthulhu", NPC.downedBoss1, "👁️", "The eye that sees all..."));
            _progressList.Add(new ProgressCard("Eater of Worlds/Brain", NPC.downedBoss2, "🐛", "Brains and worms!"));
            _progressList.Add(new ProgressCard("Skeletron", NPC.downedBoss3, "💀", "Giant skeleton!"));
            _progressList.Add(new ProgressCard("Queen Bee", NPC.downedQueenBee, "🐝", "Zzzzzzzz!"));
            _progressList.Add(new ProgressCard("Wall of Flesh", Main.hardMode, "🥩", "Wall of flesh!"));
            _progressList.Add(new ProgressCard("Plantera", NPC.downedPlantBoss, "🌱", "Killer plant!"));
            _progressList.Add(new ProgressCard("Golem", NPC.downedGolemBoss, "🤖", "Stone golem!"));
            _progressList.Add(new ProgressCard("Moon Lord", NPC.downedMoonlord, "🌙", "The end is near!"));

            // Section: Special Events
            _progressList.Add(new ProgressSectionCard("SPECIAL EVENTS", "🎪"));
            _progressList.Add(new ProgressCard("Goblin Invasion", NPC.downedGoblins, "👹", "Goblins invade!"));
            _progressList.Add(new ProgressCard("Frost Legion", NPC.downedFrost, "❄️", "Freezing everything!"));
            _progressList.Add(new ProgressCard("Pirate Invasion", NPC.downedPirates, "🏴‍☠️", "Yarr matey!"));
            _progressList.Add(new ProgressCard("Martian Invasion", NPC.downedMartians, "👽", "ET phone home!"));

            // Section: World Progress
            _progressList.Add(new ProgressSectionCard("WORLD PROGRESS", "🌍"));
            _progressList.Add(new ProgressCard("Hard Mode", Main.hardMode, "🔥", "Now it's serious!"));
            
            // Section: RPG Statistics
            _progressList.Add(new ProgressSectionCard("RPG STATISTICS", "📊"));
            float totalClassLevels = 0;
            if (modPlayer.ClassExperience != null)
            {
                foreach (var classExp in modPlayer.ClassExperience)
                {
                    totalClassLevels += modPlayer.ClassLevels.TryGetValue(classExp.Key, out var lvl) ? lvl : 0f;
                }
            }
            _progressList.Add(new ProgressCard("Total Class Levels", (int)totalClassLevels, "⭐", "Total class progress"));
            // Removed: Current Hunger, Current Sanity, Current Stamina
        }

        // Card de seção de progresso
        private class ProgressSectionCard : UIElement
        {
            private UIText _titleText;
            private UIText _iconText;

            public ProgressSectionCard(string title, string icon)
            {
                Width.Set(0, 1f);
                Height.Set(60f, 0f);

                // Ícone da seção
                _iconText = new UIText(icon, 2f);
                _iconText.TextColor = Color.Gold;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(15f, 0f);
                Append(_iconText);

                // Título da seção
                _titleText = new UIText(title, 1.1f, true);
                _titleText.TextColor = Color.Gold;
                _titleText.Left.Set(80f, 0f);
                _titleText.Top.Set(20f, 0f);
                Append(_titleText);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();

                // Fundo do card de seção
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Gold * 0.05f);
                
                // Borda dourada
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), Color.Gold);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), Color.Gold);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), Color.Gold);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), Color.Gold);
            }
        }

        // Card de entrada de progresso
        private class ProgressCard : UIElement
        {
            private UIText _titleText;
            private UIText _iconText;
            private UIText _statusText;
            private UIText _funnyText;
            private Color _color;

            public ProgressCard(string name, bool completed, string icon, string funny)
            {
                _color = completed ? RPGDesignSystem.Colors.Success : RPGDesignSystem.Colors.Error;
                Width.Set(0, 1f);
                Height.Set(120f, 0f);

                // Título
                _titleText = new UIText(name, 1.0f, true);
                _titleText.TextColor = _color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone
                _iconText = new UIText(icon, 1.5f);
                _iconText.TextColor = _color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(45f, 0f);
                Append(_iconText);

                // Status
                _statusText = new UIText(completed ? "✓ Completed" : "✗ Pending", 0.9f);
                _statusText.TextColor = completed ? Color.LightGreen : Color.LightGray;
                _statusText.Left.Set(80f, 0f);
                _statusText.Top.Set(45f, 0f);
                Append(_statusText);

                // Texto engraçado
                _funnyText = new UIText(funny, 0.8f);
                _funnyText.TextColor = Color.LightGray;
                _funnyText.Left.Set(20f, 0f);
                _funnyText.Top.Set(75f, 0f);
                Append(_funnyText);
            }

            public ProgressCard(string name, int value, string icon, string funny)
            {
                _color = value > 0 ? RPGDesignSystem.Colors.Success : RPGDesignSystem.Colors.Warning;
                Width.Set(0, 1f);
                Height.Set(120f, 0f);

                // Título
                _titleText = new UIText(name, 1.0f, true);
                _titleText.TextColor = _color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone
                _iconText = new UIText(icon, 1.5f);
                _iconText.TextColor = _color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(45f, 0f);
                Append(_iconText);

                // Valor
                _statusText = new UIText(value.ToString(), 0.9f);
                _statusText.TextColor = Color.White;
                _statusText.Left.Set(80f, 0f);
                _statusText.Top.Set(45f, 0f);
                Append(_statusText);

                // Texto engraçado
                _funnyText = new UIText(funny, 0.8f);
                _funnyText.TextColor = Color.LightGray;
                _funnyText.Left.Set(20f, 0f);
                _funnyText.Top.Set(75f, 0f);
                Append(_funnyText);
            }

            public ProgressCard(string name, string status, string icon, string funny)
            {
                _color = RPGDesignSystem.Colors.Info;
                Width.Set(0, 1f);
                Height.Set(120f, 0f);

                // Título
                _titleText = new UIText(name, 1.0f, true);
                _titleText.TextColor = _color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone
                _iconText = new UIText(icon, 1.5f);
                _iconText.TextColor = _color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(45f, 0f);
                Append(_iconText);

                // Status
                _statusText = new UIText(status, 0.9f);
                _statusText.TextColor = Color.White;
                _statusText.Left.Set(80f, 0f);
                _statusText.Top.Set(45f, 0f);
                Append(_statusText);

                // Texto engraçado
                _funnyText = new UIText(funny, 0.8f);
                _funnyText.TextColor = Color.LightGray;
                _funnyText.Left.Set(20f, 0f);
                _funnyText.Top.Set(75f, 0f);
                Append(_funnyText);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();

                // Fundo do card
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, _color * 0.1f);
                
                // Borda
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), _color);
            }

            public override void MouseOver(UIMouseEvent evt)
            {
                base.MouseOver(evt);
                Main.instance.MouseText(_funnyText.Text);
            }
        }
    }
}
