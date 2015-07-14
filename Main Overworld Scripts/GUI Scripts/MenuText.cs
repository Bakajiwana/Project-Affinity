using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Script Objective: To Update text like difficulty, and so on

public class MenuText : MonoBehaviour 
{
	//Text Variables
	public Text difficultyText;
	public Text speakerModeText;
	public Text verticalSyncText;
	public Text graphicsText;
	public Text meshText;
	public Text lodText;
	public Text textureText;
	public Text shadowText;
	public Text terrainText;
	public Text vegeText;
	public Text waterText;
	public Text particleText;
	public Text msaaText;
	public Text aaText;
	public Text ambientOcclusionText;

	//INITIALISE 
	void Start()
	{
		UpdateDifficultyText();
		UpdateSpeakerMode();
		UpdateVSyncText();
		UpdateAllGraphicsText();
	}

	//Update the Difficulty Text
	public void UpdateDifficultyText()
	{
		int currentDifficulty = PlayerPrefs.GetInt ("Difficulty");
		if(difficultyText)
		{
			switch(currentDifficulty)
			{
			case 1:
				difficultyText.text = "Easy";
				break;
			case 2:
				difficultyText.text = "Normal";
				break;
			case 3:
				difficultyText.text = "Hard";
				break;
			case 4:
				difficultyText.text = "Synergist"; 
				break;
			}
		}
	}

	//Note: This function will also update the Speaker Mode.
	//Reason: Its a lot more easier and efficient this way.
	public void UpdateSpeakerMode()
	{
	/* Speaker Modes:
	 * 1. Auto
	 * 2. Raw
	 * 3. Mono
	 * 4. Prologic
	 * 5. Stereo
	 * 6. Quad
	 * 7. Surround
	 * 8. Mode5point1
	 * 9. Mode7point1
	*/
		int currentSpeakerMode = PlayerPrefs.GetInt ("SpeakerMode");
		if(speakerModeText)
		{
			switch (currentSpeakerMode)
			{
			case 1:
				speakerModeText.text = "Auto";
				AudioSettings.speakerMode = AudioSettings.driverCaps;
				break;
			case 2:
				speakerModeText.text = "Raw";
				AudioSettings.speakerMode = AudioSpeakerMode.Raw;
				break;
			case 3:
				speakerModeText.text = "Mono";
				AudioSettings.speakerMode = AudioSpeakerMode.Mono;
				break;
			case 4:
				speakerModeText.text = "Prologic";
				AudioSettings.speakerMode = AudioSpeakerMode.Prologic;
				break;
			case 5:
				speakerModeText.text = "Stereo";
				AudioSettings.speakerMode = AudioSpeakerMode.Stereo;
				break;
			case 6:
				speakerModeText.text = "Quad";
				AudioSettings.speakerMode = AudioSpeakerMode.Quad;
				break;
			case 7:
				speakerModeText.text = "Surround";
				AudioSettings.speakerMode = AudioSpeakerMode.Surround;
				break;
			case 8:
				speakerModeText.text = "5.1 Surround";
				AudioSettings.speakerMode = AudioSpeakerMode.Mode5point1;
				break;
			case 9:
				speakerModeText.text = "7.1 Surround";
				AudioSettings.speakerMode = AudioSpeakerMode.Mode7point1;
				break; 
			}
		}
	}

	public void UpdateVSyncText()
	{
		int currentVSync = PlayerPrefs.GetInt ("VSync");
		if(verticalSyncText)
		{
			switch(currentVSync)
			{
			case 0:
				verticalSyncText.text = "Off";
				break;
			case 1:
				verticalSyncText.text = "1 Pass";
				break;
			case 2:
				verticalSyncText.text = "2 Pass";
				break;
			}
		}
	}

	public void UpdateGraphicsText()
	{
		int currentValue = PlayerPrefs.GetInt ("GraphicsQuality");
		if(graphicsText)
		{
			switch(currentValue)
			{
			case 0:
				graphicsText.text = "Low";
				break;
			case 1:
				graphicsText.text = "Medium";
				break;
			case 2:
				graphicsText.text = "High";
				break;
			case 3:
				graphicsText.text = "Ultra";
				break;
			case 4:
				graphicsText.text = "Custom";
				break;
			}
		}
	}

	public void UpdateMeshText()
	{
		int currentValue = PlayerPrefs.GetInt ("MeshQuality");
		if(meshText)
		{
			switch(currentValue)
			{
			case 2:
				meshText.text = "Low";
				break;
			case 1:
				meshText.text = "Medium";
				break;
			case 0:
				meshText.text = "High";
				break;
			}
		}
	}
	
	public void UpdateLODText()
	{
		int currentValue = PlayerPrefs.GetInt ("LevelOfDetail");
		if(lodText)
		{
			switch(currentValue)
			{
			case 150:
				lodText.text = "Low";
				break;
			case 500:
				lodText.text = "Medium";
				break;
			case 650:
				lodText.text = "High";
				break;
			case 1000:
				lodText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateTextureText()
	{
		int currentValue = PlayerPrefs.GetInt ("TextureQuality");
		if(textureText)
		{
			switch(currentValue)
			{
			case 2:
				textureText.text = "Low";
				break;
			case 1:
				textureText.text = "Medium";
				break;
			case 0:
				textureText.text = "High";
				break;
			}
		}
	}	

	public void UpdateShadowText()
	{
		int currentValue = PlayerPrefs.GetInt ("ShadowQuality");
		if(shadowText)
		{
			switch(currentValue)
			{
			case 0:
				shadowText.text = "Potato";
				break;
			case 1:
				shadowText.text = "Low";
				break;
			case 2:
				shadowText.text = "Medium";
				break;
			case 3:
				shadowText.text = "High";
				break;
			case 4:
				shadowText.text = "Very High";
				break;
			case 5:
				shadowText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateTerrainText()
	{
		int currentValue = PlayerPrefs.GetInt ("TerrainQuality");
		if(terrainText)
		{
			switch(currentValue)
			{
			case 0:
				terrainText.text = "Low";
				break;
			case 1:
				terrainText.text = "Medium";
				break;
			case 2:
				terrainText.text = "High";
				break;
			case 3:
				terrainText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateVegetationText()
	{
		int currentValue = PlayerPrefs.GetInt ("VegetationQuality");
		if(vegeText)
		{
			switch(currentValue)
			{
			case 0:
				vegeText.text = "Low";
				break;
			case 1:
				vegeText.text = "Medium";
				break;
			case 2:
				vegeText.text = "High";
				break;
			case 3:
				vegeText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateWaterText()
	{
		int currentValue = PlayerPrefs.GetInt ("WaterQuality");
		if(waterText)
		{
			switch(currentValue)
			{
			case 0:
				waterText.text = "Low";
				break;
			case 1:
				waterText.text = "Medium";
				break;
			case 2:
				waterText.text = "High";
				break;
			case 3:
				waterText.text = "Very High";
				break;
			case 4:
				waterText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateParticleText()
	{
		int currentValue = PlayerPrefs.GetInt ("ParticleQuality");
		if(particleText)
		{
			switch(currentValue)
			{
			case 0:
				particleText.text = "Potato";
				break;
			case 1:
				particleText.text = "Low";
				break;
			case 2:
				particleText.text = "Medium";
				break;
			case 3:
				particleText.text = "High";
				break;
			case 4:
				particleText.text = "Very High";
				break;
			case 5:
				particleText.text = "Ultra";
				break;
			}
		}
	}

	public void UpdateMSAAText()
	{
		int currentValue = PlayerPrefs.GetInt ("MSAA");
		if(msaaText)
		{
			switch(currentValue)
			{
			case 0:
				msaaText.text = "Off";
				break;
			case 2:
				msaaText.text = "x2";
				break;
			case 4:
				msaaText.text = "x4";
				break;
			case 8:
				msaaText.text = "x8";
				break;
			}
		}
	}

	public void UpdateAAText()
	{
		int currentValue = PlayerPrefs.GetInt ("AA");
		if(aaText)
		{
			switch(currentValue)
			{
			case 0:
				aaText.text = "Off";
				break;
			case 1:
				aaText.text = "On";
				break;
			}
		}
	}

	public void UpdateAmbientOcclusionText()
	{
		int currentValue = PlayerPrefs.GetInt ("AmbientOcclusion");
		if(ambientOcclusionText)
		{
			switch(currentValue)
			{
			case 0:
				ambientOcclusionText.text = "Off";
				break;
			case 1:
				ambientOcclusionText.text = "Low";
				break;
			case 2:
				ambientOcclusionText.text = "Medium";
				break;
			case 3:
				ambientOcclusionText.text = "High";
				break;
			}
		}
	}

	public void UpdateAllGraphicsText()
	{
		UpdateGraphicsText();
		UpdateMeshText();
		UpdateLODText ();
		UpdateTextureText ();
		UpdateShadowText ();
		UpdateTerrainText ();
		UpdateVegetationText ();
		UpdateWaterText ();
		UpdateParticleText ();
		UpdateMSAAText ();
		UpdateAAText ();
		UpdateAmbientOcclusionText ();
	}
}
