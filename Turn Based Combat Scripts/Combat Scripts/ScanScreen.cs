using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script is used as the interface of the Scan Screen

public class ScanScreen : MonoBehaviour 
{
	private Animator anim; 

	public CanvasGroup canvas; 

	private bool scanActive;
	public float fadeSpeed = 2f;

	private float xDrag = 50f;
	private float yDrag = 50f;

	public float sensitivity = 5f;

	//Texts
	public Text top;
	public Text bottom;
	public Text left;
	public Text right;

	private float yTop = 0;
	private float yBottom = 0;
	private float xLeft = 0;
	private float xRight = 0;

	// Use this for initialization
	void Awake () 
	{
		anim = gameObject.GetComponent<Animator>();

		scanActive = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(scanActive)
		{
			//Fade in
			if(canvas.alpha < 1f)
			{
				canvas.alpha += Time.deltaTime * fadeSpeed;
			}

			//Scan Mode Controls


			//Mouse movement 
			if(Input.GetAxis ("Mouse X") != 0f)
			{
				xDrag += Input.GetAxis ("Mouse X") * sensitivity;
				xDrag = Mathf.Clamp (xDrag, 0f, 100f);
				anim.SetFloat ("Horizontal", xDrag);
			}

			if(Input.GetAxis ("Mouse Y") != 0f)
			{
				yDrag += Input.GetAxis ("Mouse Y") * sensitivity;
				yDrag = Mathf.Clamp (yDrag, 0f, 100f);
				anim.SetFloat ("Vertical", yDrag);
			}


			yTop = (yDrag/100f) * 100f;
			yTop = (int)yTop;
			yBottom = 100f - yTop;

			xRight = (xDrag/100f) * 100f;
			xRight = (int)xRight;
			xLeft = 100f - xRight;

			top.text = yTop.ToString ();
			bottom.text = yBottom.ToString ();
			left.text = xLeft.ToString ();
			right.text = xRight.ToString ();
		}
		else
		{
			// fade out
			if(canvas.alpha > 0f)
			{
				canvas.alpha -= Time.deltaTime * fadeSpeed;
			}
		}
	}

	public void RevealScanScreen(bool _reveal)
	{
		if(_reveal)
		{
			scanActive = true;
		}
		else
		{
			scanActive = false;
		}
	}

	public void SubmitScanValues()
	{
		//Get the values and store then in an integer array
		int[] scanValues = new int[4];
		scanValues[0] = (int)yTop;
		scanValues[1] = (int)yBottom;
		scanValues[2] = (int)xRight;
		scanValues[3] = (int)xLeft;

		//Send Message to CombatUIManager
		GameObject.FindGameObjectWithTag ("Combat UI").SendMessage ("SetScanValues", scanValues, SendMessageOptions.DontRequireReceiver);
	}
}
