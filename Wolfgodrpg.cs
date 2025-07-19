using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ModLoader.Config;
using Wolfgodrpg.Common.Systems;
using System.IO;
using Terraria.ID;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Network;

namespace Wolfgodrpg
{
	// Sistema RPG balanceado para Terraria
	// Focado em progressão equilibrada que mantém o desafio durante todo o jogo
	public class Wolfgodrpg : Mod
	{
		// Versão do sistema de balanceamento
		public static readonly string RPG_VERSION = "1.0.0";
		
		// Referência estática para fácil acesso
		public static Wolfgodrpg Instance { get; private set; }
		
		// Keybinds (movidos para RPGKeybinds.cs)
		
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

                    // Ler dados de subclasses
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

                // Obsolete cases, handled by SyncRPGPlayer
                // case WolfgodrpgMessageType.SyncClass:
                // case WolfgodrpgMessageType.SyncClassLevel:
                //     playerID = reader.ReadByte();
                //     modPlayer = Main.player[playerID].GetModPlayer<Common.Players.RPGPlayer>();
                //     string className = reader.ReadString();
                //     float level = reader.ReadSingle();
                //     float experience = reader.ReadSingle();
                //     modPlayer.ClassLevels[className] = level;
                //     modPlayer.ClassExperience[className] = experience;
                //     if (Main.netMode == NetmodeID.Server)
                //     {
                //         var packet = GetPacket();
                //         packet.Write((byte)msgType);
                //         packet.Write(playerID);
                //         packet.Write(className);
                //         packet.Write(level);
                //         packet.Write(experience);
                //         packet.Send(-1, whoAmI);
                //     }
                //     break;
            }
        }

		private void LogRPGSystems()
		{
			Logger.Info("Sistemas RPG carregados:");
			Logger.Info("- ModPlayer: RPGPlayer");
			Logger.Info("- GlobalNPC: BalancedNPC");
			Logger.Info("- GlobalItem: ProgressiveItem, RPGGlobalItem, RPGWeaponProficiencyHooks");
			Logger.Info("- GlobalProjectile: RPGProjectileProficiencyHooks");
			Logger.Info("- GlobalRecipe: RPGGlobalRecipe");
			Logger.Info("- GlobalTile: RPGGlobalTile");
			Logger.Info("- UIState: MasterUIState (Unified)");
			Logger.Info("- Systems: PlayerVitalsSystem, RPGActionSystem, RPGCalculations, RPGConfig, RPGFishingProjectile, RPGHooks, RPGKeybinds, RPGDebugSystem, WolfgodUISystem");
		}
	}
}

