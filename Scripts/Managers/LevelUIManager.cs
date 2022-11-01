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

	/// <summary>
	/// Adds the specified amount of health
	/// </summary>
	/// /// <param name="amount">Amount of health to add</param>
	public void AddHealth(int amount = 2)
	{
		//Health is literally an index of visible sprites
		for (var spriteIndex = Health; spriteIndex < Health + amount; spriteIndex++)
		{
			if (spriteIndex % 2 != 0)
			{
				HealthBar.GetChild<Sprite2D>(spriteIndex - 1).Hide();

				if (HealthBar.GetChildCount() <= spriteIndex)
					AddHealthSprite(Textures.FullHeart, new Vector2(50 * (spriteIndex - 1)*0.5f, 40));
			}
			else
			{
				if (HealthBar.GetChildCount() <= spriteIndex)
				{
					AddHealthSprite(Textures.HalfHeart, new Vector2(50 * spriteIndex*0.5f, 40));

					HealthBar.GlobalPosition = new Vector2(HealthBar.GlobalPosition.x - 50, HealthBar.GlobalPosition.y);
					HealthBar.Size = new Vector2(50 * (spriteIndex+1), 48);
				}
			}

			HealthBar.GetChild<Sprite2D>(spriteIndex).Show();
		}
		Health += amount;
	}

	private void AddHealthSprite(Texture2D texture, Vector2 position) 
	{
		var sprite = new Sprite2D() 
		{
			Texture = texture,
			Scale = new Vector2((float)48 / texture.GetWidth(), (float)48 / texture.GetHeight()), // 48 looks good as a size (64 was too big, 32 too small)
			Position = position
		};

		HealthBar.AddChild(sprite);
	}

	/// <summary>
	/// If is able to substract some health True otherwise False
	/// </summary>
	/// <param name="amount">Amount of health to substract</param>
	public bool RemoveHealth(int amount = 1)
	{
		if ((Health -= amount) <= 0)
		{
			for (int i = HealthBar.GetChildCount() - 1; i >= 0; i--)
			{
				var curHeart  = HealthBar.GetChild<Sprite2D>(i);

				if (curHeart.Visible)
					curHeart.Hide();
			}

			Health = 0;

			return false;
		}
		else
		{
			var c = amount;

			for (var i = HealthBar.GetChildCount() - 1; i >= 0 && c > 0; i--)
			{
				var curHeart  = HealthBar.GetChild<Sprite2D>(i);
				var prevHeart = HealthBar.GetChild<Sprite2D>(i - 1);

				if (curHeart.Visible)
				{
					if (i % 2 == 0)
						curHeart.Hide();
					else
					{
						curHeart.Hide();
						prevHeart.Show();
					}

					c--;
				}
			}

			return true;
		}
	}
}
