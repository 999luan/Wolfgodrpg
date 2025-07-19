using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class AcrobatSubClass : PlayerSubClass
    {
        public AcrobatSubClass()
        {
            Name = "Acrobat";
            Description = "Master of movement and evasion.";
            Icon = "🤸";
        }

        protected override void InitializeSkills()
        {
            // These will be implemented as separate skill classes
            AddSkill(new Skills.Movement.MovementDashSkill());
            AddSkill(new Skills.Movement.DoubleJumpSkill());
            AddSkill(new Skills.Movement.WallJumpSkill());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "moveSpeed", 1f + (Level * 0.01f) },
                { "jumpHeight", 1f + (Level * 0.01f) }
            };
        }

        public override Color GetClassColor() => new Color(50, 220, 50);
    }
}
