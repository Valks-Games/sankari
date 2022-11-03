namespace Sankari;

public interface IEntityMovement : IEntityMoveable, IEntityDash
{
	// Ground acceleration
	public int AccelerationGround { get; set; }

}
