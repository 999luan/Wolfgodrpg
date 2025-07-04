using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema de notificações para mostrar ganhos de XP e level ups.
    /// Baseado nas melhores práticas do tModLoader.
    /// </summary>
    public class RPGNotificationSystem : ModSystem
    {
        /// <summary>
        /// Adiciona uma notificação de ganho de XP.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="xpGained">XP ganho</param>
        /// <param name="newLevel">Novo nível (0 se não subiu de nível)</param>
        public static void AddXPNotification(string className, float xpGained, int newLevel = 0)
        {
            string message = $"+{xpGained:F0} XP {GetClassNameDisplay(className)}";
            if (newLevel > 0)
            {
                message += $" (Level {newLevel}!)";
            }
            // Acumular log no jogador local
            if (Main.LocalPlayer != null && Main.LocalPlayer.active)
            {
                var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                modPlayer?.AddXPLog(message);
            }
        }

        /// <summary>
        /// Adiciona uma notificação de level up.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="newLevel">Novo nível</param>
        public static void AddLevelUpNotification(string className, int newLevel)
        {
            string message = $"🎉 {GetClassNameDisplay(className)} Level {newLevel}!";
            // Acumular log no jogador local
            if (Main.LocalPlayer != null && Main.LocalPlayer.active)
            {
                var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                modPlayer?.AddXPLog(message);
            }
        }

        public static void ShowXPLogs()
        {
            var player = Main.LocalPlayer;
            if (player?.active != true) return;

            var rpgPlayer = player.GetModPlayer<RPGPlayer>();
            if (rpgPlayer == null) return;

            if (rpgPlayer.XPLogs.Count == 0)
            {
                return; // Não exibir mensagem se não há logs
            }

            // Exibir todos os logs acumulados automaticamente
            foreach (string log in rpgPlayer.XPLogs)
            {
                Main.NewText(log, Color.LightBlue);
            }

            // Limpar os logs após exibição
            rpgPlayer.ClearXPLogs();
        }

        /// <summary>
        /// Obtém o nome de exibição da classe.
        /// </summary>
        private static string GetClassNameDisplay(string className)
        {
            return className switch
            {
                "warrior" => "Warrior",
                "archer" => "Archer",
                "mage" => "Mage",
                "summoner" => "Summoner",
                "acrobat" => "Acrobat",
                "explorer" => "Explorer",
                "engineer" => "Engineer",
                "survivalist" => "Survivalist",
                "blacksmith" => "Blacksmith",
                "alchemist" => "Alchemist",
                "mystic" => "Mystic",
                _ => className
            };
        }

        /// <summary>
        /// Obtém a cor da classe.
        /// </summary>
        private static Color GetClassColor(string className)
        {
            return className switch
            {
                "warrior" => Color.Red,
                "archer" => Color.Green,
                "mage" => Color.Purple,
                "summoner" => Color.Orange,
                "acrobat" => Color.Cyan,
                "explorer" => Color.Yellow,
                "engineer" => Color.Gray,
                "survivalist" => Color.Brown,
                "blacksmith" => Color.DarkOrange,
                "alchemist" => Color.Lime,
                "mystic" => Color.Pink,
                _ => Color.White
            };
        }
    }
} 