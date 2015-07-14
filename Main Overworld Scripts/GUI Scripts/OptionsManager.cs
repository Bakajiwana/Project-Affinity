using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This script will handle the Player Preferences or Options of the game

public class OptionsManager : MonoBehaviour 
{
	//Gameplay Options Variables
	//Difficulty = 1 - Easy, 2- Normal, 3 - Hard, 4 - Synergist
	public int defaultDifficulty = 2; 			//Normal Difficulty
	public static int difficulty; 				//Requires static variable because this will be accessed a lot

	//Options Button
	public Button [] optionsButtons;

	//Interface Options Variables
	//Damage Text Visibility
	public int defaultDamageText = 1; //1 = true, 0 = false
	public Toggle damageTextToggle;	//This is a toggle

	//Native Screen Resolutions
	private int nativeWidth;
	private int nativeHeight;

	//Invert X
	public int defaultInvertX = 0; //1 = true, 0 = false
	public Toggle invertXToggle;

	//Invert Y
	public int defaultInvertY = 0; //1 = true, 0 = false
	public Toggle invertYToggle;

	//Swap Mouse Butttons
	public int defaultSwapMouseButton = 0; // 1 = true, 0 = false
	public Toggle mouseSwapToggle;

	//Mouse Sensitivity 
	public float defaultMouseSensitivity = 5f;
	public Scrollbar mouseSensitivityScrollbar;

	//Mouse Smooth X
	public float defaultMouseSmoothX = 0.05f;
	public Scrollbar mouseSmoothXScrollbar;

	//Mouse Smooth Y
	public float defaultMouseSmoothY = 0.1f;
	public Scrollbar mouseSmoothYScrollbar;

	//Master Volume
	public float defaultMasterVolume = 0.5f;
	private float masterVolume;
	public Scrollbar masterVolumeScrollbar;

	//Sound FX Volume
	public float defaultSFXVolume = 1f;
	public static float sfxVolume = 1f; 
	public Scrollbar sfxVolumeScrollbar;

	//Music Volume
	public float defaultMusicVolume = 1f;
	public static float musicVolume = 1f;
	public Scrollbar musicVolumeScrollbar;

	//Speaker Mode
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
	public int defaultSpeakerMode = 1;

	//Display Variables

	//Fullscreen Mode, will always start up on full screen
	public Toggle fullScreenToggle;

	//Resolution, will always start up on native
	public InputField width;
	public InputField height;

	//Vertical Sync
	public int defaultVSync = 1;

	//Brightness
	public float defaultBrightness = 0.20f;
	public Scrollbar brightnessScrollbar;

	//Contrast
	public float defaultContrast = 0.5f;
	public Scrollbar contrastScrollbar;

	//Mouse Cursor Size
	public float defaultMouseCursorSize = 0.5f;
	public Scrollbar mouseCursorSizeScrollbar;

	//Display Warning Screen
	public float displayWarningTimer;
	public float displayWarningMaxTimer = 30f;
	private bool displayWarning;
	public Text displayWarningCountdownText;
	
	private MainMenuButton mainMenuButton;
	private PauseMenuButton pauseMenuButton;
	private MenuText menuText;

	//Motion Blur
	public float defaultMotionBlur = 0.6f;
	public Scrollbar motionBlurScrollbar;

	//Bloom
	public float defaultBloom = 0.55f;
	public Scrollbar bloomScrollbar;

	//Sun Shafts
	public float defaultSunShaft = 0.22f;
	public Scrollbar sunShaftScrollbar;


	//THE GRAPHICS SETTINGS < OH MY GOD WHY!!! > *ALL UPDATED BY PLAYERPREF*

	void Awake () 
	{
		//Get the native width and height
		nativeWidth = Screen.width;
		nativeHeight = Screen.height;

		//Get Main Menu Button component
		mainMenuButton = GetComponent <MainMenuButton>();
		//If the main menu button compnent was not found find the pause menu script
		if(!mainMenuButton)
		{
			pauseMenuButton = GetComponent <PauseMenuButton>();
		}
		menuText = GetComponent <MenuText>();

		//Gameplay Options Initialise 
		//Difficulty
		if(!PlayerPrefs.HasKey ("Difficulty"))
		{
			PlayerPrefs.SetInt ("Difficulty", defaultDifficulty);
			difficulty = defaultDifficulty; 
		}
		else
		{
			difficulty = PlayerPrefs.GetInt ("Difficulty");
		}

		//Damage Text
		if(!PlayerPrefs.HasKey ("DamageText"))
		{
			PlayerPrefs.SetInt ("DamageText", defaultDamageText);
			if(damageTextToggle)
			{
				damageTextToggle.isOn = true;
			}
		}
		else
		{
			if(damageTextToggle)
			{
				int currentDamageText = PlayerPrefs.GetInt ("DamageText");

				if(currentDamageText == 0)
				{
					damageTextToggle.isOn = false;
				}
				else
				{
					damageTextToggle.isOn = true;
				}
			}
		}

		//Invert X
		if(!PlayerPrefs.HasKey ("InvertX"))
		{
			PlayerPrefs.SetInt ("InvertX", defaultInvertX);
			if(invertXToggle)
			{
				invertXToggle.isOn = false;
			}
		}
		else
		{
			if(invertXToggle)
			{
				int currentInvertX = PlayerPrefs.GetInt ("InvertX");
				
				if(currentInvertX == 0)
				{
					invertXToggle.isOn = false;
				}
				else
				{
					invertXToggle.isOn = true;
				}
			}
		}

		//Invert Y
		if(!PlayerPrefs.HasKey ("InvertY"))
		{
			PlayerPrefs.SetInt ("InvertY", defaultInvertY);
			if(invertYToggle)
			{
				invertYToggle.isOn = false;
			}
		}
		else
		{
			if(invertYToggle)
			{
				int currentInvertY = PlayerPrefs.GetInt ("InvertY");
				
				if(currentInvertY == 0)
				{
					invertYToggle.isOn = false;
				}
				else
				{
					invertYToggle.isOn = true;
				}
			}
		}

		//Mouse Swap Toggle
		if(!PlayerPrefs.HasKey ("MouseSwap"))
		{
			PlayerPrefs.SetInt ("MouseSwap", defaultSwapMouseButton);
			if(mouseSwapToggle)
			{
				mouseSwapToggle.isOn = false;
			}
		}
		else
		{
			if(mouseSwapToggle)
			{
				int currentMouseSwap = PlayerPrefs.GetInt ("MouseSwap");
				
				if(currentMouseSwap == 0)
				{
					mouseSwapToggle.isOn = false;
				}
				else
				{
					mouseSwapToggle.isOn = true;
				}
			}
		}

		//Mouse Sensitivity
		if(mouseSensitivityScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MouseSensitivity"))
			{
				PlayerPrefs.SetFloat ("MouseSensitivity", defaultMouseSensitivity);

				mouseSensitivityScrollbar.value = defaultMouseSensitivity / 10f;
			}
			else
			{
				mouseSensitivityScrollbar.value = PlayerPrefs.GetFloat ("MouseSensitivity") / 10f;
			}
		}

		//Mouse Smooth X
		if(mouseSmoothXScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MouseSmoothX"))
			{
				PlayerPrefs.SetFloat ("MouseSmoothX", defaultMouseSmoothX);
				
				mouseSmoothXScrollbar.value = defaultMouseSmoothX;
			}
			else
			{
				mouseSmoothXScrollbar.value = PlayerPrefs.GetFloat ("MouseSmoothX");
			}
		}

		//Mouse Smooth Y
		if(mouseSmoothYScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MouseSmoothY"))
			{
				PlayerPrefs.SetFloat ("MouseSmoothY", defaultMouseSmoothY);
				
				mouseSmoothYScrollbar.value = defaultMouseSmoothY;
			}
			else
			{
				mouseSmoothYScrollbar.value = PlayerPrefs.GetFloat ("MouseSmoothY");
			}
		}

		//Master Volume
		if(masterVolumeScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MasterVolume"))			
			{
				PlayerPrefs.SetFloat ("MasterVolume", defaultMasterVolume);				
				masterVolumeScrollbar.value = defaultMasterVolume;
				masterVolume = defaultMasterVolume;
				AudioListener.volume = defaultMasterVolume;
			}
			else
			{
				float currentMasterVolume = PlayerPrefs.GetFloat ("MasterVolume");
				masterVolumeScrollbar.value = currentMasterVolume;
				masterVolume = currentMasterVolume;
				AudioListener.volume = currentMasterVolume;
			}
		}

		//SFX Volume
		if(sfxVolumeScrollbar)
		{
			if(!PlayerPrefs.HasKey ("SFXVolume"))
			{
				PlayerPrefs.SetFloat ("SFXVolume", defaultSFXVolume);
				
				sfxVolumeScrollbar.value = defaultSFXVolume;

				sfxVolume = defaultSFXVolume * masterVolume;
			}
			else
			{
				float currentSFXVolume = PlayerPrefs.GetFloat ("SFXVolume");
				sfxVolumeScrollbar.value = currentSFXVolume;
				sfxVolume = currentSFXVolume * masterVolume;
			}
		}

		//Music Volume
		if(musicVolumeScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MusicVolume"))
			{
				PlayerPrefs.SetFloat ("MusicVolume", defaultMusicVolume);
				
				musicVolumeScrollbar.value = defaultMusicVolume;
				
				musicVolume = defaultMusicVolume * masterVolume;
			}
			else
			{
				float currentMusicVolume = PlayerPrefs.GetFloat ("MusicVolume");
				musicVolumeScrollbar.value = currentMusicVolume;
				musicVolume = currentMusicVolume * masterVolume;
			}
		}

		//Speaker Mode
		if(!PlayerPrefs.HasKey ("SpeakerMode"))
		{
			PlayerPrefs.SetInt ("SpeakerMode", defaultSpeakerMode);
		}

		//Resolution
		if(width)
		{
			width.text = Screen.width.ToString();
		}
		if(height)
		{
			height.text = Screen.height.ToString();
		}

		//V Sync
		if(!PlayerPrefs.HasKey ("VSync"))
		{
			PlayerPrefs.SetInt ("VSync", defaultVSync);
			QualitySettings.vSyncCount = defaultVSync; 
		}
		else
		{
			QualitySettings.vSyncCount = PlayerPrefs.GetInt ("VSync");
		}

		//Brightness
		if(brightnessScrollbar)
		{
			if(!PlayerPrefs.HasKey ("Brightness"))
			{
				PlayerPrefs.SetFloat ("Brightness", defaultBrightness);
				
				brightnessScrollbar.value = defaultBrightness;
			}
			else
			{
				brightnessScrollbar.value = PlayerPrefs.GetFloat ("Brightness");
			}
		}

		//Contrast
		if(contrastScrollbar)
		{
			if(!PlayerPrefs.HasKey ("Contrast"))
			{
				PlayerPrefs.SetFloat ("Contrast", defaultContrast);
				
				contrastScrollbar.value = defaultContrast;
			}
			else
			{
				contrastScrollbar.value = PlayerPrefs.GetFloat ("Contrast");
			}
		}

		//Mouse Cursor Size
		if(mouseCursorSizeScrollbar)
		{
			if(!PlayerPrefs.HasKey ("MouseCursorSize"))
			{
				PlayerPrefs.SetFloat ("MouseCursorSize", defaultMouseCursorSize);
				
				mouseCursorSizeScrollbar.value = defaultMouseCursorSize;
			}
			else
			{
				mouseCursorSizeScrollbar.value = PlayerPrefs.GetFloat ("MouseCursorSize");
			}
		}

		//Graphics

		//Motion Blur
		if(!PlayerPrefs.HasKey ("MotionBlur"))
		{
			PlayerPrefs.SetFloat ("MotionBlur", defaultMotionBlur);
		}

		//Bloom
		if(!PlayerPrefs.HasKey ("Bloom"))
		{
			PlayerPrefs.SetFloat ("Bloom", defaultBloom);
		}

		//Sun Shafts
		if(!PlayerPrefs.HasKey ("SunShaft"))
		{
			PlayerPrefs.SetFloat ("SunShaft", defaultSunShaft);
		}

		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		if(!PlayerPrefs.HasKey ("GraphicsQuality"))
		{
			GraphicsMediumQuality (); //Set Everything to Medium Quality on default
		}
		else
		{
			int currentGraphics = PlayerPrefs.GetInt ("GraphicsQuality");

			switch (currentGraphics)
			{
			case 0:
				GraphicsLowQuality ();
				break;
			case 1:
				GraphicsMediumQuality ();
				break;
			case 2:
				GraphicsHighQuality ();
				break;
			case 3:
				GraphicsUltraQuality ();
				break;
			case 4:
				menuText.UpdateAllGraphicsText ();
				break;
			}
		}
	}

	//Initiate
	void Start()
	{
		displayWarningTimer = displayWarningMaxTimer;
	}

	//Update ever frame
	void Update()
	{
		//If Apply Display button is pressed then the display warning will pop up
		//If player doesn't click the accept button within the timer then Display options will reset.
		if(displayWarning)
		{
			if(displayWarningCountdownText)
			{
				int countDown = (int)displayWarningTimer;
				displayWarningCountdownText.text = countDown.ToString ();
			}
			displayWarningTimer -= Time.deltaTime;

			if(displayWarningTimer <= 0f)
			{
				DisplayReset ();
				if(mainMenuButton)
				{
					mainMenuButton.OptionsMenuSwitch (5);	//Get back to the display menu
				}
				else if(pauseMenuButton)
				{
					pauseMenuButton.OptionsMenuSwitch (5);	//Get back to the display menu
				}
				menuText.UpdateVSyncText ();
			}
		}
		else
		{
			displayWarning = false;
			displayWarningTimer = displayWarningMaxTimer;
		}
	}

	//Functions below will be called by Buttons and will just update a player pref to a certain value
	public void SetDifficulty(int _difficulty)
	{
		int currentDifficulty = PlayerPrefs.GetInt ("Difficulty") + _difficulty;

		currentDifficulty = Mathf.Clamp (currentDifficulty, 1, 4);

		PlayerPrefs.SetInt ("Difficulty", currentDifficulty);
		difficulty = currentDifficulty;
	}

	//Update the Damage Text Visibility
	public void SetDamageText()
	{
		if(damageTextToggle)
		{
			if(damageTextToggle.isOn)
			{
				PlayerPrefs.SetInt ("DamageText", 1);
			}
			else
			{
				PlayerPrefs.SetInt ("DamageText", 0);
			}
		}
	}

	//Invert X
	public void SetInvertX()
	{
		if(invertXToggle)
		{
			if(invertXToggle.isOn)
			{
				PlayerPrefs.SetInt ("InvertX", 1);
			}
			else
			{
				PlayerPrefs.SetInt ("InvertX", 0);
			}
		}
	}

	//Invert Y
	public void SetInvertY()
	{
		if(invertYToggle)
		{
			if(invertYToggle.isOn)
			{
				PlayerPrefs.SetInt ("InvertY", 1);
			}
			else
			{
				PlayerPrefs.SetInt ("InvertY", 0);
			}
		}
	}

	//Mouse Swap Button
	public void SetMouseSwap()
	{
		if(mouseSwapToggle)
		{
			if(mouseSwapToggle.isOn)
			{
				PlayerPrefs.SetInt ("MouseSwap", 1);
			}
			else
			{
				PlayerPrefs.SetInt ("MouseSwap", 0);
			}
		}
	}

	//Mouse Sensitivity
	public void SetMouseSensitivity()
	{
		if(mouseSensitivityScrollbar)
		{
			float currentMouseSensitivty = mouseSensitivityScrollbar.value * 10f;

			PlayerPrefs.SetFloat ("MouseSensitivity", currentMouseSensitivty);
		}
	}

	//Mouse Smooth X
	public void SetMouseSmoothX()
	{
		if(mouseSmoothXScrollbar)
		{			
			PlayerPrefs.SetFloat ("MouseSmoothX", mouseSmoothXScrollbar.value);
		}
	}

	//Mouse Smooth Y
	public void SetMouseSmoothY()
	{
		if(mouseSmoothYScrollbar)
		{			
			PlayerPrefs.SetFloat ("MouseSmoothY", mouseSmoothYScrollbar.value);
		}
	}

	//Brightness
	public void SetBrightness()
	{
		if(brightnessScrollbar)
		{			
			PlayerPrefs.SetFloat ("Brightness", brightnessScrollbar.value);
		}
	}

	//Contrast
	public void SetContrast()
	{
		if(contrastScrollbar)
		{
			PlayerPrefs.SetFloat ("Contrast", contrastScrollbar.value);
		}
	}

	//Master Volume
	public void SetMasterVolume()
	{
		if(masterVolumeScrollbar)
		{			
			PlayerPrefs.SetFloat ("MasterVolume", masterVolumeScrollbar.value);
			AudioListener.volume = masterVolumeScrollbar.value;
			masterVolume = masterVolumeScrollbar.value;
		}
	}

	//SFX Volume
	public void SetSFXVolume()
	{
		if(sfxVolumeScrollbar)
		{			
			PlayerPrefs.SetFloat ("SFXVolume", sfxVolumeScrollbar.value);
			sfxVolume = sfxVolumeScrollbar.value * masterVolume;		//Master Volume should also affect SFX
		}
	}

	//Music Volume
	public void SetMusicVolume()
	{
		if(musicVolumeScrollbar)
		{			
			PlayerPrefs.SetFloat ("MusicVolume", musicVolumeScrollbar.value);
			musicVolume = musicVolumeScrollbar.value * masterVolume;		//Master Volume should also affect Music
		}
	}

	//Speaker Modes
	public void SetSpeakerMode(int _mode)
	{
		int currentSpeakerMode = PlayerPrefs.GetInt ("SpeakerMode") + _mode;
		
		currentSpeakerMode = Mathf.Clamp (currentSpeakerMode, 1, 9);
		
		PlayerPrefs.SetInt ("SpeakerMode", currentSpeakerMode);
	}

	//V Sync Modes
	public void SetVSync(int _pass)
	{
		int currentVSync = PlayerPrefs.GetInt ("VSync") + _pass;
		
		currentVSync = Mathf.Clamp (currentVSync, 0, 2);

		PlayerPrefs.SetInt ("VSync", currentVSync);
	}
	
	//Set the Whole Display Options with the Apply Button
	public void ApplyDisplayOptions()
	{
		Screen.fullScreen = fullScreenToggle.isOn;													//FullScreen
		Screen.SetResolution(int.Parse (width.text), int.Parse (height.text), fullScreenToggle.isOn);//Resolution
		QualitySettings.vSyncCount = PlayerPrefs.GetInt ("VSync");									//V Sync
		PlayerPrefs.SetFloat ("Brightness", brightnessScrollbar.value);								//Brightness
		PlayerPrefs.SetFloat ("Contrast", contrastScrollbar.value);									//Contrast
		PlayerPrefs.SetFloat ("MouseCursorSize", mouseCursorSizeScrollbar.value);					//Cursor
	}

	public void SetGraphicsQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("GraphicsQuality") + _value;

		currentValue = Mathf.Clamp (currentValue, 0, 4);

		switch (currentValue)
		{
		case 0:
			GraphicsLowQuality ();
			break;
		case 1:
			GraphicsMediumQuality ();
			break;
		case 2:
			GraphicsHighQuality ();
			break;
		case 3:
			GraphicsUltraQuality ();
			break;
		case 4:
			PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
			menuText.UpdateGraphicsText ();
			break;
		}
	}

	public void SetMeshQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("MeshQuality") - _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 2);
		
		PlayerPrefs.SetInt ("MeshQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}
	
	public void SetLevelOfDetail(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("LevelOfDetail");


		if(_value == 1)
		{
			if(currentValue == 150)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 500);
			}
			else if(currentValue == 500)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 650);
			}
			else if(currentValue == 650)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 1000);
			}
		}
		else if (_value == -1)
		{
			if(currentValue == 1000)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 650);
			}
			else if(currentValue == 650)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 500);
			}
			else if(currentValue == 500)
			{
				PlayerPrefs.SetInt ("LevelOfDetail", 150);
			}
		}

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetTextureQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("TextureQuality") - _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 2);
		
		PlayerPrefs.SetInt ("TextureQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetShadowQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("ShadowQuality") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 5);
		
		PlayerPrefs.SetInt ("ShadowQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetTerrainQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("TerrainQuality") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 3);
		
		PlayerPrefs.SetInt ("TerrainQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetVegetationQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("VegetationQuality") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 3);
		
		PlayerPrefs.SetInt ("VegetationQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetWaterQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("WaterQuality") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 2);
		
		PlayerPrefs.SetInt ("WaterQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetParticleQuality(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("ParticleQuality") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 5);
		
		PlayerPrefs.SetInt ("ParticleQuality", currentValue);

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}
	
	public void SetMSAA(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("MSAA");
		
		if(_value == 1)
		{
			if(currentValue == 0)
			{
				PlayerPrefs.SetInt ("MSAA", 2);
			}
			else if(currentValue == 2)
			{
				PlayerPrefs.SetInt ("MSAA", 4);
			}
			else if(currentValue == 4)
			{
				PlayerPrefs.SetInt ("MSAA", 8);
			}
		}
		else if (_value == -1)
		{
			if(currentValue == 8)
			{
				PlayerPrefs.SetInt ("MSAA", 4);
			}
			else if(currentValue == 4)
			{
				PlayerPrefs.SetInt ("MSAA", 2);
			}
			else if(currentValue == 2)
			{
				PlayerPrefs.SetInt ("MSAA", 0);
			}
		}

		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}
	
	public void SetAA(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("AA") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 1);
		
		PlayerPrefs.SetInt ("AA", currentValue);
		
		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}

	public void SetAmbientOcclusion(int _value)
	{
		int currentValue = PlayerPrefs.GetInt ("AmbientOcclusion") + _value;
		
		currentValue = Mathf.Clamp (currentValue, 0, 3);
		
		PlayerPrefs.SetInt ("AmbientOcclusion", currentValue);
		
		PlayerPrefs.SetInt ("GraphicsQuality", 4);	//Custom
		menuText.UpdateGraphicsText ();
	}
	
	public void SetMotionBlur()
	{
		if(motionBlurScrollbar)
		{			
			PlayerPrefs.SetFloat ("MotionBlur", motionBlurScrollbar.value);
		}
	}

	public void SetBloom()
	{
		if(bloomScrollbar)
		{			
			PlayerPrefs.SetFloat ("Bloom", bloomScrollbar.value);
		}
	}

	public void SetSunShaft()
	{
		if(sunShaftScrollbar)
		{			
			PlayerPrefs.SetFloat ("SunShaft", sunShaftScrollbar.value);
		}
	}

	//RESET FUNCTIONS
	public void GameplayReset()
	{
		PlayerPrefs.SetInt ("Difficulty", defaultDifficulty);
	}

	public void InterfaceReset()
	{
		PlayerPrefs.SetInt ("DamageText", defaultDamageText);
		if(damageTextToggle)
		{
			damageTextToggle.isOn = true;
		}
	}

	public void ControlsReset()
	{
		PlayerPrefs.SetInt ("InvertX", defaultInvertX);
		if(invertXToggle)
		{
			invertXToggle.isOn = false;
		}

		PlayerPrefs.SetInt ("InvertY", defaultInvertY);
		if(invertYToggle)
		{
			invertYToggle.isOn = false;
		}

		PlayerPrefs.SetInt ("MouseSwap", defaultSwapMouseButton);
		if(mouseSwapToggle)
		{
			mouseSwapToggle.isOn = false;
		}

		PlayerPrefs.SetFloat ("MouseSensitivity", defaultMouseSensitivity);		
		if(mouseSensitivityScrollbar)
		{
			mouseSensitivityScrollbar.value = defaultMouseSensitivity / 10f;
		}

		PlayerPrefs.SetFloat ("MouseSmoothX", defaultMouseSmoothX);	
		if(mouseSmoothXScrollbar)
		{
			mouseSmoothXScrollbar.value = defaultMouseSmoothX;
		}

		PlayerPrefs.SetFloat ("MouseSmoothY", defaultMouseSmoothY);		
		if(mouseSmoothYScrollbar)
		{
			mouseSmoothYScrollbar.value = defaultMouseSmoothY;
		}
	}

	public void AudioReset()
	{
		PlayerPrefs.SetFloat ("MasterVolume", defaultMasterVolume);				
		masterVolume = defaultMasterVolume;
		AudioListener.volume = defaultMasterVolume;
		if(masterVolumeScrollbar)
		{
			masterVolumeScrollbar.value = defaultMasterVolume;
		}

		PlayerPrefs.SetFloat ("SFXVolume", defaultSFXVolume);		
		sfxVolume = defaultSFXVolume * masterVolume;
		if(sfxVolumeScrollbar)
		{
			sfxVolumeScrollbar.value = defaultSFXVolume;
		}

		PlayerPrefs.SetFloat ("MusicVolume", defaultMusicVolume);		
		musicVolume = defaultMusicVolume * masterVolume;
		if(musicVolumeScrollbar)
		{
			musicVolumeScrollbar.value = defaultMusicVolume;	
		}

		PlayerPrefs.SetInt ("SpeakerMode", defaultSpeakerMode);
	}

	public void DisplayReset()
	{
		fullScreenToggle.isOn = true;
		Screen.fullScreen = true;													//FullScreen

		width.text = nativeWidth.ToString ();
		height.text = nativeHeight.ToString ();
		Screen.SetResolution(nativeWidth, nativeHeight, Screen.fullScreen);		//Resolution

		QualitySettings.vSyncCount = defaultVSync;									//V Sync
		PlayerPrefs.SetInt ("VSync", defaultVSync);

		PlayerPrefs.SetFloat ("Brightness", defaultBrightness);						//Brightness
		brightnessScrollbar.value = defaultBrightness;

		PlayerPrefs.SetFloat ("Contrast", defaultContrast);							//Contrast
		contrastScrollbar.value = defaultContrast;

		PlayerPrefs.SetFloat ("MouseCursorSize", defaultMouseCursorSize);			//Cursor
		mouseCursorSizeScrollbar.value = defaultMouseCursorSize;

		ApplyDisplayOptions ();	//Apply these reset options
		DisableOptionsButtons (true);
		AcceptDisplayOptions();
	}

	public void GraphicsReset()
	{
		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		PlayerPrefs.SetInt ("GraphicsQuality", 1);	//Medium

		
		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		PlayerPrefs.SetInt ("MeshQuality", 1);	//Medium
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		PlayerPrefs.SetInt ("LevelOfDetail", 500); //Medium
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		PlayerPrefs.SetInt ("TextureQuality", 1); // Medium
		
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		PlayerPrefs.SetInt ("ShadowQuality", 3); 	//High
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("TerrainQuality", 1);	//Medium
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("VegetationQuality", 1);	//Medium
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		PlayerPrefs.SetInt ("WaterQuality", 1);	//Medium
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		PlayerPrefs.SetInt ("ParticleQuality", 2);	//Medium
		
		//Multisample Quality: x0 , x2, x4, x8
		PlayerPrefs.SetInt ("MSAA", 0);	//off

		//Post Effect Anti Aliasing: 0 = Off, 1 = On
		PlayerPrefs.SetInt ("AA", 0);

		//Ambient Occlusion 0 = Off, 1 = Low, 2 = Med, 3 = High
		PlayerPrefs.SetInt ("AmbientOcclusion", 1);

		//Motion Blur
		PlayerPrefs.SetFloat ("MotionBlur", defaultMotionBlur);
		motionBlurScrollbar.value = defaultMotionBlur;

		//Bloom Intensity
		PlayerPrefs.SetFloat ("Bloom", defaultBloom);
		bloomScrollbar.value = defaultBloom;

		//Sun Shaft Intensity
		PlayerPrefs.SetFloat ("SunShaft", defaultSunShaft);
		sunShaftScrollbar.value = PlayerPrefs.GetFloat ("SunShaft");

		menuText.UpdateAllGraphicsText ();
	}

	public void GraphicsLowQuality()
	{
		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		PlayerPrefs.SetInt ("GraphicsQuality", 0);	//Low		
		
		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		PlayerPrefs.SetInt ("MeshQuality", 2);	//Low
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		PlayerPrefs.SetInt ("LevelOfDetail", 500); //Low
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		PlayerPrefs.SetInt ("TextureQuality", 2); // Low
		
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		PlayerPrefs.SetInt ("ShadowQuality", 2); 	//Med
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("TerrainQuality", 1);	//Medium
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("VegetationQuality", 0);	//Low
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		PlayerPrefs.SetInt ("WaterQuality", 0);	//Low
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		PlayerPrefs.SetInt ("ParticleQuality", 0);	//Potato
		
		//Multisample Quality: x0 , x2, x4, x8
		PlayerPrefs.SetInt ("MSAA", 0);	//off

		//Post Effect Anti Aliasing: 0 = Off, 1 = On
		PlayerPrefs.SetInt ("AA", 0);
		
		//Ambient Occlusion 0 = Off, 1 = Low, 2 = Med, 3 = High
		PlayerPrefs.SetInt ("AmbientOcclusion", 0);
		
		//Motion Blur
		motionBlurScrollbar.value = PlayerPrefs.GetFloat ("MotionBlur");
		
		//Bloom Intensity
		bloomScrollbar.value = PlayerPrefs.GetFloat ("Bloom");
		
		//Sun Shaft Intensity
		sunShaftScrollbar.value = PlayerPrefs.GetFloat ("SunShaft");
		
		menuText.UpdateAllGraphicsText ();
	}

	public void GraphicsMediumQuality()
	{
		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		PlayerPrefs.SetInt ("GraphicsQuality", 1);	//Medium
		
		
		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		PlayerPrefs.SetInt ("MeshQuality", 1);	//Medium
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		PlayerPrefs.SetInt ("LevelOfDetail", 500); //Medium
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		PlayerPrefs.SetInt ("TextureQuality", 1); // Medium
		
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		PlayerPrefs.SetInt ("ShadowQuality", 3); 	//High
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("TerrainQuality", 1);	//Medium
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("VegetationQuality", 1);	//Medium
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		PlayerPrefs.SetInt ("WaterQuality", 1);	//Medium
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		PlayerPrefs.SetInt ("ParticleQuality", 2);	//Medium
		
		//Multisample Quality: x0 , x2, x4, x8
		PlayerPrefs.SetInt ("MSAA", 2);	//x2

		//Post Effect Anti Aliasing: 0 = Off, 1 = On
		PlayerPrefs.SetInt ("AA", 0);
		
		//Ambient Occlusion 0 = Off, 1 = Low, 2 = Med, 3 = High
		PlayerPrefs.SetInt ("AmbientOcclusion", 1);
		
		//Motion Blur
		motionBlurScrollbar.value = PlayerPrefs.GetFloat ("MotionBlur");
		
		//Bloom Intensity
		bloomScrollbar.value = PlayerPrefs.GetFloat ("Bloom");
		
		//Sun Shaft Intensity
		sunShaftScrollbar.value = PlayerPrefs.GetFloat ("SunShaft");
		
		menuText.UpdateAllGraphicsText ();
	}

	public void GraphicsHighQuality()
	{
		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		PlayerPrefs.SetInt ("GraphicsQuality", 2);	//High
		
		
		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		PlayerPrefs.SetInt ("MeshQuality", 0);	//High
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		PlayerPrefs.SetInt ("LevelOfDetail", 650); //High
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		PlayerPrefs.SetInt ("TextureQuality", 0); //High
		
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		PlayerPrefs.SetInt ("ShadowQuality", 4); 	//Very High
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("TerrainQuality", 2);	//High
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("VegetationQuality", 2);	//High
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		PlayerPrefs.SetInt ("WaterQuality", 2);	//High
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		PlayerPrefs.SetInt ("ParticleQuality", 4);	//Very High
		
		//Multisample Quality: x0 , x2, x4, x8
		PlayerPrefs.SetInt ("MSAA", 4);	//x4

		//Post Effect Anti Aliasing: 0 = Off, 1 = On
		PlayerPrefs.SetInt ("AA", 1);
		
		//Ambient Occlusion 0 = Off, 1 = Low, 2 = Med, 3 = High
		PlayerPrefs.SetInt ("AmbientOcclusion", 2);
		
		//Motion Blur
		motionBlurScrollbar.value = PlayerPrefs.GetFloat ("MotionBlur");
		
		//Bloom Intensity
		bloomScrollbar.value = PlayerPrefs.GetFloat ("Bloom");
		
		//Sun Shaft Intensity
		sunShaftScrollbar.value = PlayerPrefs.GetFloat ("SunShaft");
		
		menuText.UpdateAllGraphicsText ();
	}

	public void GraphicsUltraQuality()
	{
		//Graphics Quality: 0 = Low, 1 = Medium, 2 = High, 3 = Ultra, 4 = Custom
		PlayerPrefs.SetInt ("GraphicsQuality", 3);	//Ultra
		
		
		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		PlayerPrefs.SetInt ("MeshQuality", 0);	//High
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		PlayerPrefs.SetInt ("LevelOfDetail", 1000); //Ultra
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		PlayerPrefs.SetInt ("TextureQuality", 0); //High
		
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		PlayerPrefs.SetInt ("ShadowQuality", 5); 	//Ultra
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("TerrainQuality", 3);	//Ultra
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		PlayerPrefs.SetInt ("VegetationQuality", 3);	//Ultra
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		PlayerPrefs.SetInt ("WaterQuality", 2);	//Ultra
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		PlayerPrefs.SetInt ("ParticleQuality", 5);	//Ultra
		
		//Multisample Quality: x0 , x2, x4, x8
		PlayerPrefs.SetInt ("MSAA", 8);	//x8

		//Post Effect Anti Aliasing: 0 = Off, 1 = On
		PlayerPrefs.SetInt ("AA", 1);
		
		//Ambient Occlusion 0 = Off, 1 = Low, 2 = Med, 3 = High
		PlayerPrefs.SetInt ("AmbientOcclusion", 3);
		
		//Motion Blur
		motionBlurScrollbar.value = PlayerPrefs.GetFloat ("MotionBlur");
		
		//Bloom Intensity
		bloomScrollbar.value = PlayerPrefs.GetFloat ("Bloom");
		
		//Sun Shaft Intensity
		sunShaftScrollbar.value = PlayerPrefs.GetFloat ("SunShaft");
		
		menuText.UpdateAllGraphicsText ();
	}


	public void DisableOptionsButtons(bool _interactable)
	{
		if(!_interactable)
		{
			foreach (Button buttons in optionsButtons)
			{
				buttons.interactable = true;
			}
		}
		else
		{
			foreach (Button buttons in optionsButtons)
			{
				buttons.interactable = false;
			}
		}
	}

	public void DisplayWarning(bool _warning)
	{
		displayWarning = _warning;
	}

	public void AcceptDisplayOptions()
	{
		displayWarningTimer = displayWarningMaxTimer;
		displayWarning = false;
		DisableOptionsButtons (false);
	}
}
