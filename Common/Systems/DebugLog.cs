using Terraria;
using Terraria.ModLoader;
using System;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema centralizado de debug log para o mod WolfGod RPG.
    /// Permite logs detalhados de todas as áreas do mod para facilitar debug e desenvolvimento.
    /// </summary>
    public static class DebugLog
    {
        private static readonly string ModPrefix = "[WolfGodRPG]";
        
        // Configuração de logs por área (pode ser expandida para config do mod)
        private static bool EnablePlayerLogs = true;
        private static bool EnableNPCLogs = true;
        private static bool EnableItemLogs = true;
        private static bool EnableUILogs = true;
        private static bool EnableSystemLogs = true;
        private static bool EnableGameplayLogs = true;

        /// <summary>
        /// Log de informação geral
        /// </summary>
        public static void Info(string area, string method, string message)
        {
            if (ShouldLog(area))
            {
                string logMessage = $"{ModPrefix}[{area}][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log de aviso
        /// </summary>
        public static void Warn(string area, string method, string message)
        {
            if (ShouldLog(area))
            {
                string logMessage = $"{ModPrefix}[{area}][{method}] WARNING: {message}";
                Wolfgodrpg.Instance?.Logger.Warn(logMessage);
            }
        }

        /// <summary>
        /// Log de erro
        /// </summary>
        public static void Error(string area, string method, string message, Exception ex = null)
        {
            if (ShouldLog(area))
            {
                string logMessage = $"{ModPrefix}[{area}][{method}] ERROR: {message}";
                if (ex != null)
                {
                    logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                }
                Wolfgodrpg.Instance?.Logger.Error(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de gameplay (XP, level up, etc.)
        /// </summary>
        public static void Gameplay(string area, string method, string message)
        {
            if (EnableGameplayLogs && ShouldLog(area))
            {
                string logMessage = $"{ModPrefix}[{area}][{method}] GAMEPLAY: {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de UI
        /// </summary>
        public static void UI(string method, string message)
        {
            if (EnableUILogs)
            {
                string logMessage = $"{ModPrefix}[UI][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de Player
        /// </summary>
        public static void Player(string method, string message)
        {
            if (EnablePlayerLogs)
            {
                string logMessage = $"{ModPrefix}[Player][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de NPC
        /// </summary>
        public static void NPC(string method, string message)
        {
            if (EnableNPCLogs)
            {
                string logMessage = $"{ModPrefix}[NPC][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de Item
        /// </summary>
        public static void Item(string method, string message)
        {
            if (EnableItemLogs)
            {
                string logMessage = $"{ModPrefix}[Item][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Log específico para eventos de Sistema
        /// </summary>
        public static void System(string method, string message)
        {
            if (EnableSystemLogs)
            {
                string logMessage = $"{ModPrefix}[System][{method}] {message}";
                Wolfgodrpg.Instance?.Logger.Info(logMessage);
            }
        }

        /// <summary>
        /// Verifica se deve logar baseado na área
        /// </summary>
        private static bool ShouldLog(string area)
        {
            return area.ToLower() switch
            {
                "player" => EnablePlayerLogs,
                "npc" => EnableNPCLogs,
                "item" => EnableItemLogs,
                "ui" => EnableUILogs,
                "system" => EnableSystemLogs,
                _ => true // Loga por padrão se a área não for reconhecida
            };
        }

        /// <summary>
        /// Ativa/desativa logs por área
        /// </summary>
        public static void SetLogging(string area, bool enabled)
        {
            switch (area.ToLower())
            {
                case "player":
                    EnablePlayerLogs = enabled;
                    break;
                case "npc":
                    EnableNPCLogs = enabled;
                    break;
                case "item":
                    EnableItemLogs = enabled;
                    break;
                case "ui":
                    EnableUILogs = enabled;
                    break;
                case "system":
                    EnableSystemLogs = enabled;
                    break;
                case "gameplay":
                    EnableGameplayLogs = enabled;
                    break;
            }
        }

        /// <summary>
        /// Ativa todos os logs
        /// </summary>
        public static void EnableAllLogs()
        {
            EnablePlayerLogs = true;
            EnableNPCLogs = true;
            EnableItemLogs = true;
            EnableUILogs = true;
            EnableSystemLogs = true;
            EnableGameplayLogs = true;
        }

        /// <summary>
        /// Desativa todos os logs
        /// </summary>
        public static void DisableAllLogs()
        {
            EnablePlayerLogs = false;
            EnableNPCLogs = false;
            EnableItemLogs = false;
            EnableUILogs = false;
            EnableSystemLogs = false;
            EnableGameplayLogs = false;
        }
    }
} 