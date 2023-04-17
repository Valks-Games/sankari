namespace Sankari;

public abstract partial class APlatform : CharacterBody2D
{
    protected CollisionShape2D Collision { get; private set; }
    
    private GTimer Timer { get; set; }

    public void Init()
    {
        Timer = new GTimer(this, OnTimerUp, 400);

        Collision = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public void TemporarilyDisablePlatform() 
    {
        Timer.StartMs();
        Collision.Disabled = true;
    }

    private void OnTimerUp()
    {
        Collision.Disabled = false;
    }
}
