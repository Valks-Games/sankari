namespace Sankari;

public class Trigger : Area2D
{
    [Export] public string[] Entities; // the entities this trigger effects
    [Export] public TriggerAction Action; // the action that this trigger will execute when activated
    [Export] public EntityType AllowedEntities; // only these entities will activate this trigger

    [Export] protected readonly NodePath NodePathEntities;

    private Node entities;

    public override void _Ready()
    {
        entities = GetNode<Node>(NodePathEntities);
        Connect("area_entered", this, nameof(OnAreaEntered));
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent() is not Player)
            return;

        foreach (var entityName in Entities)
        {
            var entity = entities.GetNode(entityName) as IEntity;

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
