using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class MageSubClass : PlayerSubClass
    {
        public MageSubClass()
        {
            Name = "Mage";
            Description = "Master of arcane magic and elemental power.";
            Icon = "🔮";
        }

        protected override void InitializeSkills()
        {
            AddSkill(new Skills.Magic.FireballSkill());
            AddSkill(new Skills.Magic.IceShieldSkill());
            AddSkill(new Skills.Magic.LightningBoltSkill());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "MagicDamage", 1f + (Level * 0.05f) },
                { "ManaRegen", 1f + (Level * 0.02f) },
                { "MaxMana", Level * 5f }
            };
        }

        public override Color GetClassColor() => new Color(111, 66, 193);
    }
} 