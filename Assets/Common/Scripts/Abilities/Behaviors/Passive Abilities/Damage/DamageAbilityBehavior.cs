namespace OctoberStudio.Abilities
{
    public class DamageAbilityBehavior : AbilityBehavior<DamageAbilityData, DamageAbilityLevel>
    {
        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);

            PlayerBehavior.Player.RecalculateDamage(AbilityLevel.DamageMultiplier);
        }
    }
}