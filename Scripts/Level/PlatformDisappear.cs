namespace Sankari;

public class PlatformDisappear : APlatform
{
    private Sprite _sprite;
    private ShaderMaterial _shaderMaterial;
    private float _time;

    public override void _Ready()
    {
        Init();

        _sprite = GetNode<Sprite>("Sprite");
        _shaderMaterial = (_sprite.Material as ShaderMaterial);
    }

    public override void _PhysicsProcess(float delta)
    {
        _time += delta;
        _shaderMaterial.SetShaderParam("white_progress", _time.Pulse(2));
    }
}
