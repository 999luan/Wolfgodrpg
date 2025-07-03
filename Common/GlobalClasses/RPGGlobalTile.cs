using Terraria;
using Terraria.ModLoader;
using Terraria.ID; 
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Collections.Generic; // Adicionado para HashSet

namespace Wolfgodrpg.Common.GlobalClasses
{
    public class RPGGlobalTile : GlobalTile
    {
        // Lista de TileIDs de plantas colhíveis
        private static HashSet<int> HarvestablePlantIDs = new HashSet<int>
        {
            TileID.Plants, TileID.Plants2, TileID.MushroomPlants, TileID.JunglePlants, TileID.JunglePlants2,
            TileID.HallowedPlants, TileID.HallowedPlants2, TileID.CorruptPlants, TileID.CrimsonPlants,
            TileID.Sunflower, TileID.Pumpkins
        };

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail) return;

            var player = Main.LocalPlayer;
            var rpgPlayer = player.GetModPlayer<RPGPlayer>();

            // Se for uma planta colhível
            if (HarvestablePlantIDs.Contains(type))
            {
                rpgPlayer.AddClassExperience("explorer", 5f); // XP por colher
            }
            else // Se for qualquer outro bloco
            {
                RPGActionSystem.OnBlockMine();
            }
        }

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            RPGActionSystem.OnBlockPlace();
        }
    }
}
