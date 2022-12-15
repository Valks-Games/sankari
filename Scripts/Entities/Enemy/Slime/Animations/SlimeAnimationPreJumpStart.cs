namespace Sankari;

public class SlimeAnimationPreJumpStart : EntityAnimation<Slime>
{
	public Slime Slime { get; set; }

	public SlimeAnimationPreJumpStart(Slime slime) : base(slime) => Slime = slime;

	public override void Enter()
	{
		Slime.Jump += Slime_Jump;

		Entity.AnimatedSprite.Play("pre_jump_start");
	}

	public override void Update()
	{
		var shakeStrength = 0.4f;

		Entity.AnimatedSprite.Offset = new Vector2(-shakeStrength + new Random().NextSingle() * shakeStrength * 2, 0);
	}

	public override void Exit()
	{
		Slime.Jump -= Slime_Jump;
	}

	private void Slime_Jump(object sender, EventArgs e)
	{
		SwitchState(EntityAnimationType.JumpStart);
	}
}
