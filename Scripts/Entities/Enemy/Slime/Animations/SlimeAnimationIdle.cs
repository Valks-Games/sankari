﻿namespace Sankari;

public class SlimeAnimationIdle : EntityAnimationIdle<Slime>
{
    public SlimeAnimationIdle(Slime entity) : base(entity) { }

    public override void HandleTransitions()
    {
        if (Entity.IsNearGround() && !Entity.StartedPreJump && !Entity.IdleTimer.IsActive())
        {
            Entity.StartedPreJump = true;
            Entity.AnimatedSprite.Play("pre_jump_start");
            Entity.PreJumpTimer.StartMs();
            SwitchState(EntityAnimationType.PreJumpStart);
        }
    }
}
