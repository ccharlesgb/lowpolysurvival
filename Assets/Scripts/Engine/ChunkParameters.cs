using UnityEngine;
using System.Collections;

public class ChunkParameters : MonoBehaviour {
	
	public int size = 16;
	public float cubeSize = 1;
	public IntCoord chunkBounds;
	
	void Awake()
	{
		UpdateParams();
	}
	
	void UpdateParams()
	{
		chunkBounds = new IntCoord(size, size, size);		
	}
	
	void OnInspectorGUI()
	{
		UpdateParams();
	}
}
