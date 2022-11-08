namespace Sankari;

public class SlimeAnimationIdle : EntityAnimationIdle<MovingEntity>
{
	public Slime Slime { get; set; }

	public SlimeAnimationIdle(Slime slime) : base(slime) => Slime = slime;

	public override void HandleTransitions()
	{
		if (Slime.IsNearGround() && !Slime.StartedPreJump && !Slime.IdleTimer.IsActive())
		{
			Slime.StartedPreJump = true;
			Slime.AnimatedSprite.Play("pre_jump_start");
			Slime.PreJumpTimer.Start();
			SwitchState(EntityAnimationType.PreJumpStart);
		}
	}
}
