namespace Sankari;

public class PlayerCommandDeath : PlayerCommand<Player>
{
    public PlayerCommandDeath(Player player) : base(player) { }

    public override void Start()
    {
        // death animation goes through killzone a 2nd time, make sure Kill() isnt called twice
        if (Entity.HaltLogic) 
            return;

        Entity.HaltLogic = true;

        Events.Player.Notify(EventPlayer.OnDied);
        
        Entity.LevelScene.Camera.StopFollowingPlayer();
        Entity.AnimatedSprite.Stop();

        var dieStartPos = Entity.Position.Y;
        var goUpDuration = 1.25f;

        Entity.DieTween.Create();

        // animate y position
        Entity.DieTween.Animate
        (
            "position:y",
            dieStartPos - 80,
            goUpDuration
        );

        Entity.DieTween.Animate
        (
            "position:y",
            dieStartPos + 400,
            1.5f,
            true
        )
        .From(dieStartPos - 80);

        // animate rotation
        Entity.DieTween.Animate
        (
            "rotation",
            Mathf.Pi,
            1.5f,
            true
        );

        //Entity.DieTween.Start();
        Entity.DieTween.Callback(Entity.OnDieTweenCompleted);
    }
}
