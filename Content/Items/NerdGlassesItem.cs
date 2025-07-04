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
        // Timer para mostrar logs automaticamente
        private int autoLogTimer = 0;
        private const int AUTO_LOG_INTERVAL = 600; // 10 segundos (60 FPS * 10)

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // Nome e tooltip definidos via Localization/en-US_Mods.Wolfgodrpg.hjson
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            
            // Mostrar logs automaticamente a cada 10 segundos
            if (player.whoAmI == Main.myPlayer)
            {
                autoLogTimer++;
                if (autoLogTimer >= AUTO_LOG_INTERVAL)
                {
                    autoLogTimer = 0;
                    RPGNotificationSystem.ShowXPLogs();
                }
            }
        }

        public override bool? UseItem(Player player)
        {
            // Exibir logs quando usado manualmente
            if (player.whoAmI == Main.myPlayer)
            {
                RPGNotificationSystem.ShowXPLogs();
            }
            return true;
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