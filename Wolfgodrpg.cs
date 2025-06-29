using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Wolfgodrpg.Common.Systems;

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

