namespace Sankari;

public partial class PlatformDisappear : APlatform
{
    [Export] public int DurationFlash1 = 2000;
    [Export] public int DurationFlash2 = 2000;
    [Export] public int DurationReappear = 3000;

    private Sprite2D sprite;
    private ShaderMaterial shaderMaterial;
    private float time;
    private bool playerOnPlatform;
    private bool phase2;
    private GTimer timerFlash1;
    private GTimer timerFlash2;
    private GTimer timerReappear;

    public override void _Ready()
    {
        Init();

        sprite = GetNode<Sprite2D>("Sprite2D");
        shaderMaterial = (sprite.Material as ShaderMaterial);
        timerFlash1 = new GTimer(this, nameof(OnTimerFlash1Up), DurationFlash1, false, false);
        timerFlash2 = new GTimer(this, nameof(OnTimerFlash2Up), DurationFlash2, false, false);
        timerReappear = new GTimer(this, nameof(OnTimerReappear), DurationReappear, false, false);
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        time += delta;

        if (playerOnPlatform) 
        {
            if (!phase2)
                SetWhiteProgress(time.Pulse(2));
            else
                SetWhiteProgress(time.Pulse(4));
        }
            
    }

    private void SetWhiteProgress(float v) => shaderMaterial.SetShaderParameter("white_progress", v);

    private bool AreaIsPlayer(Area2D area) => area.GetParent() is Player;

    private void OnTimerFlash1Up()
    {
        phase2 = true;
        timerFlash2.Start();
    }

    private void OnTimerFlash2Up() 
    {
        Visible = false;
        Collision.Disabled = true;
        timerReappear.Start();
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
            playerOnPlatform = true;
            timerFlash1.Start();
        }
    }

    private void _on_Area2D_area_exited(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            SetWhiteProgress(0);
            playerOnPlatform = false;
            phase2 = false;
            timerFlash1.Stop();
            timerFlash2.Stop();
        }
    }
}
