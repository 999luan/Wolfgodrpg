using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
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
			InitializeRPGSystems();
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
		
		private void InitializeRPGSystems()
		{
			// Aqui inicializaremos os sistemas de RPG conforme formos criando
			Logger.Info("Sistemas RPG inicializados:");
			Logger.Info("- Sistema de Stats de Jogador (RPGPlayer)");
			Logger.Info("- Sistema de Balanceamento de NPCs (BalancedNPC)");
			Logger.Info("- Sistema de Progressão de Itens (ProgressiveItem)");
			Logger.Info("- Sistema de Interface de Stats (RPGStatsUI)");
			Logger.Info("- Sistema de Teclas de Atalho (RPGKeybinds)");
			Logger.Info("- Tecla 'R' para mostrar stats RPG");
		}
	}
}

