using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema modular para gerenciar atributos do jogador
    /// </summary>
    public class AttributesSystem
    {
        // === CONSTANTES ===
        public const int MIN_ATTRIBUTE = 1;
        public const int MAX_ATTRIBUTE = 100;
        public const int BASE_ATTRIBUTE = 10;

        // === EVENTOS ===
        public event Action OnStrengthChanged;
        public event Action OnDexterityChanged;
        public event Action OnIntelligenceChanged;
        public event Action OnConstitutionChanged;
        public event Action OnWisdomChanged;
        public event Action OnAttributePointsChanged;
        public event Action OnLevelUp;

        // === PROPRIEDADES PRIVADAS ===
        private int strength;
        private int dexterity;
        private int intelligence;
        private int constitution;
        private int wisdom;
        private int attributePoints;
        private int playerLevel;
        private float playerExperience;

        // === PROPRIEDADES PÚBLICAS COM VALIDAÇÃO ===
        public int Strength
        {
            get => strength;
            set
            {
                int oldValue = strength;
                strength = (int)MathHelper.Clamp(value, MIN_ATTRIBUTE, MAX_ATTRIBUTE);
                
                if (oldValue != strength)
                {
                    OnStrengthChanged?.Invoke();
                }
            }
        }

        public int Dexterity
        {
            get => dexterity;
            set
            {
                int oldValue = dexterity;
                dexterity = (int)MathHelper.Clamp(value, MIN_ATTRIBUTE, MAX_ATTRIBUTE);
                
                if (oldValue != dexterity)
                {
                    OnDexterityChanged?.Invoke();
                }
            }
        }

        public int Intelligence
        {
            get => intelligence;
            set
            {
                int oldValue = intelligence;
                intelligence = (int)MathHelper.Clamp(value, MIN_ATTRIBUTE, MAX_ATTRIBUTE);
                
                if (oldValue != intelligence)
                {
                    OnIntelligenceChanged?.Invoke();
                }
            }
        }

        public int Constitution
        {
            get => constitution;
            set
            {
                int oldValue = constitution;
                constitution = (int)MathHelper.Clamp(value, MIN_ATTRIBUTE, MAX_ATTRIBUTE);
                
                if (oldValue != constitution)
                {
                    OnConstitutionChanged?.Invoke();
                }
            }
        }

        public int Wisdom
        {
            get => wisdom;
            set
            {
                int oldValue = wisdom;
                wisdom = (int)MathHelper.Clamp(value, MIN_ATTRIBUTE, MAX_ATTRIBUTE);
                
                if (oldValue != wisdom)
                {
                    OnWisdomChanged?.Invoke();
                }
            }
        }

        public int AttributePoints
        {
            get => attributePoints;
            set
            {
                int oldValue = attributePoints;
                attributePoints = Math.Max(0, value);
                
                if (oldValue != attributePoints)
                {
                    OnAttributePointsChanged?.Invoke();
                }
            }
        }

        public int PlayerLevel
        {
            get => playerLevel;
            set
            {
                int oldValue = playerLevel;
                playerLevel = Math.Max(1, value);
                
                if (oldValue != playerLevel)
                {
                    OnLevelUp?.Invoke();
                }
            }
        }

        public float PlayerExperience
        {
            get => playerExperience;
            set
            {
                playerExperience = Math.Max(0f, value);
                CheckLevelUp();
            }
        }

        // === CONSTRUTOR ===
        public AttributesSystem()
        {
            // Inicializar com valores base
            strength = BASE_ATTRIBUTE;
            dexterity = BASE_ATTRIBUTE;
            intelligence = BASE_ATTRIBUTE;
            constitution = BASE_ATTRIBUTE;
            wisdom = BASE_ATTRIBUTE;
            attributePoints = 0;
            playerLevel = 1;
            playerExperience = 0f;
        }

        // === MÉTODOS DE ATRIBUIÇÃO ===
        /// <summary>
        /// Tenta aumentar um atributo usando pontos disponíveis
        /// </summary>
        public bool TryIncreaseAttribute(string attributeName)
        {
            if (AttributePoints <= 0) return false;

            switch (attributeName.ToLower())
            {
                case "strength":
                    if (Strength < MAX_ATTRIBUTE)
                    {
                        Strength++;
                        AttributePoints--;
                        return true;
                    }
                    break;
                case "dexterity":
                    if (Dexterity < MAX_ATTRIBUTE)
                    {
                        Dexterity++;
                        AttributePoints--;
                        return true;
                    }
                    break;
                case "intelligence":
                    if (Intelligence < MAX_ATTRIBUTE)
                    {
                        Intelligence++;
                        AttributePoints--;
                        return true;
                    }
                    break;
                case "constitution":
                    if (Constitution < MAX_ATTRIBUTE)
                    {
                        Constitution++;
                        AttributePoints--;
                        return true;
                    }
                    break;
                case "wisdom":
                    if (Wisdom < MAX_ATTRIBUTE)
                    {
                        Wisdom++;
                        AttributePoints--;
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Adiciona experiência e verifica level up
        /// </summary>
        public void AddExperience(float experience)
        {
            PlayerExperience += experience;
        }

        /// <summary>
        /// Verifica se o jogador subiu de nível
        /// </summary>
        private void CheckLevelUp()
        {
            float experienceNeeded = GetExperienceForLevel(PlayerLevel + 1);
            
            while (PlayerExperience >= experienceNeeded)
            {
                PlayerExperience -= experienceNeeded;
                PlayerLevel++;
                
                // Dar pontos de atributo no level up
                AttributePoints += 2;
                
                // Notificar o jogador
                Main.NewText($"Level Up! Você agora é nível {PlayerLevel}!", Color.Green);
                Main.NewText("Você ganhou 2 pontos de atributo!", Color.Yellow);
                
                // Recalcular experiência necessária para o próximo nível
                experienceNeeded = GetExperienceForLevel(PlayerLevel + 1);
            }
        }

        /// <summary>
        /// Calcula experiência necessária para um nível
        /// </summary>
        public static float GetExperienceForLevel(int level)
        {
            return level * 1000f + (level * level * 100f);
        }

        // === MÉTODOS DE CÁLCULO DE BÔNUS ===
        /// <summary>
        /// Calcula bônus de dano corpo a corpo baseado na força
        /// </summary>
        public float GetMeleeDamageBonus()
        {
            return (Strength - BASE_ATTRIBUTE) * 0.02f; // 2% por ponto acima da base
        }

        /// <summary>
        /// Calcula bônus de dano à distância baseado na destreza
        /// </summary>
        public float GetRangedDamageBonus()
        {
            return (Dexterity - BASE_ATTRIBUTE) * 0.02f;
        }

        /// <summary>
        /// Calcula bônus de dano mágico baseado na inteligência
        /// </summary>
        public float GetMagicDamageBonus()
        {
            return (Intelligence - BASE_ATTRIBUTE) * 0.02f;
        }

        /// <summary>
        /// Calcula bônus de vida máxima baseado na constituição
        /// </summary>
        public int GetMaxHealthBonus()
        {
            return (Constitution - BASE_ATTRIBUTE) * 2; // 2 HP por ponto
        }

        /// <summary>
        /// Calcula bônus de mana máxima baseado na inteligência
        /// </summary>
        public int GetMaxManaBonus()
        {
            return (Intelligence - BASE_ATTRIBUTE) * 3; // 3 mana por ponto
        }

        /// <summary>
        /// Calcula bônus de defesa baseado na constituição
        /// </summary>
        public int GetDefenseBonus()
        {
            return (Constitution - BASE_ATTRIBUTE) / 2; // 0.5 defesa por ponto
        }

        /// <summary>
        /// Calcula bônus de velocidade de movimento baseado na destreza
        /// </summary>
        public float GetMovementSpeedBonus()
        {
            return (Dexterity - BASE_ATTRIBUTE) * 0.01f; // 1% por ponto
        }

        /// <summary>
        /// Calcula bônus de chance crítica baseado na destreza
        /// </summary>
        public float GetCriticalChanceBonus()
        {
            return (Dexterity - BASE_ATTRIBUTE) * 0.005f; // 0.5% por ponto
        }

        // === MÉTODOS DE ESTADO ===
        /// <summary>
        /// Retorna o total de pontos gastos em atributos
        /// </summary>
        public int GetTotalAttributePointsSpent()
        {
            return (Strength - BASE_ATTRIBUTE) +
                   (Dexterity - BASE_ATTRIBUTE) +
                   (Intelligence - BASE_ATTRIBUTE) +
                   (Constitution - BASE_ATTRIBUTE) +
                   (Wisdom - BASE_ATTRIBUTE);
        }

        /// <summary>
        /// Retorna se o jogador pode aumentar um atributo
        /// </summary>
        public bool CanIncreaseAttribute(string attributeName)
        {
            if (AttributePoints <= 0) return false;

            return attributeName.ToLower() switch
            {
                "strength" => Strength < MAX_ATTRIBUTE,
                "dexterity" => Dexterity < MAX_ATTRIBUTE,
                "intelligence" => Intelligence < MAX_ATTRIBUTE,
                "constitution" => Constitution < MAX_ATTRIBUTE,
                "wisdom" => Wisdom < MAX_ATTRIBUTE,
                _ => false
            };
        }

        /// <summary>
        /// Retorna string formatada com status dos atributos
        /// </summary>
        public string GetAttributesStatus()
        {
            return $"Level: {PlayerLevel} | XP: {PlayerExperience:F0}/{GetExperienceForLevel(PlayerLevel + 1):F0} | " +
                   $"Points: {AttributePoints} | " +
                   $"STR: {Strength} | DEX: {Dexterity} | INT: {Intelligence} | CON: {Constitution} | WIS: {Wisdom}";
        }

        /// <summary>
        /// Loga o status atual dos atributos
        /// </summary>
        public void LogAttributesStatus()
        {
            Main.NewText($"Attributes Status: {GetAttributesStatus()}", Color.Cyan);
        }

        // === MÉTODOS DE RESET ===
        /// <summary>
        /// Reseta todos os atributos para valores base
        /// </summary>
        public void ResetToBase()
        {
            Strength = BASE_ATTRIBUTE;
            Dexterity = BASE_ATTRIBUTE;
            Intelligence = BASE_ATTRIBUTE;
            Constitution = BASE_ATTRIBUTE;
            Wisdom = BASE_ATTRIBUTE;
            AttributePoints = 0;
        }
    }
} 