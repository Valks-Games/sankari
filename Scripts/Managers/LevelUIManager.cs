namespace Sankari;

public partial class LevelUIManager : Control
{
    [Export] protected NodePath NodePathCoinSprite { get; set; }
    [Export] protected NodePath NodePathLabelCoins { get; set; }
    [Export] protected NodePath NodePathControlLives { get; set; }
    [Export] protected NodePath NodePathLabelLives { get; set; }

    private AnimatedSprite2D CoinSprite { get; set; }
    private Label LabelCoins { get; set; }
    private Label LabelLives { get; set; }
    private Control ControlLives { get; set; }

    private GTween Tween { get; set; }

    private int Coins { get; set; }
    private int Lives { get; set; } = 3;

    public override void _Ready()
    {
        LabelCoins = GetNode<Label>(NodePathLabelCoins);
        LabelLives = GetNode<Label>(NodePathLabelLives);
        CoinSprite = GetNode<AnimatedSprite2D>(NodePathCoinSprite);
        ControlLives = GetNode<Control>(NodePathControlLives);
        Tween = new GTween(ControlLives);
        CoinSprite.Playing = true;
        ControlLives.Hide();
    }

    public void ShowLives() 
    {
        ControlLives.Modulate = Colors.White;
        ControlLives.Show();
    }
    
    public void HideLives() => ControlLives.Hide();

    public async Task HideLivesTransition() 
    {
        Tween.InterpolateProperty("modulate", new Color(1, 1, 1, 0), 2);
        Tween.Start();
        await Task.Delay(2000);
    }

    public void RemoveLife()
    {
        Lives--;
        LabelLives.Text = "" + Lives;
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
        LabelCoins.Text = "" + Coins;
    }
}
