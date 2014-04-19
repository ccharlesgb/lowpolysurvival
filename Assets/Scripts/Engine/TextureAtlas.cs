using UnityEngine;
using System.Collections;

public class TextureAtlas : MonoBehaviour
{
	public Material material;
		
	public int atlasTextureSize = 1024;
	public int atlasGridCount = 8;
	public int atlasPixelsPerBlock = 0;
	
	public TextureAtlas()
	{
		atlasPixelsPerBlock = atlasTextureSize / atlasGridCount;	
	}
	
	//Atlas coordinates go like
	// 9  10  11  12  13  14  15  16
	// 1  2   3   4   5   6   7   8
	public Vector2[] CoordToUV(int coord)
	{
		atlasTextureSize = material.mainTexture.width;
		atlasPixelsPerBlock = material.mainTexture.width / atlasGridCount;
		
		Vector2 uvTop = new Vector2();
		uvTop.x = (float)(coord % atlasGridCount) / (float)atlasGridCount;
		uvTop.y = Mathf.FloorToInt(coord / atlasGridCount) / (float)atlasGridCount;
		//uvTop.x += 0.5f / atlasTextureSize;
		//uvTop.y += 0.5f / atlasTextureSize;
		Vector2 uvSize = new Vector2();
		uvSize.x = 1.0f / (float)atlasGridCount;
		uvSize.y = 1.0f / (float)atlasGridCount;
		//uvSize.x -= 0.5f / atlasTextureSize;
		//uvSize.y -= 0.5f / atlasTextureSize;
		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(uvTop.x, uvTop.y + uvSize.y);
		uvs[1] = new Vector2(uvTop.x + uvSize.x, uvTop.y + uvSize.y);
		uvs[2] = new Vector2(uvTop.x + uvSize.x, uvTop.y);
		uvs[3] = new Vector2(uvTop.x, uvTop.y);
		
		return uvs;
	}
			
}

