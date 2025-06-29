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
using System.Linq;

namespace Wolfgodrpg.Common.UI
{
    // Aba de Itens do menu RPG
    public class RPGItemsPageUI : UIElement
    {
        private UIList _itemsList;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            
            _itemsList = new UIList();
            _itemsList.Width.Set(0, 1f);
            _itemsList.Height.Set(0, 1f);
            _itemsList.ListPadding = 5f;
            Append(_itemsList);
        }

        // Atualiza o conteúdo da aba de itens
        public void UpdateItems()
        {
            _itemsList.Clear();
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active || Main.LocalPlayer.inventory == null) return;
            
            bool foundItems = false;
            foreach (var item in Main.LocalPlayer.inventory)
            {
                if (item.TryGetGlobalItem<RPGGlobalItem>(out var globalItem) && globalItem.randomStats.Any())
                {
                    _itemsList.Add(new ItemEntry(item, globalItem));
                    foundItems = true;
                }
            }

            if (!foundItems)
            {
                _itemsList.Add(new UIText("Nenhum item com atributos equipado."));
            }
        }
        
        // Elemento visual para cada item
        private class ItemEntry : UIElement
        {
            public ItemEntry(Item item, RPGGlobalItem globalItem)
            {
                Width.Set(0, 1f);
                Height.Set(80f, 0f);
                SetPadding(5);
                
                var text = new UIText($"{item.Name}: {string.Join(", ", globalItem.randomStats.Keys)}");
                Append(text);
                // Adicione mais detalhes visuais do item aqui se desejar
            }
        }
    }
}

