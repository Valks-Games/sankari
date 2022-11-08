namespace Sankari;

public class EntityAnimationIdle : EntityAnimation<Slime>
{
	public EntityAnimationIdle(Slime entity) : base(entity) { }

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("idle");
	}

	public override void ExitState()
	{
		
	}

	public override void HandleStateTransitions()
	{
		// Idle -> JumpStart
		// Idle -> JumpFall

		if (!Entity.IsNearGround() && Entity.IsFalling())
			SwitchState(EntityAnimationType.JumpFall);
		else if (!Entity.IsNearGround() && Entity.Velocity.y != 0)
			SwitchState(EntityAnimationType.JumpStart);
	}

	public override void UpdateState()
	{
		
	}
}
