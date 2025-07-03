using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Wolfgodrpg.Content.NPCs
{
    /// <summary>
    /// NPC de teste para demonstração do sistema RPG.
    /// </summary>
    public class RPGTestNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Bunny];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 10;
            NPC.defense = 2;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = 7; // Bunny AI
            AIType = NPCID.Bunny;
            AnimationType = NPCID.Bunny;
            NPC.friendly = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Spawna na superfície durante o dia
            return spawnInfo.Player.ZoneOverworldHeight && Main.dayTime ? 0.05f : 0f;
        }
    }
} 