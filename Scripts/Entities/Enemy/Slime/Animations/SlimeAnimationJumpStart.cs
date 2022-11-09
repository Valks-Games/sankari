namespace Sankari;

public class SlimeAnimationJumpStart : EntityAnimationJumpStart<Slime>
{
	private bool ChangingDirections { get; set; }

	public SlimeAnimationJumpStart(Slime slime) : base(slime) { }

	public override void Enter()
	{
		base.Enter();
		ChangingDirections = false;
	}

	public override void Update()
	{
		if (Entity.IsOnWall())
        {
            Entity.WallHugTime++;

            if (Entity.WallHugTime >= 10 && !ChangingDirections)
			{
				ChangingDirections = true;
                Entity.MovingForward = !Entity.MovingForward;
			}
        }
	}
}
