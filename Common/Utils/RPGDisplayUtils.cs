namespace Wolfgodrpg.Common.Utils
{
    public static class RPGDisplayUtils
    {
        public static string GetStatDisplayName(string statKey)
        {
            return statKey switch
            {
                "meleeDamage" => "Melee Damage",
                "rangedDamage" => "Ranged Damage",
                "magicDamage" => "Magic Damage",
                "summonDamage" => "Summon Damage",
                "critChance" => "Critical",
                "meleeCrit" => "Melee Crit",
                "rangedCrit" => "Ranged Crit",
                "magicCrit" => "Magic Crit",
                "summonCrit" => "Summon Crit",
                "maxLife" => "Max Life",
                "lifeRegen" => "Life Regen",
                "maxMana" => "Max Mana",
                "manaRegen" => "Mana Regen",
                "defense" => "Defense",
                "miningSpeed" => "Mining Speed",
                "pickSpeed" => "Pick Speed",
                "axe" => "Axe Power",
                "hammer" => "Hammer Power",
                "moveSpeed" => "Move Speed",
                "jumpSpeed" => "Jump Speed",
                "fallResist" => "Fall Resist",
                "knockbackResist" => "Knockback Resist",
                "luck" => "Luck",
                _ => statKey
            };
        }
    }
}
