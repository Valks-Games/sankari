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
	private Control HealthBar { get; set; }
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

        CoinSprite.Playing = true;
		LabelGameOver.Hide();
        ControlLives.Hide();
    }


	public void ShowLevelUI()
	{
		ShowCoins();
		ShowHealth();
	}
	public void HideLevelUI()
	{
		HideCoins();
		HideHealth();
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
		CoinSprite.Playing = true;
	}
	public void HideCoins()
	{
		CoinSprite.Hide();
		LabelCoins.Hide();
	}
    public void SetLabelCoins(int amount) => LabelCoins.Text = "" + amount;


	public void ShowHealth() => HealthBar.Show();
	public void HideHealth() => HealthBar.Hide();
	public void SetHealthBar(int amount)
	{
		var count = HealthBar.GetChildCount();
		Texture2D[] texture = {Textures.FullHeart, Textures.HalfHeart};

		for (int i = 0; i < count; i++)
			HealthBar.GetChild<Sprite2D>(i).Hide();
		while ((count++) < amount)
		{
			var sprite = new Sprite2D()
			{
				Texture = texture[count%2],
				Scale = new Vector2((float)48 / texture[count%2].GetWidth(), (float)48 / texture[count%2].GetHeight()), // 48 looks good as a size (64 was too big, 32 too small)
			};
			HealthBar.AddChild(sprite);
		}
		for (count = 0; count < amount; count++)
		{
			HealthBar.GetChild<Sprite2D>(count).Position = new Vector2(50 * (int)(count/2), 40);
			if (count % 2 == 1)
				HealthBar.GetChild<Sprite2D>(count - 1).Hide();
			HealthBar.GetChild<Sprite2D>(count).Show();
		}
	}
}
