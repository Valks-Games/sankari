namespace Sankari;

public static class ExtensionsNode 
{
    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }
}