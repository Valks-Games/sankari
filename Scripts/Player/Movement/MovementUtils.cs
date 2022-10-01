namespace Sankari.Scripts.Player.Movement;

public class MovementInput
{
	public bool IsJump { get; set; }
	public bool IsUp { get; set; }
	public bool IsDown { get; set; }
	public bool IsFastFall { get; set; }
	public bool IsDash { get; set; }
	public bool IsSprint { get; set; }
}

internal class MovementUtils
{
	public static MovementInput GetMovementInput()
	{
		return new()
		{
			IsJump = Input.IsActionJustPressed("player_jump"),
			IsUp = Input.IsActionPressed("player_move_up"),
			IsDown = Input.IsActionPressed("player_move_down"),
			IsFastFall = Input.IsActionPressed("player_fast_fall"),
			IsDash = Input.IsActionJustPressed("player_dash"),
			IsSprint = Input.IsActionPressed("player_sprint"),
		};
	}
}