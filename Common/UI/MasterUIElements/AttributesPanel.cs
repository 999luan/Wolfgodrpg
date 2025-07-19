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

            // Attributes
            int yOffset = 110;
            Append(CreateAttributeRow("Strength", _player.Strength, ref yOffset));
            Append(CreateAttributeRow("Dexterity", _player.Dexterity, ref yOffset));
            Append(CreateAttributeRow("Intelligence", _player.Intelligence, ref yOffset));
            Append(CreateAttributeRow("Constitution", _player.Constitution, ref yOffset));
            Append(CreateAttributeRow("Wisdom", _player.Wisdom, ref yOffset));
        }

        private UIElement CreateAttributeRow(string name, int value, ref int yOffset)
        {
            var row = new UIPanel();
            row.Width.Set(0, 1f);
            row.Height.Set(40, 0);
            row.Top.Set(yOffset, 0);
            row.BackgroundColor = new Color(50, 60, 70) * 0.8f;
            yOffset += 45;

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
                    // This is a simplified way to handle stat increases.
                    // A more robust system would use a method in RPGPlayer.
                    _player.AttributePoints--;
                    switch (name)
                    {
                        case "Strength": _player.Strength++; break;
                        case "Dexterity": _player.Dexterity++; break;
                        case "Intelligence": _player.Intelligence++; break;
                        case "Constitution": _player.Constitution++; break;
                        case "Wisdom": _player.Wisdom++; break;
                    }
                }
            };
            row.Append(plusButton);

            return row;
        }
    }
}