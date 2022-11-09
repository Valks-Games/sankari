namespace Sankari;

public class SlimeAnimationIdle : EntityAnimationIdle<Slime>
{
	public SlimeAnimationIdle(Slime slime) : base(slime) { }

	public override void HandleTransitions()
	{
		if (Entity.IsNearGround() && !Entity.StartedPreJump && !Entity.IdleTimer.IsActive())
		{
			Entity.StartedPreJump = true;
			Entity.AnimatedSprite.Play("pre_jump_start");
			Entity.PreJumpTimer.Start();
			SwitchState(EntityAnimationType.PreJumpStart);
		}
	}
}
