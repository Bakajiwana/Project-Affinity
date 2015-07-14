using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreenFade : MonoBehaviour 
{
	public CanvasGroup image;
	public float delayTimer = 1f;
	public float fadeSpeed = 0.05f;

	public bool isStart = false;

	void Start()
	{
		if(image)
		{
			image.alpha = 1f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!isStart)
		{
			if(delayTimer > 0f)
			{
				delayTimer -= Time.deltaTime;
			}
			else
			{
				image.alpha -= Time.deltaTime * fadeSpeed;

				if(image.alpha <= 0f)
				{
					Destroy (gameObject);
				}
			}
		}
	}

	public void LoadLevel(string _level)
	{
		Application.LoadLevel (_level);
	}
}
