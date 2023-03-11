namespace Sankari;

public class SlimeAnimationPreJumpStart : EntityAnimation<Slime>
{
    public SlimeAnimationPreJumpStart(Slime entity) : base(entity) => Entity = entity;

    public override void Enter()
    {
        Entity.Jump += SlimeJump;

        Entity.AnimatedSprite.Play("pre_jump_start");
    }

    public override void Update()
    {
        var shakeStrength = 0.4f;

        Entity.AnimatedSprite.Offset = new Vector2(-shakeStrength + new Random().NextSingle() * shakeStrength * 2, 0);
    }

    public override void Exit()
    {
        Entity.Jump -= SlimeJump;
    }

    private void SlimeJump()
    {
        SwitchState(EntityAnimationType.JumpStart);
    }
}
