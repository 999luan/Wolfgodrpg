using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Players;
using Terraria.ModLoader; // Necessário para DamageClass
using Terraria.ID; // Necessário para BuffID

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema para processar efeitos especiais dos milestones de classe.
    /// </summary>
    public static class RPGMilestoneEffects
    {
        /// <summary>
        /// Processa efeitos especiais baseados no milestone atual da classe.
        /// </summary>
        /// <param name="player">Jogador RPG</param>
        /// <param name="className">Nome da classe</param>
        /// <param name="classLevel">Nível da classe</param>
        public static void ProcessSpecialEffects(RPGPlayer player, string className, float classLevel)
        {
            // Processar efeitos específicos por classe
            switch (className.ToLower())
            {
                case "warrior":
                    ProcessWarriorEffects(player, classLevel);
                    break;
                case "archer":
                    ProcessArcherEffects(player, classLevel);
                    break;
                case "mage":
                    ProcessMageEffects(player, classLevel);
                    break;
                case "acrobat":
                    ProcessAcrobatEffects(player, classLevel);
                    break;
                case "alchemist":
                    ProcessAlchemistEffects(player, classLevel);
                    break;
                case "mystic":
                    ProcessMysticEffects(player, classLevel);
                    break;
                case "engineer":
                    ProcessEngineerEffects(player, classLevel);
                    break;
                case "survivalist":
                    ProcessSurvivalistEffects(player, classLevel);
                    break;
                case "blacksmith":
                    ProcessBlacksmithEffects(player, classLevel);
                    break;
                case "explorer":
                    ProcessExplorerEffects(player, classLevel);
                    break;
                case "summoner":
                    ProcessSummonerEffects(player, classLevel);
                    break;
            }
        }

        private static void ProcessWarriorEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Guerreiro
            if (classLevel >= 10) // Milestone 10: Berserker Rage
            {
                // Aumentar dano quando vida baixa
                if (player.Player.statLife < player.Player.statLifeMax2 * 0.3f)
                {
                    player.Player.GetDamage(DamageClass.Melee) += 0.5f;
                }
            }

            if (classLevel >= 25) // Milestone 25: Battle Hardened
            {
                // Reduzir dano recebido baseado na defesa
                float defenseBonus = player.Player.statDefense * 0.01f;
                player.Player.endurance += defenseBonus;
            }
        }

        private static void ProcessArcherEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Arqueiro
            if (classLevel >= 15) // Milestone 15: Eagle Eye
            {
                // Aumentar precisão baseado na destreza
                float accuracyBonus = player.Dexterity * 0.002f;
                player.Player.GetDamage(DamageClass.Ranged) += accuracyBonus;
            }

            if (classLevel >= 30) // Milestone 30: Rapid Fire
            {
                // Aumentar velocidade de ataque
                player.Player.GetAttackSpeed(DamageClass.Ranged) += 0.2f;
            }
        }

        private static void ProcessMageEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Mago
            if (classLevel >= 20) // Milestone 20: Mana Surge
            {
                // Regeneração de mana melhorada
                if (player.Player.statMana < player.Player.statManaMax2)
                {
                    player.Player.manaRegen += 2;
                }
            }

            if (classLevel >= 35) // Milestone 35: Spell Mastery
            {
                // Reduzir custo de mana
                player.Player.manaCost -= 0.1f;
            }
        }

        private static void ProcessAcrobatEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Acrobata
            if (classLevel >= 10) // Milestone 10: +1 Dash
            {
                // Já implementado no sistema de dash
            }

            if (classLevel >= 20) // Milestone 20: Wall Jump
            {
                // Permitir wall jump
                // player.Player.wallJump = true; // Propriedade inexistente, precisa de implementação customizada
            }

            if (classLevel >= 30) // Milestone 30: Double Jump
            {
                // Permitir double jump
                // player.Player.doubleJump = true; // Propriedade inexistente, precisa de implementação customizada
            }
        }

        private static void ProcessAlchemistEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Alquimista
            if (classLevel >= 15) // Milestone 15: Potion Mastery
            {
                // Aumentar duração de poções
                player.Player.potionDelayTime = (int)(player.Player.potionDelayTime * 0.8f);
            }

            if (classLevel >= 25) // Milestone 25: Toxic Immunity
            {
                // Imunidade a venenos
                player.Player.buffImmune[BuffID.Poisoned] = true;
                player.Player.buffImmune[BuffID.Venom] = true;
            }
        }

        private static void ProcessMysticEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Místico
            if (classLevel >= 20) // Milestone 20: Spirit Sight
            {
                // Detectar inimigos invisíveis
                player.Player.detectCreature = true;
            }

            if (classLevel >= 35) // Milestone 35: Ethereal Form
            {
                // Chance de esquiva baseada na sabedoria
                float dodgeChance = player.Wisdom * 0.01f;
                if (Main.rand.NextFloat() < dodgeChance)
                {
                    // Implementar esquiva
                }
            }
        }

        private static void ProcessEngineerEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Engenheiro
            if (classLevel >= 15) // Milestone 15: Mechanical Insight
            {
                // Aumentar eficiência de ferramentas
                player.Player.pickSpeed -= 0.1f;
                // player.Player.axe -= 1; // Propriedade inexistente, não faz sentido em Player
            }

            if (classLevel >= 30) // Milestone 30: Automation
            {
                // Auto-uso de poções quando vida baixa
                if (player.Player.statLife < player.Player.statLifeMax2 * 0.3f)
                {
                    // Implementar auto-cura
                }
            }
        }

        private static void ProcessSurvivalistEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Sobrevivente
            if (classLevel >= 10) // Milestone 10: Wilderness Expert
            {
                // Reduzir consumo de fome/sanidade
                player.HungerRegenRate *= 0.8f;
                player.SanityRegenRate *= 0.8f;
            }

            if (classLevel >= 25) // Milestone 25: Natural Healing
            {
                // Regeneração natural melhorada
                if (player.Player.statLife < player.Player.statLifeMax2)
                {
                    player.Player.lifeRegen += 2;
                }
            }
        }

        private static void ProcessBlacksmithEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Ferreiro
            if (classLevel >= 15) // Milestone 15: Master Craftsman
            {
                // Aumentar durabilidade de itens
                // Implementar sistema de durabilidade
            }

            if (classLevel >= 30) // Milestone 30: Weapon Mastery
            {
                // Bônus de dano para armas corpo a corpo
                player.Player.GetDamage(DamageClass.Melee) += 0.2f;
            }
        }

        private static void ProcessExplorerEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Explorador
            if (classLevel >= 10) // Milestone 10: Treasure Hunter
            {
                // Aumentar chance de encontrar tesouros
                // Implementar sistema de tesouros
            }

            if (classLevel >= 25) // Milestone 25: Cartographer
            {
                // Revelar mais do mapa
                player.Player.accOreFinder = true;
            }
        }

        private static void ProcessSummonerEffects(RPGPlayer player, float classLevel)
        {
            // Efeitos especiais do Invocador
            if (classLevel >= 15) // Milestone 15: Summoner's Bond
            {
                // Aumentar dano dos minions baseado na sabedoria
                float wisdomBonus = player.Wisdom * 0.01f;
                player.Player.GetDamage(DamageClass.Summon) += wisdomBonus;
            }

            if (classLevel >= 30) // Milestone 30: Horde Master
            {
                // Aumentar número máximo de minions
                player.Player.maxMinions += 1;
            }
        }
    }
} 