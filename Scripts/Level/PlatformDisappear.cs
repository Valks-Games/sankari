namespace Sankari;

public class PlatformDisappear : APlatform
{
    [Export] public int DurationFlash1 = 2000;
    [Export] public int DurationFlash2 = 2000;
    [Export] public int DurationReappear = 3000;

    private Sprite _sprite;
    private ShaderMaterial _shaderMaterial;
    private float _time;
    private bool _playerOnPlatform;
    private bool _phase2;
    private GTimer _timerFlash1;
    private GTimer _timerFlash2;
    private GTimer _timerReappear;

    public override void _Ready()
    {
        Init();

        _sprite = GetNode<Sprite>("Sprite");
        _shaderMaterial = (_sprite.Material as ShaderMaterial);
        _timerFlash1 = new GTimer(this, nameof(OnTimerFlash1Up), DurationFlash1, false, false);
        _timerFlash2 = new GTimer(this, nameof(OnTimerFlash2Up), DurationFlash2, false, false);
        _timerReappear = new GTimer(this, nameof(OnTimerReappear), DurationReappear, false, false);
    }

    public override void _PhysicsProcess(float delta)
    {
        _time += delta;

        if (_playerOnPlatform) 
        {
            if (!_phase2)
                SetWhiteProgress(_time.Pulse(2));
            else
                SetWhiteProgress(_time.Pulse(4));
        }
            
    }

    private void SetWhiteProgress(float v) => _shaderMaterial.SetShaderParam("white_progress", v);

    private bool AreaIsPlayer(Area2D area) => area.GetParent() is Player;

    private void OnTimerFlash1Up()
    {
        _phase2 = true;
        _timerFlash2.Start();
    }

    private void OnTimerFlash2Up() 
    {
        Visible = false;
        Collision.Disabled = true;
        _timerReappear.Start();
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
            _playerOnPlatform = true;
            _timerFlash1.Start();
        }
    }

    private void _on_Area2D_area_exited(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            SetWhiteProgress(0);
            _playerOnPlatform = false;
            _phase2 = false;
            _timerFlash1.Stop();
            _timerFlash2.Stop();
        }
    }
}
