namespace Sankari;

public static class GodotExtensions 
{
    public static void QueueFreeChildren(this Node parentNode) 
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }
}