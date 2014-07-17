using UnityEngine;
using System.Collections;

[System.Serializable]
public class BrushSettings
{
    public int size;
    public float opacity;
    public int paintChannel;

    public brushMode Mode;
    public enum brushMode
    {
        Raise,
        Smooth
    };
}

public class TerrainPainter
{
    public static void PaintSplats(Map map, RaycastHit hit, BrushSettings settings)
    {
        Vector3 terrainPos = hit.point;
        terrainPos.y = 0.0f;

        Point splatCoord = map.WorldToTextureCoords(terrainPos, map.splatSettings.control.width);

        float brushWidth = settings.size / 2.0f;
        for (int x = -settings.size; x < settings.size; x++)
        {
            for (int y = -settings.size; y < settings.size; y++)
            {
                Color originalVal = map.splatSettings.control.GetPixel(splatCoord.x + x, splatCoord.y + y);

                float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), settings.opacity, Vector2.zero,
                new Vector2(brushWidth, brushWidth));

                Color addVal = new Color(-brushStrength, -brushStrength, -brushStrength);
                //Debug.Log("Brush Strength " + brushStrength);
                addVal[settings.paintChannel - 1] += brushStrength * 2;
                originalVal += addVal;
                originalVal.r = Mathf.Clamp(originalVal.r, 0.0f, 1.0f);
                originalVal.g = Mathf.Clamp(originalVal.g, 0.0f, 1.0f);
                originalVal.b = Mathf.Clamp(originalVal.b, 0.0f, 1.0f);
                map.splatSettings.control.SetPixel(splatCoord.x + x, splatCoord.y + y, originalVal);
            }
        }

        map.splatSettings.control.Apply();
    }

    public static void PaintHeight(Map map, RaycastHit hit, BrushSettings settings)
    {
        Texture2D heightMap = map.heightTexture;
        Vector3 terrainPos = hit.point;
        terrainPos.y = 0.0f;
        Point heightCoord = map.WorldToTextureCoords(terrainPos, heightMap.width);

        float averageHeight = 0.0f;
        for (int x = -settings.size; x < settings.size; x++)
        {
            for (int y = -settings.size; y < settings.size; y++)
            {
                averageHeight += heightMap.GetPixel(heightCoord.x + x, heightCoord.y + y).r;
            }
        }
        averageHeight /= (float)settings.size * (float)settings.size * 4.0f;

        int brushSize = settings.size;
        float brushOpacity = settings.opacity;
        float brushWidth = brushSize / 2.0f;

        for (int x = -brushSize; x < brushSize; x++)
        {
            for (int y = -brushSize; y < brushSize; y++)
            {
                float originalVal = heightMap.GetPixel(heightCoord.x + x, heightCoord.y + y).r;

                //2D gaussian can be used to model a 'soft' paint brush
                float brushStrength = MathTools.Gaussian2D(new Vector2(x, y), brushOpacity, Vector2.zero,
                    new Vector2(brushWidth, brushWidth));
                if (settings.Mode == BrushSettings.brushMode.Raise)
                {
                    originalVal += brushStrength;
                    originalVal = Mathf.Clamp(originalVal, 0.0f, 1.0f);
                }
                else
                {
                    originalVal = originalVal - (originalVal - averageHeight) * brushStrength;
                }

                heightMap.SetPixel(heightCoord.x + x, heightCoord.y + y, new Color(originalVal, originalVal, originalVal));
            }
        }

        heightMap.Apply();

        GameObject hitTile = hit.collider.gameObject;
        Mesh newMesh = TileMeshBuilder.GenerateMesh(hitTile.transform.position);
        hitTile.GetComponent<MeshFilter>().sharedMesh = newMesh;
        hitTile.GetComponent<MeshCollider>().sharedMesh = newMesh;

        TileMeshBuilder.ClearMesh();
        /*
        TilePlacer placer = GetComponent<TilePlacer>();

        float brushSizeWorld = terrainSettings.tileSquareSize * 2.0f;

        TileRender tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, -brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, -brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(+brushSizeWorld, 0, +brushSizeWorld));
        tile.CreateMesh();

        tile = placer.GetTileAt(pos + new Vector3(-brushSizeWorld, 0, +brushSizeWorld));
        tile.CreateMesh();*/
    }
}

