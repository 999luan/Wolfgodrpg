using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.GlobalTiles
{
    public class RPGGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // Se o bloco foi realmente destruído (não falhou)
            if (!fail)
            {
                // Chama o nosso sistema de ação para registrar a mineração
                RPGActionSystem.OnBlockMine();
            }
        }

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            // Chama o nosso sistema de ação para registrar a construção
            RPGActionSystem.OnBlockPlace();
        }
    }
}
