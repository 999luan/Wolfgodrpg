using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;
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

                case WolfgodrpgMessageType.SyncClass:
                    string syncClassName = reader.ReadString();
                    float syncLevel = reader.ReadSingle();
                    float syncExp = reader.ReadSingle();
                    modPlayer.ClassLevels[syncClassName] = syncLevel;
                    modPlayer.ClassExperience[syncClassName] = syncExp;
                    break;

                case WolfgodrpgMessageType.UnlockAbility:
                    ClassAbility newAbility = (ClassAbility)reader.ReadInt32();
                    modPlayer.UnlockedAbilities.Add(newAbility);
                    break;

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
            // Receber classes
            int classCount = reader.ReadInt32();
            modPlayer.ClassLevels.Clear();
            for (int i = 0; i < classCount; i++)
            {
                string className = reader.ReadString();
                float level = reader.ReadSingle();
                modPlayer.ClassLevels[className] = level;
            }

            int expCount = reader.ReadInt32();
            modPlayer.ClassExperience.Clear();
            for (int i = 0; i < expCount; i++)
            {
                string className = reader.ReadString();
                float exp = reader.ReadSingle();
                modPlayer.ClassExperience[className] = exp;
            }

            // Receber habilidades
            int abilityCount = reader.ReadInt32();
            modPlayer.UnlockedAbilities.Clear();
            for (int i = 0; i < abilityCount; i++)
            {
                ClassAbility ability = (ClassAbility)reader.ReadInt32();
                modPlayer.UnlockedAbilities.Add(ability);
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

            // Enviar classes
            packet.Write(modPlayer.ClassLevels.Count);
            foreach (var kvp in modPlayer.ClassLevels)
            {
                packet.Write(kvp.Key);
                packet.Write(kvp.Value);
            }

            packet.Write(modPlayer.ClassExperience.Count);
            foreach (var kvp in modPlayer.ClassExperience)
            {
                packet.Write(kvp.Key);
                packet.Write(kvp.Value);
            }

            // Enviar habilidades
            packet.Write(modPlayer.UnlockedAbilities.Count);
            foreach (var ability in modPlayer.UnlockedAbilities)
            {
                packet.Write((int)ability);
            }

            // Enviar vitals
            packet.Write(modPlayer.CurrentHunger);
            packet.Write(modPlayer.CurrentSanity);
            packet.Write(modPlayer.CurrentStamina);

            packet.Send(toClient, ignoreClient);
        }
    }
} 