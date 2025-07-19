using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Wolfgodrpg.Common.Skills;
using ReLogic.Content;

namespace Wolfgodrpg.Common.UI
{
    public class UIProgressBar : UIPanel
    {
        private float progress = 0f;

        public UIProgressBar()
        {
            BackgroundColor = new Color(63, 63, 70) * 0.7f;
        }

        public void SetProgress(float progress)
        {
            this.progress = MathHelper.Clamp(progress, 0f, 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var dims = GetInnerDimensions();
            var barWidth = dims.Width * progress;

            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle((int)dims.X, (int)dims.Y, (int)barWidth, (int)dims.Height), new Color(70, 160, 240));
        }
    }

    public class SkillItemUI : UIPanel
    {
        private BaseSkill skill;
        private UIText nameText;
        private UIText cooldownText;
        private UIText costText;

        public SkillItemUI(BaseSkill skill)
        {
            this.skill = skill;

            Width.Set(0, 1);
            Height.Set(50, 0);
            BackgroundColor = new Color(33, 33, 33) * 0.85f;
            PaddingLeft = 10;
            PaddingRight = 10;
            MarginBottom = 6;

            // In a real mod, you would load the icon texture here.
            // var icon = new UIImage(ModContent.Request<Texture2D>($"Wolfgodrpg/Assets/UI/Icons/{skill.Name}").Value);
            // Append(icon);

            nameText = new UIText(skill.Name, 1f, false);
            nameText.Left.Set(10, 0); // Changed from 44 to 10 since there's no icon
            nameText.Top.Set(6, 0);
            Append(nameText);

            cooldownText = new UIText($"CD: {skill.GetCooldownTimeRemaining():F1}s", 0.8f);
            cooldownText.Left.Set(-120, 1);
            cooldownText.Top.Set(8, 0);
            Append(cooldownText);

            costText = new UIText($"Cost: {skill.StaminaCost:F0}", 0.8f);
            costText.Left.Set(-60, 1);
            costText.Top.Set(8, 0);
            Append(costText);

            OnMouseOver += (evt, elem) =>
            {
                Main.hoverItemName = skill.Name;
                Main.instance.MouseText(skill.GetDisplayDescription());
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cooldownText.SetText($"CD: {skill.GetCooldownTimeRemaining():F1}s");
            costText.SetText($"Cost: {skill.StaminaCost:F0}");
            
            if (skill.IsUnlocked)
            {
                BackgroundColor = skill.CooldownTimer > 0 ? new Color(50, 40, 40) * 0.85f : new Color(33, 33, 33) * 0.85f;
                nameText.TextColor = Color.White;
                cooldownText.TextColor = skill.CooldownTimer > 0 ? Color.Red : Color.LightGreen;
            }
            else
            {
                BackgroundColor = new Color(20, 20, 20) * 0.85f;
                nameText.TextColor = Color.Gray;
                cooldownText.TextColor = Color.DarkGray;
            }
        }
    }
}
