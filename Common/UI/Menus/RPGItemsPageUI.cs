using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.GlobalItems;
using System.Linq;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Design;

namespace Wolfgodrpg.Common.UI.Menus
{
    // Aba de Itens do menu RPG (Padrão ExampleMod)
    public class RPGItemsPageUI : UIElement
    {
        private UIList _itemsList;
        private UIScrollbar _itemsScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            
            // Lista de itens com scrollbar (padrão oficial tModLoader)
            _itemsList = new UIList();
            _itemsList.Width.Set(-25f, 1f);
            _itemsList.Height.Set(0, 1f);
            _itemsList.ListPadding = 5f;
            Append(_itemsList);

            // Scrollbar acoplado à lista (padrão oficial tModLoader)
            _itemsScrollbar = new UIScrollbar();
            _itemsScrollbar.SetView(100f, 1000f);
            _itemsScrollbar.Height.Set(0, 1f);
            _itemsScrollbar.HAlign = 1f;
            _itemsList.SetScrollbar(_itemsScrollbar);
            Append(_itemsScrollbar);
        }

        // Atualiza o conteúdo da aba de itens
        public void UpdateItems()
        {
            _itemsList.Clear();
            
            // Verificações robustas de null (padrão ExampleMod)
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active || Main.LocalPlayer.inventory == null)
            {
                _itemsList.Add(new UIText("Player not available."));
                return;
            }
            
            bool foundItems = false;
            foreach (var item in Main.LocalPlayer.inventory)
            {
                if (item == null || item.IsAir) continue;

                // Verificar itens com stats RPG
                if (item.TryGetGlobalItem<RPGGlobalItem>(out var globalItem))
                {
                    bool hasRPGData = false;
                    
                    // Verificar se tem stats aleatórios ou experiência
                    if (globalItem.RandomStats != null && globalItem.RandomStats.Any())
                        hasRPGData = true;
                    
                    // Verificar se é um ProgressiveItem
                    if (item.TryGetGlobalItem<ProgressiveItem>(out var progressiveItem) && progressiveItem.Experience > 0)
                        hasRPGData = true;

                    if (hasRPGData)
                    {
                        _itemsList.Add(new ItemCard(item, globalItem, progressiveItem));
                        foundItems = true;
                    }
                }
            }

            if (!foundItems)
            {
                _itemsList.Add(new UIText("No item with RPG attributes found."));
            }
        }
        
        // Card de item com atributos RPG
        private class ItemCard : UIElement
        {
            private UIText _titleText;
            private UIText _iconText;
            private UIText _rarityText;
            private UIText _statsText;
            private UIText _progressiveText;
            private Color _color;

            public ItemCard(Item item, RPGGlobalItem globalItem, ProgressiveItem progressiveItem)
            {
                _color = GetItemRarityColor(item.rare);
                Width.Set(0, 1f);
                Height.Set(200f, 0f);

                // Título do item
                _titleText = new UIText(item.Name, 1.1f, true);
                _titleText.TextColor = _color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone do item
                _iconText = new UIText(GetItemIcon(item), 2f);
                _iconText.TextColor = _color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(50f, 0f);
                Append(_iconText);

                // Raridade
                _rarityText = new UIText($"Rarity: {GetRarityName(item.rare)}", 0.9f);
                _rarityText.TextColor = Color.LightGray;
                _rarityText.Left.Set(80f, 0f);
                _rarityText.Top.Set(50f, 0f);
                Append(_rarityText);

                float yOffset = 80f;

                // Stats aleatórios
                if (globalItem.RandomStats != null && globalItem.RandomStats.Any())
                {
                    var statsHeader = new UIText("📊 RPG Attributes:", 0.9f);
                    statsHeader.TextColor = Color.LightBlue;
                    statsHeader.Left.Set(20f, 0f);
                    statsHeader.Top.Set(yOffset, 0f);
                    Append(statsHeader);
                    yOffset += 20f;

                    foreach (var stat in globalItem.RandomStats)
                    {
                        string statName = RPGDisplayUtils.GetStatDisplayName(stat.Key);
                        var statText = new UIText($"  {statName}: +{stat.Value:F1}", 0.8f);
                        statText.TextColor = Color.LightBlue;
                        statText.Left.Set(20f, 0f);
                        statText.Top.Set(yOffset, 0f);
                        Append(statText);
                        yOffset += 15f;
                    }
                }

                // Experiência progressiva
                if (progressiveItem != null && progressiveItem.Experience > 0)
                {
                    int level = progressiveItem.GetItemLevel();
                    float nextLevelExp = GetProgressiveExperienceForLevel(level + 1);
                    float progressPercent = nextLevelExp > 0 ? (progressiveItem.Experience / nextLevelExp) * 100f : 0f;
                    
                    _progressiveText = new UIText($"⚡ Level {level:F0} | XP: {progressiveItem.Experience:F0}/{nextLevelExp:F0} ({progressPercent:F1}%)", 0.8f);
                    _progressiveText.TextColor = Color.LightGreen;
                    _progressiveText.Left.Set(20f, 0f);
                    _progressiveText.Top.Set(yOffset, 0f);
                    Append(_progressiveText);
                }
            }

            private string GetItemIcon(Item item)
            {
                return item.type switch
                {
                    ItemID.WoodenSword => "⚔️",
                    ItemID.WoodenBow => "🏹",
                    ItemID.WandofSparking => "🔮",
                    ItemID.SlimeStaff => "👾",
                    ItemID.HermesBoots => "🏃",
                    ItemID.Compass => "🧭",
                    ItemID.Wrench => "🔧",
                    ItemID.Campfire => "🔥",
                    ItemID.IronAnvil => "🛠️",
                    ItemID.BottledWater => "⚗️",
                    ItemID.CrystalBall => "🔮",
                    _ => "📦"
                };
            }

            private Color GetItemRarityColor(int rarity)
            {
                return rarity switch
                {
                    -1 => Color.Gray,      // Cinza
                    0 => Color.White,      // Branco
                    1 => Color.Green,      // Verde
                    2 => Color.Blue,       // Azul
                    3 => Color.Purple,     // Roxo
                    4 => Color.Orange,     // Laranja
                    5 => Color.Red,        // Vermelho
                    6 => Color.Pink,       // Rosa
                    7 => Color.Yellow,     // Amarelo
                    8 => Color.Cyan,       // Ciano
                    9 => Color.Magenta,    // Magenta
                    10 => Color.Gold,      // Dourado
                    _ => Color.White
                };
            }

            private string GetRarityName(int rarity)
            {
                return rarity switch
                {
                    -1 => "Common",
                    0 => "White",
                    1 => "Green",
                    2 => "Blue",
                    3 => "Purple",
                    4 => "Orange",
                    5 => "Red",
                    6 => "Pink",
                    7 => "Yellow",
                    8 => "Cyan",
                    9 => "Magenta",
                    10 => "Gold",
                    _ => "Unknown"
                };
            }

            private float GetProgressiveExperienceForLevel(int level)
            {
                return 100f * (float)Math.Pow(level, 1.8f);
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
                Main.instance.MouseText("Item with RPG attributes");
            }
        }
    }
}
