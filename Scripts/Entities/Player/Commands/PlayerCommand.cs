namespace Sankari;

public enum PlayerCommandType
{
    Animation,
    Dash,
    Movement,
    WallJump,
    MidAirJump
}

public abstract class PlayerCommand
{
    /// <summary>
    /// Called after parent entity is ready
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// Called when Movement occurs.
    /// </summary>
    /// <param name="input">Input to act on</param>
    public virtual void Update(float delta) { }

    /// <summary>
    /// Called when moving in the air
    /// </summary>
    /// <param name="delta"></param>
    public virtual void UpdateAir(float delta) { }

    /// <summary>
    /// Called when walking on the ground
    /// </summary>
    /// <param name="delta"></param>
    public virtual void UpdateGroundWalking(float delta) { }

    /// <summary>
    /// Called when running on the ground
    /// </summary>
    /// <param name="delta"></param>
    public virtual void UpdateGroundSprinting(float delta) { }

    /// <summary>
    /// Start the command
    /// </summary>
    public virtual void Start() { }

    /// <summary>
    /// Stop the command
    /// </summary>
    public virtual void Stop() { }
}

public abstract class PlayerCommand<T> : PlayerCommand
{
    protected T Entity { get; set; }

    public PlayerCommand(T entity)
    {
        Entity = entity;
    }
}
