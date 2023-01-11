using System;

namespace Sankari;

public partial class TestPlayer : CharacterBody2D
{
	// settings
	private int GravityForce { get; set; } = 20;
	private int JumpForce { get; set; } = 70;
	private float JumpForceLoss { get; set; } = 2.5f;
	private bool HoldingJumpKey { get; set; }

	// not to be edited
	private float JumpForceLossCounter { get; set; }

	public override void _PhysicsProcess(double delta)
	{
		// Gravity
		Velocity += new Vector2(0, GravityForce);

		// Just pressed jump
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor())
		{
			HoldingJumpKey = true;
			JumpForceLossCounter = 0;
		}

		// Holding down jump key
		if (Input.IsActionPressed("player_jump") && HoldingJumpKey)
		{
			JumpForceLossCounter += JumpForceLoss;
			Velocity -= new Vector2(0, Mathf.Max(0, JumpForce - JumpForceLossCounter));
		}

		if (Input.IsActionJustReleased("player_jump"))
		{
			HoldingJumpKey = false;
		}

		MoveAndSlide();
	}
}
