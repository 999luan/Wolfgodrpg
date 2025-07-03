using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Wolfgodrpg.Common.Systems;
using System.IO;
using Terraria.ID;
using Wolfgodrpg.Common.Classes;

namespace Wolfgodrpg
{
    public enum WolfgodrpgMessageType : byte
    {
        SyncRPGPlayer,
        SyncHunger,
        SyncSanity,
        SyncStamina,
        SyncClass,
        SyncClassLevel,
        SyncExperience
    }

	// Sistema RPG balanceado para Terraria
	// Focado em progressão equilibrada que mantém o desafio durante todo o jogo
	public class Wolfgodrpg : Mod
	{
		// Versão do sistema de balanceamento
		public static readonly string RPG_VERSION = "1.0.0";
		
		// Referência estática para fácil acesso
		public static Wolfgodrpg Instance { get; private set; }
		
		public override void Load()
		{
			Instance = this;
			Logger.Info($"Wolf God RPG Core v{RPG_VERSION} carregado com sucesso!");
			
			// Inicialização dos sistemas RPG
			LogRPGSystems();
		}
		
		public override void Unload()
		{
			Instance = null;
			Logger.Info("Wolf God RPG Core descarregado.");
		}

		public override void PostSetupContent()
		{
			// Adicionar callback para XP de criação
			foreach (Recipe recipe in Main.recipe)
			{
				recipe.AddOnCraftCallback((Recipe r, Item item, List<Item> consumedItems, Item destinationStack) => {
					RPGActionSystem.OnCraft(item);
				});
			}
		}

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            WolfgodrpgMessageType msgType = (WolfgodrpgMessageType)reader.ReadByte();
            byte playerID;
            Common.Players.RPGPlayer modPlayer;

            switch (msgType)
            {
                case WolfgodrpgMessageType.SyncRPGPlayer:
                    playerID = reader.ReadByte();
                    modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();

                    // Ler dados vitais
                    modPlayer.CurrentHunger = reader.ReadSingle();
                    modPlayer.CurrentSanity = reader.ReadSingle();
                    modPlayer.CurrentStamina = reader.ReadSingle();

                    // Ler dados de classe
                    int classLevelCount = reader.ReadInt32();
                    modPlayer.ClassLevels.Clear();
                    for (int i = 0; i < classLevelCount; i++)
                    {
                        modPlayer.ClassLevels[reader.ReadString()] = reader.ReadSingle();
                    }

                    int classExpCount = reader.ReadInt32();
                    modPlayer.ClassExperience.Clear();
                    for (int i = 0; i < classExpCount; i++)
                    {
                        modPlayer.ClassExperience[reader.ReadString()] = reader.ReadSingle();
                    }

                    int abilityCount = reader.ReadInt32();
                    modPlayer.UnlockedAbilities.Clear();
                    for (int i = 0; i < abilityCount; i++)
                    {
                        modPlayer.UnlockedAbilities.Add((ClassAbility)reader.ReadInt32());
                    }

                    // Se o pacote veio do servidor, o cliente precisa reenviá-lo para outros clientes
                    if (Main.netMode == NetmodeID.Server)
                    {
                        modPlayer.SyncPlayer(-1, whoAmI, false);
                    }
                    break;

                case WolfgodrpgMessageType.SyncHunger:
                    playerID = reader.ReadByte();
                    modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();
                    modPlayer.CurrentHunger = reader.ReadSingle();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var packet = GetPacket();
                        packet.Write((byte)WolfgodrpgMessageType.SyncHunger);
                        packet.Write(playerID);
                        packet.Write(modPlayer.CurrentHunger);
                        packet.Send(-1, whoAmI);
                    }
                    break;

                case WolfgodrpgMessageType.SyncSanity:
                    playerID = reader.ReadByte();
                    modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();
                    modPlayer.CurrentSanity = reader.ReadSingle();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var packet = GetPacket();
                        packet.Write((byte)WolfgodrpgMessageType.SyncSanity);
                        packet.Write(playerID);
                        packet.Write(modPlayer.CurrentSanity);
                        packet.Send(-1, whoAmI);
                    }
                    break;

                case WolfgodrpgMessageType.SyncStamina:
                    playerID = reader.ReadByte();
                    modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();
                    modPlayer.CurrentStamina = reader.ReadSingle();
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var packet = GetPacket();
                        packet.Write((byte)WolfgodrpgMessageType.SyncStamina);
                        packet.Write(playerID);
                        packet.Write(modPlayer.CurrentStamina);
                        packet.Send(-1, whoAmI);
                    }
                    break;

                case WolfgodrpgMessageType.SyncClass:
                case WolfgodrpgMessageType.SyncClassLevel:
                    playerID = reader.ReadByte();
                    modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();
                    string className = reader.ReadString();
                    float level = reader.ReadSingle();
                    float experience = reader.ReadSingle();
                    modPlayer.ClassLevels[className] = level;
                    modPlayer.ClassExperience[className] = experience;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var packet = GetPacket();
                        packet.Write((byte)msgType);
                        packet.Write(playerID);
                        packet.Write(className);
                        packet.Write(level);
                        packet.Write(experience);
                        packet.Send(-1, whoAmI);
                    }
                    break;
            }
        }

		private void LogRPGSystems()
		{
			Logger.Info("Sistemas RPG carregados:");
			Logger.Info("- ModPlayer: RPGPlayer");
			Logger.Info("- GlobalNPC: BalancedNPC");
			Logger.Info("- GlobalItem: ProgressiveItem, RPGGlobalItem");
			Logger.Info("- GlobalRecipe: RPGGlobalRecipe");
			Logger.Info("- GlobalTile: RPGGlobalTile");
			Logger.Info("- UIState: QuickStatsUI, RPGStatsUI, SimpleRPGMenu");
			Logger.Info("- Systems: PlayerVitalsSystem, RPGActionSystem, RPGCalculations, RPGConfig, RPGFishingProjectile, RPGHooks, RPGKeybinds, RPGMenuController, RPGMenuControls, RPGDebugSystem");
		}
	}
}

