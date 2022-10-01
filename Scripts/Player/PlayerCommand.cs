using Sankari.Scripts.Player.Movement;

namespace Sankari;

public abstract class PlayerCommand
{
	public virtual void Init(Player player)
	{ }

	public virtual void Update(Player player, MovementInput input)
	{ }

	/// <summary>
	/// Last Change to modify player velocity
	/// </summary>
	/// <param name="player">player being modified</param>
	public virtual void LateUpdate(Player player, MovementInput input)
	{ }
}