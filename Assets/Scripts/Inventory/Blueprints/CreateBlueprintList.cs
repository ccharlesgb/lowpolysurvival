using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateBlueprintList
{
    [MenuItem("Assets/Create/Blueprint List")]
    public static BlueprintList Create()
    {
        BlueprintList asset = ScriptableObject.CreateInstance<BlueprintList>();

        AssetDatabase.CreateAsset(asset, "Assets/Inventory/BlueprintList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
