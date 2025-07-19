using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class ArcherSubClass : PlayerSubClass
    {
        public ArcherSubClass()
        {
            Name = "Archer";
            Description = "Master of ranged combat and precision.";
            Icon = "🏹";
        }

        protected override void InitializeSkills()
        {
            AddSkill(new Skills.Combat.PreciseShotSkill());
            AddSkill(new Skills.Combat.RapidFireSkill());
            AddSkill(new Skills.Combat.PowerShotSkill());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "RangedDamage", 1f + (Level * 0.05f) },
                { "RangedSpeed", 1f + (Level * 0.02f) },
                { "RangedCrit", Level * 0.5f }
            };
        }

        public override Color GetClassColor() => new Color(40, 167, 69);
    }
} 