using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wolfgodrpg.Common.Classes
{
    public class RPGClassDefinitions
    {
        // Raridades de Item
        public enum ItemRarity
        {
            Common = 0,     // Branco
            Uncommon = 1,   // Verde
            Rare = 2,       // Azul
            Epic = 3,       // Roxo
            Legendary = 4   // Laranja
        }

        // Classes baseadas em ações
        public static readonly Dictionary<string, ClassInfo> ActionClasses = new Dictionary<string, ClassInfo>
        {
            // Movimento
            {"movement", new ClassInfo(
                "Movimento",
                "Mestre da mobilidade",
                new Dictionary<string, float> {
                    {"moveSpeed", 0.002f},      // +0.2% velocidade por nível
                    {"acceleration", 0.002f},    // +0.2% aceleração por nível
                    {"jumpHeight", 0.002f},      // +0.2% altura do pulo por nível
                    {"fallDamageReduction", 0.002f}, // +0.2% redução de dano de queda por nível
                    {"staminaRegen", 0.002f},    // +0.2% regeneração de stamina por nível
                },
                new Dictionary<int, string> {
                    {25, "Dash mais rápido"},
                    {50, "Dash duplo"},
                    {75, "Dash não gasta stamina"},
                    {100, "Dash infinito"}
                }
            )},

            // Pulo
            {"jumping", new ClassInfo(
                "Pulo",
                "Mestre dos saltos",
                new Dictionary<string, float> {
                    {"jumpHeight", 0.003f},      // +0.3% altura do pulo por nível
                    {"jumpControl", 0.002f},     // +0.2% controle no ar por nível
                    {"fallSpeed", -0.001f},      // -0.1% velocidade de queda por nível
                    {"wingTime", 0.002f},        // +0.2% tempo de voo por nível
                },
                new Dictionary<int, string> {
                    {25, "Pulo mais alto"},
                    {50, "Pulo duplo"},
                    {75, "Pulo triplo"},
                    {100, "Pulo infinito"}
                }
            )},

            // Combate Corpo a Corpo
            {"melee", new ClassInfo(
                "Combate Corpo a Corpo",
                "Mestre do combate próximo",
                new Dictionary<string, float> {
                    {"meleeDamage", 0.002f},     // +0.2% dano corpo a corpo por nível
                    {"meleeSpeed", 0.002f},      // +0.2% velocidade de ataque por nível
                    {"meleeKnockback", 0.002f},  // +0.2% knockback por nível
                    {"meleeCrit", 0.001f},       // +0.1% chance crítica por nível
                    {"lifeSteal", 0.0005f},      // +0.05% roubo de vida por nível
                },
                new Dictionary<int, string> {
                    {25, "Combo básico"},
                    {50, "Combo avançado"},
                    {75, "Combo mestre"},
                    {100, "Combo supremo"}
                }
            )},

            // Combate à Distância
            {"ranged", new ClassInfo(
                "Combate à Distância",
                "Mestre do combate ranged",
                new Dictionary<string, float> {
                    {"rangedDamage", 0.002f},    // +0.2% dano à distância por nível
                    {"rangedSpeed", 0.002f},     // +0.2% velocidade de projétil por nível
                    {"rangedCrit", 0.001f},      // +0.1% chance crítica por nível
                    {"ammoSave", 0.001f},        // +0.1% chance de não gastar munição por nível
                },
                new Dictionary<int, string> {
                    {25, "Tiro preciso"},
                    {50, "Tiro duplo"},
                    {75, "Tiro penetrante"},
                    {100, "Tiro supremo"}
                }
            )},

            // Magia
            {"magic", new ClassInfo(
                "Magia",
                "Mestre das artes arcanas",
                new Dictionary<string, float> {
                    {"magicDamage", 0.002f},     // +0.2% dano mágico por nível
                    {"manaCost", -0.001f},       // -0.1% custo de mana por nível
                    {"manaRegen", 0.002f},       // +0.2% regeneração de mana por nível
                    {"magicCrit", 0.001f},       // +0.1% chance crítica por nível
                },
                new Dictionary<int, string> {
                    {25, "Magia aprimorada"},
                    {50, "Magia dupla"},
                    {75, "Magia contínua"},
                    {100, "Magia suprema"}
                }
            )},

            // Invocação
            {"summon", new ClassInfo(
                "Invocação",
                "Mestre dos minions",
                new Dictionary<string, float> {
                    {"minionDamage", 0.002f},    // +0.2% dano de minion por nível
                    {"minionSlots", 0.02f},      // +0.02 slots de minion por nível
                    {"minionKnockback", 0.002f}, // +0.2% knockback de minion por nível
                    {"minionRange", 0.002f},     // +0.2% alcance de minion por nível
                },
                new Dictionary<int, string> {
                    {25, "Minion melhorado"},
                    {50, "Minion duplo"},
                    {75, "Minion supremo"},
                    {100, "Minion divino"}
                }
            )},

            // Mineração
            {"mining", new ClassInfo(
                "Mineração",
                "Mestre da mineração",
                new Dictionary<string, float> {
                    {"miningSpeed", 0.002f},     // +0.2% velocidade de mineração por nível
                    {"miningRange", 0.002f},     // +0.2% alcance de mineração por nível
                    {"gemFind", 0.001f},         // +0.1% chance de gemas por nível
                    {"oreQuality", 0.001f},      // +0.1% qualidade de minério por nível
                },
                new Dictionary<int, string> {
                    {25, "Mineração eficiente"},
                    {50, "Mineração em área"},
                    {75, "Mineração automática"},
                    {100, "Mineração suprema"}
                }
            )},

            // Construção
            {"building", new ClassInfo(
                "Construção",
                "Mestre da construção",
                new Dictionary<string, float> {
                    {"buildSpeed", 0.002f},      // +0.2% velocidade de construção por nível
                    {"buildRange", 0.002f},      // +0.2% alcance de construção por nível
                    {"materialSave", 0.001f},    // +0.1% chance de não gastar material por nível
                    {"buildQuality", 0.002f},    // +0.2% qualidade de construção por nível
                },
                new Dictionary<int, string> {
                    {25, "Construção rápida"},
                    {50, "Construção em área"},
                    {75, "Construção automática"},
                    {100, "Construção suprema"}
                }
            )},

            // Pesca
            {"fishing", new ClassInfo(
                "Pesca",
                "Mestre da pesca",
                new Dictionary<string, float> {
                    {"fishingPower", 0.002f},    // +0.2% poder de pesca por nível
                    {"catchRate", 0.002f},       // +0.2% taxa de captura por nível
                    {"rareFish", 0.001f},        // +0.1% chance de peixe raro por nível
                    {"crateLuck", 0.001f},       // +0.1% chance de baú por nível
                },
                new Dictionary<int, string> {
                    {25, "Pesca eficiente"},
                    {50, "Pesca dupla"},
                    {75, "Pesca rara"},
                    {100, "Pesca suprema"}
                }
            )},

            // Coleta
            {"gathering", new ClassInfo(
                "Coleta",
                "Mestre da coleta",
                new Dictionary<string, float> {
                    {"gatherSpeed", 0.002f},     // +0.2% velocidade de coleta por nível
                    {"gatherRange", 0.002f},     // +0.2% alcance de coleta por nível
                    {"doubleHarvest", 0.001f},   // +0.1% chance de coleta dupla por nível
                    {"rareFind", 0.001f},        // +0.1% chance de item raro por nível
                },
                new Dictionary<int, string> {
                    {25, "Coleta eficiente"},
                    {50, "Coleta em área"},
                    {75, "Coleta automática"},
                    {100, "Coleta suprema"}
                }
            )},

            // Bestiário
            {"bestiary", new ClassInfo(
                "Bestiário",
                "Mestre do conhecimento",
                new Dictionary<string, float> {
                    {"monsterDamage", 0.001f},   // +0.1% dano contra monstros por nível
                    {"monsterInfo", 0.002f},     // +0.2% informação no bestiário por nível
                    {"rareDrop", 0.001f},        // +0.1% chance de drop raro por nível
                    {"expGain", 0.001f},         // +0.1% ganho de experiência por nível
                },
                new Dictionary<int, string> {
                    {25, "Conhecimento básico"},
                    {50, "Conhecimento avançado"},
                    {75, "Conhecimento mestre"},
                    {100, "Conhecimento supremo"}
                }
            )},

            // Comerciante
            {"merchant", new ClassInfo(
                "Comerciante",
                "Mestre do comércio",
                new Dictionary<string, float> {
                    {"shopDiscount", 0.001f},    // +0.1% desconto em lojas por nível
                    {"sellPrice", 0.001f},       // +0.1% preço de venda por nível
                    {"rareStock", 0.001f},       // +0.1% chance de item raro em lojas por nível
                    {"haggle", 0.001f},          // +0.1% chance de barganha por nível
                },
                new Dictionary<int, string> {
                    {25, "Comerciante básico"},
                    {50, "Comerciante avançado"},
                    {75, "Comerciante mestre"},
                    {100, "Comerciante supremo"}
                }
            )},

            // Defesa
            {"defense", new ClassInfo(
                "Defesa",
                "Mestre da sobrevivência",
                new Dictionary<string, float> {
                    {"defense", 0.1f},           // +0.1 defesa por nível
                    {"damageReduction", 0.001f}, // +0.1% redução de dano por nível
                    {"lifeRegen", 0.001f},       // +0.1% regeneração de vida por nível
                    {"maxLife", 0.002f},         // +0.2% vida máxima por nível
                },
                new Dictionary<int, string> {
                    {25, "Defesa reforçada"},
                    {50, "Defesa aprimorada"},
                    {75, "Defesa mestre"},
                    {100, "Defesa suprema"}
                }
            )}
        };

        // Status aleatórios possíveis para itens
        public static readonly Dictionary<string, StatInfo> RandomStats = new Dictionary<string, StatInfo>
        {
            {"expGain", new StatInfo("Ganho de Experiência", 0.01f, 0.05f)},
            {"classExpGain", new StatInfo("Ganho de Exp. de Classe", 0.02f, 0.1f)},
            {"moveSpeed", new StatInfo("Velocidade de Movimento", 0.01f, 0.05f)},
            {"jumpBoost", new StatInfo("Altura do Pulo", 0.01f, 0.05f)},
            {"meleeDamage", new StatInfo("Dano Corpo a Corpo", 0.01f, 0.05f)},
            {"rangedDamage", new StatInfo("Dano à Distância", 0.01f, 0.05f)},
            {"magicDamage", new StatInfo("Dano Mágico", 0.01f, 0.05f)},
            {"minionDamage", new StatInfo("Dano de Minion", 0.01f, 0.05f)},
            {"critChance", new StatInfo("Chance Crítica", 0.01f, 0.05f)},
            {"defense", new StatInfo("Defesa", 1f, 5f)},
            {"lifeRegen", new StatInfo("Regeneração de Vida", 0.1f, 0.5f)},
            {"manaRegen", new StatInfo("Regeneração de Mana", 0.1f, 0.5f)},
            {"maxLife", new StatInfo("Vida Máxima", 5f, 25f)},
            {"maxMana", new StatInfo("Mana Máxima", 5f, 25f)},
            {"luck", new StatInfo("Sorte", 0.01f, 0.05f)}
        };

        // Número de status aleatórios por raridade
        public static readonly Dictionary<ItemRarity, int> StatsPerRarity = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, 0},
            {ItemRarity.Uncommon, 1},
            {ItemRarity.Rare, 2},
            {ItemRarity.Epic, 3},
            {ItemRarity.Legendary, 4}
        };

        // Multiplicador de valor dos status por raridade
        public static readonly Dictionary<ItemRarity, float> StatMultiplierPerRarity = new Dictionary<ItemRarity, float>
        {
            {ItemRarity.Common, 1.0f},
            {ItemRarity.Uncommon, 1.2f},
            {ItemRarity.Rare, 1.5f},
            {ItemRarity.Epic, 2.0f},
            {ItemRarity.Legendary, 3.0f}
        };
    }

    public class ClassInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, float> StatBonuses { get; private set; }
        public Dictionary<int, string> Milestones { get; private set; }

        public ClassInfo(string name, string description, Dictionary<string, float> statBonuses, Dictionary<int, string> milestones)
        {
            Name = name;
            Description = description;
            StatBonuses = statBonuses;
            Milestones = milestones;
        }
    }

    public class StatInfo
    {
        public string Name { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public StatInfo(string name, float minValue, float maxValue)
        {
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
} 