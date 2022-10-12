namespace Sankari;

public interface IEntityJumpable : IEntityMovement
{
	public int JumpCount { get; set; }
}

public interface IEntityGroundJumpable : IEntityJumpable 
{
	public int JumpForce { get; set; }	
}

public class EntityCommandGroundJump : EntityCommand<IEntityGroundJumpable>
{
	public EntityCommandGroundJump(IEntityGroundJumpable entity) : base(entity) { }

	public override void Start()
	{
		GameManager.EventsPlayer.Notify(EventPlayer.OnJump);

		Entity.JumpCount++;
		//Velocity = new Vector2(Velocity.x, 0); // reset velocity before jump (is this really needed?)
		Entity.Velocity = Entity.Velocity - new Vector2(0, Entity.JumpForce);
	}
}
