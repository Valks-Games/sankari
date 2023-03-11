namespace Sankari;

public partial class LevelUIManager : Control
{
    [Export] protected NodePath NodePathCoinSprite { get; set; }
    [Export] protected NodePath NodePathLabelCoins { get; set; }
    [Export] protected NodePath NodePathControlLives { get; set; }
    [Export] protected NodePath NodePathLabelLives { get; set; }
    [Export] protected NodePath NodePathHealthBar { get; set; }
    [Export] protected NodePath NodePathGameOver  { get; set; }

    private AnimatedSprite2D CoinSprite { get; set; }
    private Label LabelCoins { get; set; }
    private Label LabelLives { get; set; }
    private Control ControlLives { get; set; }
    public Control HealthBar { get; set; }
    private Label LabelGameOver { get; set; }

    private GTween Tween { get; set; }
    public int Health { get; set; }

    public override void _Ready()
    {
        LabelCoins = GetNode<Label>(NodePathLabelCoins);
        LabelLives = GetNode<Label>(NodePathLabelLives);
        CoinSprite = GetNode<AnimatedSprite2D>(NodePathCoinSprite);
        ControlLives = GetNode<Control>(NodePathControlLives);
        HealthBar = GetNode<HBoxContainer>(NodePathHealthBar);
        LabelGameOver = GetNode<Label>(NodePathGameOver);

        CoinSprite.Play();
        LabelGameOver.Hide();
        ControlLives.Hide();
    }

    public void ShowLives() 
    {
        ControlLives.Modulate = Colors.White;
        ControlLives.Show();
    }

    public void HideLives() => ControlLives.Hide();

    public void SetLabelLives(int amount) => LabelLives.Text = "" + amount;

    public async Task HideLivesTransition() 
    {
        Tween = new GTween(ControlLives);
        Tween.InterpolateProperty("modulate", new Color(1, 1, 1, 0), 2);
        Tween.Start();
        await Task.Delay(2000);
    }

    public void ShowGameOver()
    {
        LabelGameOver.Modulate = Colors.DarkRed;
        LabelGameOver.Show();
    }

    public void HideGameOver() => LabelGameOver.Hide();
    
    public void ShowCoins()
    {
        CoinSprite.Show();
        LabelCoins.Show();
        CoinSprite.Play();
    }
    public void HideCoins()
    {
        CoinSprite.Hide();
        LabelCoins.Hide();
    }
    public void SetLabelCoins(int amount) => LabelCoins.Text = "" + amount;
}
