namespace Sankari;

public partial class Trigger : Node
{
    [Export] public string[] Entities; // the entities this trigger effects
    [Export] public TriggerAction Action; // the action that this trigger will execute when activated
    [Export] public EntityType AllowedEntities; // only these entities will activate this trigger
    [Export] public bool OnlyExecuteOnce;
    [Export] public BasicEnemy BasicEnemy;

    [Export] protected  NodePath NodePathEntities;

    private Area2D area2D;
    private Node entities;

    public override void _Ready()
    {
        area2D = GetParent<Area2D>();
        entities = GetNode<Node>(NodePathEntities);
        area2D.Connect("area_entered",new Callable(this,nameof(OnAreaEntered)));
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent() is not Player)
            return;

        foreach (var entityName in Entities)
        {
            var entity = entities.GetNodeOrNull(entityName) as IEntity;

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
            area2D.SetDeferred("monitoring", false);
            area2D.SetDeferred("monitorable", false);
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
