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
	private HBoxContainer HealthBar { get; set; }

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
		AddHealth((float)3);
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
        if((Lives--) > 0)
			AddHealth((float)3);
        LabelLives.Text = "" + Lives;
    }

    public void AddCoins(int amount = 1)
    {
        Coins += amount;
        LabelCoins.Text = "" + Coins;
    }

	/// <summary>
	/// Adds teh specified amount of health
	/// </summary>
	/// /// <param name="amount">Amount of health to add</param>
	public void AddHealth(float amount = 1)
	{
		Health += amount;
		Texture2D textureFullHeart = GD.Load<Texture2D>("res://Sprites/icon.png");
		Texture2D textureHalfHeart = GD.Load<Texture2D>("res://Sprites/light.png");
		int j=0;
		for (float i = (float)0.5; i <= amount; i += (float)0.5, j++)
		{
			if ((2*i)%2 == 0) //i is an int
			{
				HealthBar.GetChild<Sprite2D>(j-1).Hide();
				if (HealthBar.GetChildCount() <= j)
				{
					Sprite2D healthSprite = new Sprite2D();
					healthSprite = new Sprite2D();
					healthSprite.Texture = textureFullHeart;
					healthSprite.Scale = new Vector2((float)48 / healthSprite.Texture.GetWidth(), (float)48 / healthSprite.Texture.GetHeight());
					healthSprite.Position = new Vector2(50 * (i - 1), 40);
					HealthBar.AddChild(healthSprite);
				}
				HealthBar.GetChild<Sprite2D>(j).Show();
			}
			else
			{
				if(HealthBar.GetChildCount() <= j)
				{
					Sprite2D healthSprite = new Sprite2D();
					healthSprite = new Sprite2D();
					healthSprite.Texture = textureHalfHeart;
					healthSprite.Scale = new Vector2((float)48 / healthSprite.Texture.GetWidth(), (float)48 / healthSprite.Texture.GetHeight());
					HealthBar.GlobalPosition = new Vector2(HealthBar.GlobalPosition.x - 50, HealthBar.GlobalPosition.y);
					HealthBar.Size = new Vector2(50 * ((int)i + 1), 48);
					healthSprite.Position = new Vector2(50 * (int)i, 40);
					HealthBar.AddChild(healthSprite);
				}
				HealthBar.GetChild<Sprite2D>(j).Show();
			}
		}
	}

	/// <summary>
	/// If is able to substract some health True otherwise False
	/// </summary>
	/// <param name="amount">Amount of health to substract</param>
	public bool RemoveHealth(float amount = (float)0.5)
	{
		if ((Health -= amount) <= 0)
		{
			for (int i = HealthBar.GetChildCount() - 1; i >= 0; i--)
				if (HealthBar.GetChild<Sprite2D>(i).Visible)
					HealthBar.GetChild<Sprite2D>(i).Hide();
			Health = 0;
			return false;
		}
		else
		{
			float c = 2*amount;
			for(int i=HealthBar.GetChildCount()-1; i>=0 && c>0; i--)
			{
				if(HealthBar.GetChild<Sprite2D>(i).Visible)
				{
					if (i % 2 == 0)
						HealthBar.GetChild<Sprite2D>(i).Hide();
					else
					{
						HealthBar.GetChild<Sprite2D>(i).Hide();
						HealthBar.GetChild<Sprite2D>(i - 1).Show();
					}
					c--;
				}
			}
			return true;
		}
	}
}
