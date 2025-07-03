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
            
            // Lista de itens com scrollbar (padrão ExampleMod)
            _itemsList = new UIList();
            _itemsList.Width.Set(-25f, 1f);
            _itemsList.Height.Set(0, 1f);
            _itemsList.ListPadding = 5f;
            Append(_itemsList);

            // Scrollbar acoplado à lista
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
                _itemsList.Add(new UIText("Jogador não disponível."));
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
                        _itemsList.Add(new ItemEntry(item, globalItem, progressiveItem));
                        foundItems = true;
                    }
                }
            }

            if (!foundItems)
            {
                _itemsList.Add(new UIText("Nenhum item com atributos RPG encontrado."));
            }
        }
        
        // Elemento visual para cada item (padrão ExampleMod)
        private class ItemEntry : UIElement
        {
            public ItemEntry(Item item, RPGGlobalItem globalItem, ProgressiveItem progressiveItem)
            {
                Width.Set(0, 1f);
                Height.Set(100f, 0f);
                SetPadding(5);
                
                // Nome do item
                var nameText = new UIText($"{item.Name}", 1f);
                nameText.TextColor = Color.White; // Cor padrão por enquanto
                Append(nameText);
                
                float yOffset = 25f;

                // Mostrar nível e XP se for um item progressivo
                if (progressiveItem != null && progressiveItem.Experience > 0)
                {
                    int level = (int)(progressiveItem.Experience / 100); // Calcular nível baseado na experiência
                    float currentExp = progressiveItem.Experience;
                    float nextLevelExp = 100 * (level + 1); // Assumindo mesmo cálculo das classes
                    
                    var levelText = new UIText($"Nível: {level} | XP: {currentExp:F0}/{nextLevelExp:F0}", 0.8f);
                    levelText.Top.Set(yOffset, 0f);
                    levelText.TextColor = Color.LightBlue;
                    Append(levelText);
                    yOffset += 20f;

                    // Barra de progresso
                    var progressText = new UIText($"Progresso: {(currentExp / nextLevelExp * 100):F1}%", 0.7f);
                    progressText.Top.Set(yOffset, 0f);
                    progressText.TextColor = Color.LightGreen;
                    Append(progressText);
                    yOffset += 20f;
                }

                // Mostrar stats aleatórios se houver
                if (globalItem.RandomStats != null && globalItem.RandomStats.Any())
                {
                    var statsText = new UIText($"Bônus: {string.Join(", ", globalItem.RandomStats.Select(s => $"{GetStatDisplayName(s.Key)} +{s.Value:F1}"))}", 0.7f);
                    statsText.Top.Set(yOffset, 0f);
                    statsText.TextColor = Color.Yellow;
                    Append(statsText);
                }
            }

            // Nome amigável para cada stat (mesmo método da aba de stats)
            private string GetStatDisplayName(string statKey)
            {
                return statKey switch
                {
                    "meleeDamage" => "Dano C.C.",
                    "rangedDamage" => "Dano Dist.",
                    "magicDamage" => "Dano Mágico",
                    "minionDamage" => "Dano Servos",
                    "critChance" => "Crítico",
                    "meleeCrit" => "Crit C.C.",
                    "rangedCrit" => "Crit Dist.",
                    "magicCrit" => "Crit Mágico",
                    "meleeSpeed" => "Vel. C.C.",
                    "defense" => "Defesa",
                    "maxLife" => "Vida Máx.",
                    "lifeRegen" => "Regen Vida",
                    "moveSpeed" => "Velocidade",
                    "maxMana" => "Mana Máx.",
                    "manaRegen" => "Regen Mana",
                    "luck" => "Sorte",
                    _ => statKey
                };
            }
        }
    }
}
