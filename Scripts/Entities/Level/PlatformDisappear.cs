namespace Sankari;

public partial class PlatformDisappear : APlatform
{
    [Export] public int DurationFlash1 { get; set; } = 2000;
    [Export] public int DurationFlash2 { get; set; } = 2000;
    [Export] public int DurationReappear { get; set; } = 3000;

    private Sprite2D Sprite { get; set; }
    private ShaderMaterial ShaderMaterial { get; set; }
    private float Time { get; set; }
    private bool PlayerOnPlatform { get; set; }
    private bool Phase2 { get; set; }
    private GTimer TimerFlash1 { get; set; }
    private GTimer TimerFlash2 { get; set; }
    private GTimer TimerReappear { get; set; }

    public override void _Ready()
    {
        Init();

        Sprite = GetNode<Sprite2D>("Sprite2D");
        ShaderMaterial = (Sprite.Material as ShaderMaterial);

        TimerFlash1 = new GTimer(this, OnTimerFlash1Up, DurationFlash1);
        TimerFlash2 = new GTimer(this, OnTimerFlash2Up, DurationFlash2);
        TimerReappear = new GTimer(this, OnTimerReappear, DurationReappear);
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        Time += delta;

        if (PlayerOnPlatform) 
        {
            if (!Phase2)
                SetWhiteProgress(Time.Pulse(2));
            else
                SetWhiteProgress(Time.Pulse(4));
        }
            
    }

    private void SetWhiteProgress(float v) => ShaderMaterial.SetShaderParameter("white_progress", v);

    private bool AreaIsPlayer(Area2D area) => area.GetParent() is Player;

    private void OnTimerFlash1Up()
    {
        Phase2 = true;
        TimerFlash2.StartMs();
    }

    private void OnTimerFlash2Up() 
    {
        Visible = false;
        Collision.Disabled = true;
        TimerReappear.StartMs();
    }

    private void OnTimerReappear()
    {
        Visible = true;
        Collision.Disabled = false;
        SetWhiteProgress(0);
    }

    private void _on_Area2D_area_entered(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            PlayerOnPlatform = true;
            TimerFlash1.StartMs();
        }
    }

    private void _on_Area2D_area_exited(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            SetWhiteProgress(0);
            PlayerOnPlatform = false;
            Phase2 = false;
            TimerFlash1.Stop();
            TimerFlash2.Stop();
        }
    }
}
