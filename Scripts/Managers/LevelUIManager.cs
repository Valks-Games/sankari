namespace Sankari;

public partial class LevelUIManager : Control
{
    [Export] protected NodePath NodePathCoinSprite { get; set; }
    [Export] protected NodePath NodePathLabelCoins { get; set; }
    [Export] protected NodePath NodePathControlLives { get; set; }
    [Export] protected NodePath NodePathLabelLives { get; set; }
	[Export] protected NodePath NodePathHealthBar { get; set; }


	private AnimatedSprite2D CoinSprite { get; set; }
    private Label LabelCoins { get; set; }
    private Label LabelLives { get; set; }
    private Control ControlLives { get; set; }
	private Control HealthBar { get; set; }

    private GTween Tween { get; set; }

    private int Coins { get; set; }
    private int Lives { get; set; } = 3;
	private float Health { get; set; } = 0;

    public override void _Ready()
    {
        LabelCoins = GetNode<Label>(NodePathLabelCoins);
        LabelLives = GetNode<Label>(NodePathLabelLives);
        CoinSprite = GetNode<AnimatedSprite2D>(NodePathCoinSprite);
        ControlLives = GetNode<Control>(NodePathControlLives);
		HealthBar = GetNode<HBoxContainer>(NodePathHealthBar);

		AddHealth(3);
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
		Tween = new GTween(ControlLives);
        Tween.InterpolateProperty("modulate", new Color(1, 1, 1, 0), 2);
        Tween.Start();
        await Task.Delay(2000);
    }

    public void RemoveLife()
    {
		RemoveHealth(Health);
		AddHealth(3);
		Lives--;
        LabelLives.Text = "" + Lives;
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
        LabelCoins.Text = "" + Coins;
    }

	/// <summary>
	/// Adds the specified amount of health
	/// </summary>
	/// /// <param name="amount">Amount of health to add</param>
	public void AddHealth(float amount = 1)
	{
		var textureFullHeart = GD.Load<Texture2D>("res://Sprites/icon.png");
		var textureHalfHeart = GD.Load<Texture2D>("res://Sprites/light.png");

		var j = Health*2; //Health is literally a halved index of visible sprites
		for (var i = Health + 0.5f; i <= Health + amount; i += 0.5f, j++) //adding 'Health' is required due to the nature of the usage of 'i'
		{
			if ((2 * i) % 2 == 0) // if 'i' is an int
			{
				HealthBar.GetChild<Sprite2D>((int)j - 1).Hide();

				if (HealthBar.GetChildCount() <= j)
					AddHealthSprite(textureFullHeart, new Vector2(50 * (i - 1), 40));
			}
			else
			{
				if (HealthBar.GetChildCount() <= j)
				{
					AddHealthSprite(textureHalfHeart, new Vector2(50 * (int)i, 40));

					HealthBar.GlobalPosition = new Vector2(HealthBar.GlobalPosition.x - 50, HealthBar.GlobalPosition.y);
					HealthBar.Size = new Vector2(50 * ((int)i + 1), 48);
				}
			}

			HealthBar.GetChild<Sprite2D>((int)j).Show();
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
	public bool RemoveHealth(float amount = 0.5f)
	{
		if ((Health -= amount) <= 0)
		{
			for (var i = HealthBar.GetChildCount() - 1; i >= 0; i--)
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
			var c = 2 * amount;

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
