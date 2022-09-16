namespace Sankari;

public partial class LevelUIManager : Control
{
    [Export] protected  NodePath NodePathCoinSprite;
    [Export] protected  NodePath NodePathLabelCoins;
    [Export] protected  NodePath NodePathControlLives;
    [Export] protected  NodePath NodePathLabelLives;

    private AnimatedSprite2D coinSprite;
    private Label labelCoins;
    private Label labelLives;
    private Control controlLives;

    private GTween tween;

    private int coins;
    private int lives = 3;

    public override void _Ready()
    {
        labelCoins = GetNode<Label>(NodePathLabelCoins);
        labelLives = GetNode<Label>(NodePathLabelLives);
        coinSprite = GetNode<AnimatedSprite2D>(NodePathCoinSprite);
        controlLives = GetNode<Control>(NodePathControlLives);
        tween = new GTween(controlLives);
        coinSprite.Playing = true;
        controlLives.Hide();
    }

    public void ShowLives() 
    {
        controlLives.Modulate = Colors.White;
        controlLives.Show();
    }
    
    public void HideLives() => controlLives.Hide();

    public async Task HideLivesTransition() 
    {
        tween.InterpolateProperty("modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 2);
        tween.Start();
        await Task.Delay(2000);
    }

    public void RemoveLife()
    {
        lives--;
        labelLives.Text = "" + lives;
    }

    public void AddCoins(int amount = 1)
    {
        coins += amount;
        labelCoins.Text = "" + coins;
    }
}
