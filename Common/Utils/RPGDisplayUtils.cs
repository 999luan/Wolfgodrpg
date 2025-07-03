namespace Wolfgodrpg.Common.Utils
{
    public static class RPGDisplayUtils
    {
        public static string GetStatDisplayName(string statKey)
        {
            return statKey switch
            {
                "meleeDamage" => "Dano C.C.",
                "rangedDamage" => "Dano Dist.",
                "magicDamage" => "Dano Mágico",
                "minionDamage" => "Dano Servos",
                "critChance" => "Crítico",
                "meleeCrit" => "Crit C.C.",
                "rangedCrit" => "Crit Dist.",
                "magicCrit" => "Crit Mágico",
                "meleeSpeed" => "Vel. C.C.",
                "defense" => "Defesa",
                "maxLife" => "Vida Máx.",
                "lifeRegen" => "Regen Vida",
                "moveSpeed" => "Velocidade",
                "maxMana" => "Mana Máx.",
                "manaRegen" => "Regen Mana",
                "luck" => "Sorte",
                "miningSpeed" => "Mineração",
                _ => statKey
            };
        }
    }
}
