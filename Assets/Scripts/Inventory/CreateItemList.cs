using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
///		Creates an inventory asset.
/// </summary>
public class CreateItemList
{
	[MenuItem("Assets/Create/Inventory Item List")]
	public static ItemList Create()
	{
		ItemList asset = ScriptableObject.CreateInstance<ItemList>();

		AssetDatabase.CreateAsset (asset, "Assets/Inventory/ItemList.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}
}
