using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Systems;
using System.Collections.Generic;
using Terraria.Localization;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Content.Items
{
    public class NerdGlassesItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // tModLoader 1.4+ usa propriedades
            // DisplayName.SetDefault("Nerd Glasses");
            // Tooltip.SetDefault("Use to view all XP logs accumulated so far.");
        }

        public override LocalizedText DisplayName => Language.GetText("Mods.Wolfgodrpg.Items.NerdGlassesItem.DisplayName");
        public override LocalizedText Tooltip => Language.GetText("Mods.Wolfgodrpg.Items.NerdGlassesItem.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 0, 50, 0); // 50 silver
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item4;
            Item.autoReuse = false;
            Item.consumable = false;
            Item.maxStack = 1;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            // Exibir todos os logs de XP acumulados
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer != null)
            {
                RPGNotificationSystem.ShowAllXPLogs(player);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lens, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
} 