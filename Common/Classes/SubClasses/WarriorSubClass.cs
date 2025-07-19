using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class WarriorSubClass : PlayerSubClass
    {
        public WarriorSubClass()
        {
            Name = "Warrior";
            Description = "A master of close-quarters combat.";
            Icon = "⚔️";
        }

        protected override void InitializeSkills()
        {
            AddSkill(new BasicSlash());
            AddSkill(new PowerStrike());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "MeleeDamage", 1f + (Level * 0.05f) },
                { "MeleeSpeed", 1f + (Level * 0.01f) }
            };
        }

        public override Color GetClassColor() => new Color(220, 50, 50);
    }

    // Exemplo de Skills para a classe Warrior
    public class BasicSlash : BaseSkill
    {
        public BasicSlash()
        {
            Name = "Basic Slash";
            Description = "A quick slash with your weapon.";
            Cooldown = 30;
            StaminaCost = 5;
            Level = 1; // Desbloqueada desde o início
        }

        protected override bool OnActivate(Player player)
        {
            // Lógica da skill aqui
            player.itemAnimation = 10;
            return true;
        }
    }

    public class PowerStrike : BaseSkill
    {
        public PowerStrike()
        {
            Name = "Power Strike";
            Description = "A powerful strike that deals extra damage.";
            Cooldown = 180;
            StaminaCost = 20;
            Level = 1; // Desbloqueada no nível 5
        }

        protected override bool OnActivate(Player player)
        {
            // Lógica da skill aqui
            player.GetModPlayer<RPGPlayer>().Player.AddBuff(Terraria.ID.BuffID.Wrath, 120);
            return true;
        }
    }
}