using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;
using System.Linq;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Network;

namespace Wolfgodrpg.Common.Systems
{
    public class NetworkSystem : ModSystem
    {
        public void HandlePacket(BinaryReader reader, int whoAmI)
        {
            WolfgodrpgMessageType msgType = (WolfgodrpgMessageType)reader.ReadByte();
            int playerID = reader.ReadInt32();
            var player = Main.player[playerID];
            var modPlayer = player.GetModPlayer<RPGPlayer>();

            switch (msgType)
            {
                case WolfgodrpgMessageType.SyncRPGPlayer:
                    HandlePlayerSync(modPlayer, reader);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Reenviar para outros clientes
                        SendPlayerSync(modPlayer, -1, whoAmI);
                    }
                    break;

                // case WolfgodrpgMessageType.SyncClass: // Obsolete, handled by SyncRPGPlayer
                //     string syncClassName = reader.ReadString();
                //     float syncLevel = reader.ReadSingle();
                //     float syncExp = reader.ReadSingle();
                //     // This logic is now handled by the SubClassSystem's LoadData
                //     break;

                // case WolfgodrpgMessageType.UnlockAbility: // Obsolete, abilities are unlocked via subclass level
                //     ClassAbility newAbility = (ClassAbility)reader.ReadInt32();
                //     // This logic is now handled by the SubClassSystem's LoadData
                //     break;

                case WolfgodrpgMessageType.UpdateVitals:
                    float value = reader.ReadSingle();
                    byte vitalType = reader.ReadByte();
                    switch (vitalType)
                    {
                        case 0: modPlayer.CurrentHunger = value; break;
                        case 1: modPlayer.CurrentSanity = value; break;
                        case 2: modPlayer.CurrentStamina = value; break;
                    }
                    break;

                case WolfgodrpgMessageType.SyncDash:
                    modPlayer.DashCooldown = reader.ReadInt32();
                    modPlayer.DashesUsed = reader.ReadInt32();
                    modPlayer.DashResetTimer = reader.ReadInt32();
                    break;
            }
        }

        private void HandlePlayerSync(RPGPlayer modPlayer, BinaryReader reader)
        {
            // Receber subclasses
            int subClassCount = reader.ReadInt32();
            for (int i = 0; i < subClassCount; i++)
            {
                string subclassName = reader.ReadString();
                int level = reader.ReadInt32();
                int xp = reader.ReadInt32();
                bool isUnlocked = reader.ReadBoolean();

                var subClass = modPlayer.SubClasses.SubClasses.FirstOrDefault(sc => sc.Name == subclassName);
                if (subClass != null)
                {
                    subClass.SetLevel(level);
                    subClass.SetXP(xp);
                    subClass.SetUnlocked(isUnlocked);
                }
            }

            // Receber vitals
            modPlayer.CurrentHunger = reader.ReadSingle();
            modPlayer.CurrentSanity = reader.ReadSingle();
            modPlayer.CurrentStamina = reader.ReadSingle();
        }

        public void SendPlayerSync(RPGPlayer modPlayer, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            var packet = Mod.GetPacket();
            packet.Write((byte)WolfgodrpgMessageType.SyncRPGPlayer);
            packet.Write(modPlayer.Player.whoAmI);

            // Enviar subclasses
            packet.Write(modPlayer.SubClasses.SubClasses.Count);
            foreach (var subClass in modPlayer.SubClasses.SubClasses)
            {
                packet.Write(subClass.Name);
                packet.Write(subClass.Level);
                packet.Write(subClass.XP);
                packet.Write(subClass.IsUnlocked);
            }

            // Enviar vitals
            packet.Write(modPlayer.CurrentHunger);
            packet.Write(modPlayer.CurrentSanity);
            packet.Write(modPlayer.CurrentStamina);

            packet.Send(toClient, ignoreClient);
        }
    }
} 