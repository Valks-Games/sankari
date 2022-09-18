namespace Sankari;

public partial class Trigger : Node
{
    [Export] public string[] Entities { get; set; } // the entities this trigger effects
    [Export] public TriggerAction Action { get; set; } // the action that this trigger will execute when activated
    [Export] public EntityType AllowedEntities { get; set; } // only these entities will activate this trigger
    [Export] public bool OnlyExecuteOnce { get; set; }
    [Export] public BasicEnemy BasicEnemy { get; set; }

    [Export] protected NodePath NodePathEntities { get; set; }

    private Area2D Area2D { get; set; }
    private Node NodeEntities { get; set; }

    public override void _Ready()
    {
        Area2D = GetParent<Area2D>();
        NodeEntities = GetNode<Node>(NodePathEntities);
        Area2D.Connect("area_entered",new Callable(this,nameof(OnAreaEntered)));
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent() is not Player)
            return;

        foreach (var entityName in Entities)
        {
            var entity = NodeEntities.GetNodeOrNull(entityName) as IEntity;

            if (entity == null)
                continue;

            switch (Action)
            {
                case TriggerAction.Activate:
                    entity.Activate();
                    break;
                case TriggerAction.Deactivate:
                    entity.Deactivate();
                    break;
                case TriggerAction.Destroy:
                    entity.Destroy();
                    break;
            }
        }

        if (OnlyExecuteOnce) 
        {
            Area2D.SetDeferred("monitoring", false);
            Area2D.SetDeferred("monitorable", false);
        }
    }
}

public enum EntityType 
{
    Player,
    Enemy
}

public enum TriggerAction
{
    Activate,
    Deactivate,
    Destroy
}
