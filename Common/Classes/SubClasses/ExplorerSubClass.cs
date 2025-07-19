using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Skills;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.Classes.SubClasses
{
    public class ExplorerSubClass : PlayerSubClass
    {
        public ExplorerSubClass()
        {
            Name = "Explorer";
            Description = "Master of exploration and discovery.";
            Icon = "🗺️";
        }

        protected override void InitializeSkills()
        {
            AddSkill(new Skills.Exploration.TreasureHuntSkill());
            AddSkill(new Skills.Exploration.PathfinderSkill());
            AddSkill(new Skills.Exploration.DiscoverySkill());
        }

        public override Dictionary<string, float> GetStatModifiers()
        {
            return new Dictionary<string, float>
            {
                { "MoveSpeed", 1f + (Level * 0.02f) },
                { "MiningSpeed", 1f + (Level * 0.03f) },
                { "Luck", Level * 0.1f }
            };
        }

        public override Color GetClassColor() => new Color(255, 193, 7);
    }
} 