using System;

namespace Sankari;

public partial class TestPlayer : CharacterBody2D
{
	private GTimer TimerJumpForce { get; set; }

	public override void _Ready()
	{
		// Only allow holding down the jump key for a certain time
		TimerJumpForce = new(this, 1000, false);

		// Godot sets this to true by default which always confuses me
		// because I'm thinking I'm dealing with a non-looping timer
		// This should be set to false by default in GTimer
		TimerJumpForce.Loop = false; 
	}

	public override void _PhysicsProcess(double delta)
	{
		// Gravity
		Velocity += new Vector2(0, 20);

		// Just pressed jump
		if (Input.IsActionJustPressed("player_jump"))
		{
			TimerJumpForce.Start();
		}

		// Holding down jump key
		if (Input.IsActionPressed("player_jump"))
		{
			if (TimerJumpForce.IsActive())
			{
				// only apply force while allowed to do so
				Velocity -= new Vector2(0, 30);
			}
		}

		MoveAndSlide();
	}
}
