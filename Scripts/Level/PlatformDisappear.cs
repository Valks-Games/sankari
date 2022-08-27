namespace Sankari;

public class PlatformDisappear : APlatform
{
    private Sprite _sprite;

    public override void _Ready()
    {
        Init();

        _sprite = GetNode<Sprite>("Sprite");
        (_sprite.Material as ShaderMaterial).SetShaderParam("white_progress", 1);
    }
}
