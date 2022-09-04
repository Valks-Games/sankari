namespace Sankari;

public static class GodotExtensions
{
    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }

    // enable a layer with Mathf.Pow(2, x - 1) where x is the layer you want enabled
    // if you wanted to enable multiple then add the sum of the powers
    // e.g. Mathf.Pow(2, 1) + Mathf.Pow(2, 3) to enable layers 0 and 2
    public static void EnableLayers(this TileMap tileMap, params uint[] layers)
    {
        uint result = 0;

        foreach (var layer in layers)
            result += UIntPow(2, layer - 1);

        tileMap.CollisionLayer = result;
        tileMap.CollisionMask = result;
    }

    private static uint UIntPow(uint x, uint pow)
    {
        uint ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}