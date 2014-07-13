using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
///		Creates a float/vector field
/// </summary>
public class CreateFields
{
    [MenuItem("Assets/Create/Float Field")]
    public static FloatField CreateFloat()
    {
        FloatField asset = ScriptableObject.CreateInstance<FloatField>();

        AssetDatabase.CreateAsset(asset, "Assets/Terrain/FloatField.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }

    [MenuItem("Assets/Create/Vector Field")]
    public static VectorField CreateVector()
    {
        VectorField asset = ScriptableObject.CreateInstance<VectorField>();

        AssetDatabase.CreateAsset(asset, "Assets/Terrain/VectorField.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}