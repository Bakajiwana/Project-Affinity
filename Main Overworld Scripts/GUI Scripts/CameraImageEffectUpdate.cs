using UnityEngine;
using System.Collections;
using System.Reflection;

public class CameraImageEffectUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		/* Check Component names
		Component[] components = gameObject.GetComponents<Component>();
		foreach (Component c in components) 
		{
			Debug.Log(c.GetType());
		}
		*/

		UpdateCameraEffects ();
	}

	void Update () 
	{
		if(Pause.isPaused && Input.GetButtonDown ("Cancel"))
		{
			UpdateCameraEffects ();
		}
	}

	public void UpdateCameraEffects()
	{
		//Update Post Effect Anti Aliasing
		if(PlayerPrefs.GetInt ("AA") == 0)
		{
			GetComponent<AntialiasingAsPostEffect>().enabled = false;
		}
		else
		{
			GetComponent<AntialiasingAsPostEffect>().enabled = true;
		}

		//Ambient Occlusion Update
		Component ssao = gameObject.GetComponent ("SSAOEffect");
		FieldInfo ssaoField = ssao.GetType ().GetField ("m_SampleCount");
		int currentSSAO = PlayerPrefs.GetInt ("AmbientOcclusion");
		switch(currentSSAO)
		{
		case 0:
			GetComponent<SSAOEffect>().enabled = false;
			break;
		case 1:
			ssaoField.SetValue (ssao, 0);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		case 2:
			ssaoField.SetValue (ssao, 1);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		case 3:
			ssaoField.SetValue (ssao, 2);
			GetComponent<SSAOEffect>().enabled = true;
			break;
		}

		//Motion Blur
		Component motionBlur = gameObject.GetComponent ("CameraMotionBlur");
		FieldInfo motionBlurField = motionBlur.GetType ().GetField ("velocityScale");
		float currentMotionBlur = PlayerPrefs.GetFloat ("MotionBlur");
		motionBlurField.SetValue (motionBlur, currentMotionBlur);
		if(currentMotionBlur == 0f)
		{
			GetComponent<CameraMotionBlur>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<CameraMotionBlur>().enabled = true;
		}

		//Bloom
		Component bloom = gameObject.GetComponent ("Bloom");
		FieldInfo bloomField = bloom.GetType ().GetField ("bloomIntensity");
		float currentBloom = PlayerPrefs.GetFloat ("Bloom") * 20f;
		bloomField.SetValue (bloom, currentBloom);
		if(currentBloom == 0f)
		{
			GetComponent<Bloom>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<Bloom>().enabled = true;
		}

		//Sun Shafts
		Component sunShaft = gameObject.GetComponent ("SunShafts");
		FieldInfo sunShaftField = sunShaft.GetType ().GetField ("sunShaftIntensity");
		float currentSunShaft = PlayerPrefs.GetFloat ("SunShaft") * 2f;
		sunShaftField.SetValue (sunShaft, currentSunShaft);
		if(currentSunShaft == 0f)
		{
			GetComponent<SunShafts>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<SunShafts>().enabled = true;
		}

		//Brightness and contrast
		float currentBrightness = PlayerPrefs.GetFloat ("Brightness") * 2f;
		float currentContrast = PlayerPrefs.GetFloat ("Contrast") * 4f;


		Component brightness = gameObject.GetComponent ("Tonemapping");
		FieldInfo brightnessField = brightness.GetType ().GetField ("middleGrey");

		Component contrast = gameObject.GetComponent ("Tonemapping");
		FieldInfo contrastField = contrast.GetType ().GetField ("white");

		brightnessField.SetValue (brightness, currentBrightness);
		contrastField.SetValue (contrast, currentContrast);


		if(currentBrightness == 0f && currentContrast == 0f)
		{
			GetComponent<Tonemapping>().enabled = false;	//Turn it off
		}
		else
		{
			GetComponent<Tonemapping>().enabled = true;
		}
	}

	void OnEnable()
	{
		UpdateCameraEffects ();
	}
}
