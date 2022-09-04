namespace Sankari;

public static class ExtensionsTileMap 
{
    // enable a layer with Mathf.Pow(2, x - 1) where x is the layer you want enabled
    // if you wanted to enable multiple then add the sum of the powers
    // e.g. Mathf.Pow(2, 1) + Mathf.Pow(2, 3) to enable layers 0 and 2
    public static void EnableLayers(this TileMap tileMap, params uint[] layers)
    {
        uint result = 0;

        foreach (var layer in layers)
            result += Math.UIntPow(2, layer - 1);

        tileMap.CollisionLayer = result;
        tileMap.CollisionMask = result;
    }

    public static string GetTileName(this TileMap tilemap, Vector2 pos) =>
        tilemap.TileSet.TileGetName(tilemap.GetCurrentTileId(pos));

    private static int GetCurrentTileId(this TileMap tilemap, Vector2 pos)
    {
        var cellPos = tilemap.WorldToMap(pos);
        return tilemap.GetCellv(cellPos);
    }
}