namespace Sankari;

public class PlatformDisappear : APlatform
{
    [Export] public int Duration1 = 2000;
    [Export] public int Duration2 = 2000;

    private Sprite _sprite;
    private ShaderMaterial _shaderMaterial;
    private float _time;
    private bool _playerOnPlatform;
    private bool _phase2;
    private GTimer _timer1;
    private GTimer _timer2;

    public override void _Ready()
    {
        Init();

        _sprite = GetNode<Sprite>("Sprite");
        _shaderMaterial = (_sprite.Material as ShaderMaterial);
        _timer1 = new GTimer(this, nameof(OnTimer1Up), Duration1, false, false);
        _timer2 = new GTimer(this, nameof(OnTimer2Up), Duration2, false, false);
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

    private void OnTimer1Up()
    {
        _phase2 = true;
        _timer2.Start();
    }

    private void OnTimer2Up() => QueueFree();

    private void _on_Area2D_area_entered(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            _playerOnPlatform = true;
            _timer1.Start();
        }
    }

    private void _on_Area2D_area_exited(Area2D area)
    {
        if (AreaIsPlayer(area))
        {
            SetWhiteProgress(0);
            _playerOnPlatform = false;
            _phase2 = false;
            _timer1.Stop();
            _timer2.Stop();
        }
    }
}
