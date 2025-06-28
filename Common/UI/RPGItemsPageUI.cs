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
using Wolfgodrpg.Common.GlobalItems;

namespace Wolfgodrpg.Common.UI
{
    public class RPGItemsPageUI : UIElement
    {
        private UIElement _itemsContainer;
        private bool _visible = false;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            _itemsContainer = new UIElement();
            _itemsContainer.Width.Set(0, 1f);
            _itemsContainer.Height.Set(0, 1f);
            _itemsContainer.SetPadding(0);
            Append(_itemsContainer);
        }

        public void UpdateItems()
        {
            _itemsContainer.RemoveAllChildren(); // Clear previous items

            var player = Main.LocalPlayer;
            float currentY = 0f;

            bool hasItems = false;

            // Armadura e Acessórios
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir && item.TryGetGlobalItem(out RPGGlobalItem globalItem) && globalItem.randomStats.Count > 0)
                {
                    ItemEntry itemUI = new ItemEntry(item, globalItem);
                    itemUI.Top.Set(currentY, 0f);
                    itemUI.Left.Set(0, 0.05f);
                    itemUI.Width.Set(0, 0.9f);
                    itemUI.Height.Set(60f + (globalItem.randomStats.Count * 15f), 0f); // Dynamic height based on stats
                    _itemsContainer.Append(itemUI);
                    currentY += itemUI.Height.Pixels + 10f; // Spacing between items
                    hasItems = true;
                }
            }

            // Item Segurado
            Item heldItem = player.HeldItem;
            if (heldItem != null && !heldItem.IsAir && heldItem.TryGetGlobalItem(out RPGGlobalItem heldGlobalItem) && heldGlobalItem.randomStats.Count > 0)
            {
                ItemEntry itemUI = new ItemEntry(heldItem, heldGlobalItem);
                itemUI.Top.Set(currentY, 0f);
                itemUI.Left.Set(0, 0.05f);
                itemUI.Width.Set(0, 0.9f);
                itemUI.Height.Set(60f + (heldGlobalItem.randomStats.Count * 15f), 0f); // Dynamic height based on stats
                _itemsContainer.Append(itemUI);
                currentY += itemUI.Height.Pixels + 10f; // Spacing between items
                hasItems = true;
            }

            if (!hasItems)
            {
                UIText noItemsText = new UIText("Nenhum item com atributos equipado.", 0.9f);
                noItemsText.HAlign = 0.5f;
                noItemsText.VAlign = 0.5f;
                _itemsContainer.Append(noItemsText);
            }
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

        private class ItemEntry : UIElement
        {
            private UIText _itemNameText;
            private UIElement _statsContainer;

            public ItemEntry(Item item, RPGGlobalItem globalItem)
            {
                Width.Set(0, 1f);
                Height.Set(0, 1f);

                _itemNameText = new UIText($"--- {item.Name} ---", 1f, true);
                _itemNameText.HAlign = 0.5f;
                _itemNameText.Top.Set(0f, 0f);
                Append(_itemNameText);

                _statsContainer = new UIElement();
                _statsContainer.Width.Set(0, 1f);
                _statsContainer.Height.Set(0, 1f);
                _statsContainer.Top.Set(20f, 0f);
                _statsContainer.SetPadding(0);
                Append(_statsContainer);

                float statY = 0f;
                foreach (var stat in globalItem.randomStats)
                {
                    var statInfo = RPGClassDefinitions.RandomStats[stat.Key];
                    string valueString = stat.Value < 1 ? $"+{stat.Value:P1}" : $"+{stat.Value:F0}";
                    UIText statText = new UIText($"  {valueString} {statInfo.Name}", 0.8f);
                    statText.Top.Set(statY, 0f);
                    statText.Left.Set(20f, 0f);
                    _statsContainer.Append(statText);
                    statY += 15f;
                }
            }
        }
    }
}

