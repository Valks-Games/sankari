namespace Sankari;

public class SlimeAnimationPreJumpStart : EntityAnimation<Slime>
{
	public SlimeAnimationPreJumpStart(Slime entity) : base(entity) => Entity = entity;

	public override void Enter()
	{
		Entity.Jump += Slime_Jump;

		Entity.AnimatedSprite.Play("pre_jump_start");
	}

	public override void Update()
	{
		var shakeStrength = 0.4f;

		Entity.AnimatedSprite.Offset = new Vector2(-shakeStrength + new Random().NextSingle() * shakeStrength * 2, 0);
	}

	public override void Exit()
	{
		Entity.Jump -= Slime_Jump;
	}

	private void Slime_Jump(object sender, EventArgs e)
	{
		SwitchState(EntityAnimationType.JumpStart);
	}
}
