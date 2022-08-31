namespace Sankari;

public class LevelUIManager : Control
{
    [Export] protected readonly NodePath NodePathCoinSprite;
    [Export] protected readonly NodePath NodePathLabelCoins;
    [Export] protected readonly NodePath NodePathControlLives;
    [Export] protected readonly NodePath NodePathLabelLives;

    private AnimatedSprite coinSprite;
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
        coinSprite = GetNode<AnimatedSprite>(NodePathCoinSprite);
        controlLives = GetNode<Control>(NodePathControlLives);
        tween = new GTween(controlLives);
        coinSprite.Playing = true;
        controlLives.Hide();
        Hide();
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