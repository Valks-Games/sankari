namespace Sankari;

public class PlayerAnimationJumpFall : EntityAnimationJumpFall<Player>
{
	public PlayerAnimationJumpFall(Player player) : base(player) => Entity = player;

	public override void Update()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleTransitions()
	{
		// JumpFall -> Idle
		// JumpFall -> Walking
		// JumpFall -> Running
		// JumpFall -> Dash
		// JumpFall -> JumpStart

		base.HandleTransitions();
			
		if (Entity.PlayerInput.IsDash && Entity.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady)
			SwitchState(EntityAnimationType.Dash);
	}

	public override void HandleTransitionsNearGround()
	{
		if (Entity.MoveDir.x != 0)
			if (Entity.PlayerInput.IsSprint)
				SwitchState(EntityAnimationType.Running);
			else
				SwitchState(EntityAnimationType.Walking);
		else
			SwitchState(EntityAnimationType.Idle);
	}
}
