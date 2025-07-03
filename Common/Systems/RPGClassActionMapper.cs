using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Wolfgodrpg.Common.Players;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema que mapeia ações do jogador para as classes RPG corretas.
    /// Baseado na documentação oficial do tModLoader.
    /// </summary>
    public static class RPGClassActionMapper
    {
        /// <summary>
        /// Mapeia o tipo de dano para a classe de combate correspondente.
        /// </summary>
        /// <param name="damageType">Tipo de dano</param>
        /// <returns>Nome da classe</returns>
        public static string MapDamageTypeToClass(DamageClass damageType)
        {
            return damageType switch
            {
                DamageClass.Melee => "warrior",
                DamageClass.Ranged => "archer", 
                DamageClass.Magic => "mage",
                DamageClass.Summon => "summoner",
                _ => "warrior" // Fallback
            };
        }

        /// <summary>
        /// Mapeia ações de movimento para a classe Acrobata.
        /// </summary>
        /// <param name="action">Tipo de ação de movimento</param>
        /// <param name="value">Valor da ação (distância, etc.)</param>
        public static void MapMovementAction(MovementAction action, float value)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            switch (action)
            {
                case MovementAction.Dash:
                    rpgPlayer.AddClassExperience("acrobat", value * 0.1f);
                    break;
                case MovementAction.Jump:
                    rpgPlayer.AddClassExperience("acrobat", 2f);
                    break;
                case MovementAction.Walk:
                    rpgPlayer.AddClassExperience("acrobat", value * 0.001f);
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de combate para as classes correspondentes.
        /// </summary>
        /// <param name="action">Tipo de ação de combate</param>
        /// <param name="damage">Dano causado</param>
        /// <param name="damageType">Tipo de dano</param>
        public static void MapCombatAction(CombatAction action, int damage, DamageClass damageType)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            string className = MapDamageTypeToClass(damageType);
            float xpAmount = damage * 0.1f;

            switch (action)
            {
                case CombatAction.HitNPC:
                    rpgPlayer.AddClassExperience(className, xpAmount);
                    break;
                case CombatAction.KillNPC:
                    rpgPlayer.AddClassExperience(className, xpAmount * 2f);
                    break;
                case CombatAction.TakeDamage:
                    rpgPlayer.AddClassExperience("warrior", xpAmount * 0.5f); // Guerreiro ganha XP por tankar
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de crafting para as classes correspondentes.
        /// </summary>
        /// <param name="action">Tipo de ação de crafting</param>
        /// <param name="item">Item craftado</param>
        public static void MapCraftingAction(CraftingAction action, Item item)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            float baseXP = item.value * 0.01f;

            switch (action)
            {
                case CraftingAction.CraftWeapon:
                    // Armas dão XP para ferreiro
                    rpgPlayer.AddClassExperience("blacksmith", baseXP);
                    break;
                case CraftingAction.CraftArmor:
                    // Armaduras dão XP para ferreiro
                    rpgPlayer.AddClassExperience("blacksmith", baseXP);
                    break;
                case CraftingAction.CraftPotion:
                    // Poções dão XP para alquimista
                    rpgPlayer.AddClassExperience("alchemist", baseXP);
                    break;
                case CraftingAction.CraftBuilding:
                    // Blocos/construção dão XP para engenheiro
                    rpgPlayer.AddClassExperience("engineer", baseXP);
                    break;
                case CraftingAction.CraftTool:
                    // Ferramentas dão XP para ferreiro
                    rpgPlayer.AddClassExperience("blacksmith", baseXP);
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de exploração para a classe Explorador.
        /// </summary>
        /// <param name="action">Tipo de ação de exploração</param>
        /// <param name="value">Valor da ação</param>
        public static void MapExplorationAction(ExplorationAction action, float value)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            switch (action)
            {
                case ExplorationAction.DiscoverBiome:
                    rpgPlayer.AddClassExperience("explorer", 50f);
                    break;
                case ExplorationAction.FindTreasure:
                    rpgPlayer.AddClassExperience("explorer", value * 0.1f);
                    break;
                case ExplorationAction.MineResource:
                    rpgPlayer.AddClassExperience("explorer", 5f);
                    break;
                case ExplorationAction.TravelDistance:
                    rpgPlayer.AddClassExperience("explorer", value * 0.001f);
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de sobrevivência para a classe Sobrevivente.
        /// </summary>
        /// <param name="action">Tipo de ação de sobrevivência</param>
        /// <param name="value">Valor da ação</param>
        public static void MapSurvivalAction(SurvivalAction action, float value)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            switch (action)
            {
                case SurvivalAction.EatFood:
                    rpgPlayer.AddClassExperience("survivalist", 10f);
                    break;
                case SurvivalAction.RegenerateHealth:
                    rpgPlayer.AddClassExperience("survivalist", value * 0.1f);
                    break;
                case SurvivalAction.EnvironmentalResistance:
                    rpgPlayer.AddClassExperience("survivalist", 5f);
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de pesca para a classe correspondente.
        /// </summary>
        /// <param name="action">Tipo de ação de pesca</param>
        /// <param name="value">Valor da ação</param>
        public static void MapFishingAction(FishingAction action, float value)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            switch (action)
            {
                case FishingAction.CatchFish:
                    // Pesca dá XP para sobrevivente (habilidade de sobrevivência)
                    rpgPlayer.AddClassExperience("survivalist", value * 0.1f);
                    break;
                case FishingAction.CatchRareFish:
                    // Peixes raros dão mais XP
                    rpgPlayer.AddClassExperience("survivalist", value * 0.5f);
                    break;
            }
        }

        /// <summary>
        /// Mapeia ações de comércio para a classe correspondente.
        /// </summary>
        /// <param name="action">Tipo de ação de comércio</param>
        /// <param name="value">Valor da transação</param>
        public static void MapTradeAction(TradeAction action, float value)
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            float xpAmount = value * 0.0001f;

            switch (action)
            {
                case TradeAction.BuyItem:
                    // Compras dão XP para explorador (descoberta de itens)
                    rpgPlayer.AddClassExperience("explorer", xpAmount);
                    break;
                case TradeAction.SellItem:
                    // Vendas dão XP para explorador (negociação)
                    rpgPlayer.AddClassExperience("explorer", xpAmount);
                    break;
            }
        }
    }

    // Enums para tipos de ações
    public enum MovementAction
    {
        Dash,
        Jump,
        Walk
    }

    public enum CombatAction
    {
        HitNPC,
        KillNPC,
        TakeDamage
    }

    public enum CraftingAction
    {
        CraftWeapon,
        CraftArmor,
        CraftPotion,
        CraftBuilding,
        CraftTool
    }

    public enum ExplorationAction
    {
        DiscoverBiome,
        FindTreasure,
        MineResource,
        TravelDistance
    }

    public enum SurvivalAction
    {
        EatFood,
        RegenerateHealth,
        EnvironmentalResistance
    }

    public enum FishingAction
    {
        CatchFish,
        CatchRareFish
    }

    public enum TradeAction
    {
        BuyItem,
        SellItem
    }
} 