using UnityEngine;
using System.Collections;

//Script objective: holographic 2D offset

//Script Source: https://www.youtube.com/watch?v=IzXUQz2inJs

public class TextureOffsetScript : MonoBehaviour 
{
	public int matIndex = 0;
	public Vector2 AnimRate = new Vector2 (1.0f, 0.0f);
	public string textureName = "_MainTex";
	
	Vector2 Offset= Vector2.zero;
	
	
	// Update is called once per frame
	void LateUpdate () 
	{
		Offset += (AnimRate * Time.deltaTime);
		
		if(renderer.enabled)
		{
			renderer.materials[matIndex ].SetTextureOffset(textureName, Offset);
		}
	}
}
