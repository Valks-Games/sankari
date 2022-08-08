namespace MarioLikeGame;

public class PlayerStateManager : KinematicBody2D
{
    public GameManager GameManager { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public Vector2 Velocity;

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

    public void PreInit(GameManager gameManager)
    {
        GameManager = gameManager;
        LevelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _levelStartPos = Position;
        _currentState = new PlayerIdleState();
        _currentState.EnterState(this);
    }

    public override void _PhysicsProcess(float delta)
    {
        InputLeft = Input.IsActionPressed("player_move_left");
        InputRight = Input.IsActionPressed("player_move_right");
        InputJump = Input.IsActionJustPressed("player_jump");


        Velocity.x = 0;
        Velocity.y += _gravity * delta;

        if (IsOnFloor())
            Velocity.y = 0;

        _currentState.UpdateState(this);

        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));
    }

    public void SwitchState(PlayerBaseState state)
    {
        _currentState = state;
        _currentState.EnterState(this);
    }

    public void HandleMovement(int speed) 
    {
        if (InputLeft) 
            Velocity.x -= speed;

        if (InputRight)
            Velocity.x += speed;
    }

    public bool IsMoving() => InputLeft || InputRight;
    public bool IsJumping() => InputJump;
    public bool IsFalling() => Velocity.y > 10;

    private void _on_Player_Area_area_entered(Area2D area)
    {
        if (area.Name == "Bottom")
            Position = _levelStartPos;

        if (area.Name == "Level Finish")
            LevelManager.CompleteLevel(LevelManager.CurrentLevel);

        if (area.Name == "Enemy")
            Position = _levelStartPos;

        _currentState.OnAreaEntered(this, area);
    }
}
