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
    public bool InputDash { get; private set; }

    public bool DashReady = true;

    public PlayerIdleState PlayerIdleState = new();
    public PlayerMovingState PlayerMovingState = new();
    public PlayerJumpingState PlayerJumpingState = new();
    public PlayerFallingState PlayerFallingState = new();

    private PlayerBaseState _currentState;
    private const float _gravity = 200;
    private Vector2 _levelStartPos;
    private bool _haltPlayerLogic;
    private GTimer _dashTimer;

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
        _dashTimer = new GTimer(this, nameof(OnDashReady), 1000, false, false);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_haltPlayerLogic)
            return;

        InputLeft = Input.IsActionPressed("player_move_left");
        InputRight = Input.IsActionPressed("player_move_right");
        InputJump = Input.IsActionJustPressed("player_jump");
        InputDash = Input.IsActionJustPressed("player_dash");


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
        {
            //Direction = Direction.West;
            Velocity.x -= speed;
        }

        if (InputRight) 
        {
            //Direction = Direction.East;
            Velocity.x += speed;
        }

        if (InputDash && DashReady)
        {
            DashReady = false;
            _dashTimer.Start();
            Velocity += Velocity * 10;
        }
    }

    public bool IsMoving() => InputLeft || InputRight;
    public bool IsJumping() => InputJump;
    public bool IsFalling() => Velocity.y > 10;

    public async Task Died()
    {
        _haltPlayerLogic = true;
        await GameManager.TransitionManager.AlphaToBlackAndBack();
        _haltPlayerLogic = false;
        //Position = _levelStartPos;
        GameManager.LevelManager.LoadLevel();
    }

    private void OnDashReady()
    {
        DashReady = true;
    }

    private async void _on_Player_Area_area_entered(Area2D area)
    {
        if (_haltPlayerLogic)
            return;

        if (area.IsInGroup("Killzone")) 
        {
            await Died();
            return;
        }

        if (area.IsInGroup("Level Finish"))
        {
            _haltPlayerLogic = true;
            await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
            _haltPlayerLogic = false;
            return;
        }

        if (area.IsInGroup("Enemy")) 
        {
            await Died();
            return;
        }

        if (area.IsInGroup("Coin"))
        {
            GameManager.Audio.PlaySFX("coin_pickup");
            area.GetParent().QueueFree();
        }

        _currentState.OnAreaEntered(this, area);
    }
}
