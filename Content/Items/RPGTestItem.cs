using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Content.Items
{
    /// <summary>
    /// Item de teste para demonstrar o sistema RPG.
    /// Concede XP de movimento quando usado.
    /// </summary>
    public class RPGTestItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 1, 0, 0); // 1 gold
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.consumable = true;
            Item.maxStack = 99;
        }

        public override bool CanUseItem(Player player)
        {
            // Só pode usar se tiver o ModPlayer
            return player.GetModPlayer<RPGPlayer>() != null;
        }

        public override bool? UseItem(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer != null)
            {
                // Concede XP de movimento
                modPlayer.GainClassExp("movement", 50f);
                
                // Mensagem de feedback
                Main.NewText("XP de Movimento +50!", Color.LightBlue);
                
                return true;
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            // Receita simples: 10 gel + 5 madeira
            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 10)
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
} 