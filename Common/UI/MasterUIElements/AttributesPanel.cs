using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.UI.MasterUIElements
{
    public class AttributesPanel : UIPanel
    {
        private readonly RPGPlayer _player;
        private UIList _attributeList;
        private UIScrollbar _attributeScrollbar;

        public AttributesPanel(RPGPlayer player)
        {
            _player = player;
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            // Player Level and Experience
            var levelText = new UIText($"Level: {_player.PlayerLevel}", 1.1f);
            levelText.Top.Set(10, 0);
            levelText.Left.Set(10, 0);
            Append(levelText);

            var xpText = new UIText($"XP: {_player.PlayerExperience:F0} / {RPGPlayer.GetPlayerExperienceForLevel(_player.PlayerLevel + 1):F0}", 0.9f);
            xpText.Top.Set(40, 0);
            xpText.Left.Set(10, 0);
            Append(xpText);

            var pointsText = new UIText($"Attribute Points: {_player.AttributePoints}", 1f, true);
            pointsText.Top.Set(70, 0);
            pointsText.HAlign = 0.5f;
            Append(pointsText);

            // Attribute List
            _attributeList = new UIList();
            _attributeList.Width.Set(-20, 1f);
            _attributeList.Height.Set(-100, 1f); // Adjust height to make space for level/xp/points
            _attributeList.Top.Set(100, 0);
            _attributeList.Left.Set(0, 0);
            _attributeList.ListPadding = 5f;
            Append(_attributeList);

            _attributeScrollbar = new UIScrollbar();
            _attributeScrollbar.SetView(100f, 1000f); // Placeholder values, will adjust dynamically
            _attributeScrollbar.Height.Set(-100, 1f);
            _attributeScrollbar.Top.Set(100, 0);
            _attributeScrollbar.Left.Set(-20, 1f);
            Append(_attributeScrollbar);

            _attributeList.SetScrollbar(_attributeScrollbar);

            // Populate attributes
            PopulateAttributes();
        }

        private void PopulateAttributes()
        {
            _attributeList.Clear();

            _attributeList.Add(CreateAttributeRow("Strength", _player.Strength));
            _attributeList.Add(CreateAttributeRow("Dexterity", _player.Dexterity));
            _attributeList.Add(CreateAttributeRow("Intelligence", _player.Intelligence));
            _attributeList.Add(CreateAttributeRow("Constitution", _player.Constitution));
            _attributeList.Add(CreateAttributeRow("Wisdom", _player.Wisdom));
        }

        private UIElement CreateAttributeRow(string name, int value)
        {
            var row = new UIPanel();
            row.Width.Set(0, 1f);
            row.Height.Set(40, 0);
            row.BackgroundColor = new Color(50, 60, 70) * 0.8f;

            var nameText = new UIText(name, 1f);
            nameText.Left.Set(15, 0);
            nameText.VAlign = 0.5f;
            row.Append(nameText);

            var valueText = new UIText(value.ToString(), 1f);
            valueText.Left.Set(150, 0);
            valueText.VAlign = 0.5f;
            row.Append(valueText);

            var plusButton = new UITextPanel<string>("+", 1f, true);
            plusButton.Width.Set(30, 0);
            plusButton.Height.Set(30, 0);
            plusButton.Left.Set(200, 0);
            plusButton.VAlign = 0.5f;
            plusButton.BackgroundColor = new Color(60, 140, 70);
            plusButton.OnLeftClick += (evt, elem) => {
                if (_player.AttributePoints > 0)
                {
                    _player.AttributePoints--;
                    switch (name)
                    {
                        case "Strength": _player.Strength++; break;
                        case "Dexterity": _player.Dexterity++; break;
                        case "Intelligence": _player.Intelligence++; break;
                        case "Constitution": _player.Constitution++; break;
                        case "Wisdom": _player.Wisdom++; break;
                    }
                    PopulateAttributes(); // Re-populate to update values
                }
            };
            row.Append(plusButton);

            return row;
        }
    }
}
