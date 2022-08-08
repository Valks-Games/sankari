namespace MarioLikeGame;

public class PlayerStateManager : KinematicBody2D
{
    public Label Label { get; private set; }

    public GameManager GameManager { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public Vector2 Velocity;
    public float Speed;

    public bool InputLeft { get; private set; }
    public bool InputRight { get; private set; }
    public bool InputJump { get; private set; }

    public PlayerIdleState PlayerIdleState = new();
    public PlayerMovingState PlayerMovingState = new();
    public PlayerJumpingState PlayerJumpingState = new();
    public PlayerFallingState PlayerFallingState = new();

    private PlayerBaseState _currentState;
    private const float _gravity = 200;
    private Vector2 _levelStartPos;
    private bool _dead;

    public void PreInit(GameManager gameManager)
    {
        GameManager = gameManager;
        LevelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        Label = GetNode<Label>("Label");
        _levelStartPos = Position;
        _currentState = new PlayerIdleState();
        _currentState.EnterState(this);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_dead)
            return;

        InputLeft = Input.IsActionPressed("player_move_left");
        InputRight = Input.IsActionPressed("player_move_right");
        InputJump = Input.IsActionJustPressed("player_jump");


        Velocity.x = 0;
        Velocity.y += _gravity * delta;

        HandleMovement(Speed);

        _currentState.UpdateState(this);

        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));
    }

    public void SwitchState(PlayerBaseState state)
    {
        _currentState = state;
        _currentState.EnterState(this);
    }

    private void HandleMovement(float speed) 
    {
        if (InputLeft) 
            Velocity.x -= speed;

        if (InputRight)
            Velocity.x += speed;
    }

    public bool IsMoving() => InputLeft || InputRight;
    public bool IsJumping() => InputJump;
    public bool IsFalling() => Velocity.y > 10;

    public async Task Died()
    {
        _dead = true;
        await GameManager.TransitionManager.AlphaToBlackAndBack();
        _dead = false;
        Position = _levelStartPos;
    }

    private async void _on_Player_Area_area_entered(Area2D area)
    {
        if (_dead)
            return;

        if (area.Name == "Bottom") 
        {
            await Died();
        }

        if (area.Name == "Level Finish") 
        {
            await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
        }

        if (area.Name == "Enemy") 
        {
            await Died();
        }

        _currentState.OnAreaEntered(this, area);
    }
}
