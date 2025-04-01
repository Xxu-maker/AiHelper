namespace OctoberStudio.Abilities
{
    public class GoldAbilityBehavior : AbilityBehavior<GoldAbilityData, GoldAbilityLevel>
    {
        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);

            PlayerBehavior.Player.RecalculateGoldMultiplier(AbilityLevel.GoldMultiplier);
        }
    }
}