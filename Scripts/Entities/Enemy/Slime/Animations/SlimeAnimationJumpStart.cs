namespace Sankari;

public class SlimeAnimationJumpStart : EntityAnimationJumpStart<Slime>
{
	public Slime Slime { get; set; }

	private bool ChangingDirections { get; set; }

	public SlimeAnimationJumpStart(Slime slime) : base(slime) => Slime = slime;

	public override void Enter()
	{
		base.Enter();
		ChangingDirections = false;
	}

	public override void Update()
	{
		if (Entity.IsOnWall())
        {
            Slime.WallHugTime++;

            if (Slime.WallHugTime >= 10 && !ChangingDirections)
			{
				ChangingDirections = true;
                Slime.MovingForward = !Slime.MovingForward;
			}
        }
	}
}
