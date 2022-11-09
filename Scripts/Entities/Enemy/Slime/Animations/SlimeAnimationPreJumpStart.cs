namespace Sankari;

public class SlimeAnimationPreJumpStart : EntityAnimation<Slime>
{
	public SlimeAnimationPreJumpStart(Slime slime) : base(slime) { }

	public override void Enter()
	{
		Entity.SlimeJump += Slime_SlimeJump;

		Entity.AnimatedSprite.Play("pre_jump_start");
	}

	public override void Update()
	{
		var shakeStrength = 0.4f;

		Entity.AnimatedSprite.Offset = new Vector2(-shakeStrength + new Random().NextSingle() * shakeStrength * 2, 0);
	}

	public override void Exit()
	{
		Entity.SlimeJump -= Slime_SlimeJump;
	}

	private void Slime_SlimeJump(object sender, EventArgs e)
	{
		SwitchState(EntityAnimationType.JumpStart);
	}
}
