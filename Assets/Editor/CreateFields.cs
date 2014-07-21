using System.IO;
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

		string sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
        AssetDatabase.CreateAsset(asset, "Assets/Scenes/" + sceneName + "/FloatField.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }

    [MenuItem("Assets/Create/Vector Field")]
    public static VectorField CreateVector()
    {
        VectorField asset = ScriptableObject.CreateInstance<VectorField>();

		string sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
		AssetDatabase.CreateAsset(asset, "Assets/Scenes/" + sceneName + "VectorField.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}