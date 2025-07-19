using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class SummonerSubClass : PlayerSubClass
    {
        public SummonerSubClass()
        {
            Name = "Summoner";
            Description = "Master of summoning and minion control.";
            Icon = "👻";
        }

        protected override void InitializeSkills()
        {
            AddSkill(new Skills.Summoning.SummonMinionSkill());
            AddSkill(new Skills.Summoning.MinionBoostSkill());
            AddSkill(new Skills.Summoning.SoulBondSkill());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "SummonDamage", 1f + (Level * 0.05f) },
                { "MaxMinions", Level * 0.5f },
                { "MinionKnockback", Level * 0.3f }
            };
        }

        public override Color GetClassColor() => new Color(255, 123, 0);
    }
} 