using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 
using UnityEngine.UI;


//Script Objective: Save and Load. Managers and Objects will obtain information from this script.
//Source: Using the code from http://wiki.unity3d.com/index.php/Save_and_Load_from_XML. Tweaked to my preference

public class SaveLoadManager : MonoBehaviour 
{
	private string _FileLocation;
	private string _FileName; 
	UserData myData; 
	string _data; 
	private int saveNumber;	

	//Static variables
	public static Vector3 savePosition; 	//Posiion of player
	public static string saveTime;			//Time
	public static string saveDate;			//Date
	public static float savePlayTime;		//Play Time
	public static string saveLevelName;		//Location Name
	public static int latestSave;
	public static bool latestQuicksave;
	public static bool latestAutosave;

	//Independant Variables
	public long saveLatestTime;				//Latest Time

	private CharacterManager characterManager;

	//This Object can be a button too
	public Text buttonFileName;
	public Text buttonLocation;
	public Text buttonPlayTime;
	public Text buttonDate;
	public Text buttonTime;
	public Text buttonLatest;

	public bool isLoadButton;
	public bool isSaveButton;
	public bool isContinueButton;

	public Transform loadGameTravel;

	//Hovering
	private bool isHover;

	//Quick Save and Autosave booleans
	public bool isQuicksave;
	public bool isAutosave;

	void Awake()
	{
		// Where we want to save and load to and from 
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 

		/* This Fails to work on build
		string filePath = Application.dataPath + System.IO.Path.GetDirectoryName ("Save Games");

		if(!Directory.Exists (filePath))
		{
			//print ("It Is Here");
			//AssetDatabase.CreateFolder ("Assets", "Save Games");
			Directory.CreateDirectory (filePath);
		}
		*/

		// we need soemthing to store the information into 
		myData = new UserData(); 
	}

	// When the EGO is instansiated the Start will trigger 
	// so we setup our initial values for our local members 
	void Start () 
	{ 		
		if(GameObject.FindGameObjectWithTag("Adventure Manager"))
		{
			characterManager = GameObject.FindGameObjectWithTag("Adventure Manager").GetComponent<CharacterManager>();
		}
	} 

	void Update()
	{
		if(isHover && Input.GetKeyDown (KeyCode.Delete) && !isQuicksave && !isAutosave)
		{
			DeleteSave ();
		}

		//Quick Save
		if(Input.GetKeyDown (KeyCode.F5) && !Pause.isPaused && !CharacterManager.isBusy && !GameOver.isGameOver)
		{
			Quicksave ();
		}

		//Quick Load
		if(Input.GetKeyDown (KeyCode.F9) && !Pause.isPaused && !CharacterManager.isBusy && !GameOver.isGameOver)
		{
			GameObject loadTravel;
			loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
			Quickload ();
			loadGame.SetSaveNumber (saveNumber, saveLevelName);
			loadGame.quickload = true;
			DontDestroyOnLoad (loadTravel);
			Application.LoadLevel ("Loading Scene");
		}
	}

	//Load Function called from start of scene to Initialise level/ game
	public void Load(int _saveFileNumber)
	{
		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		// Load our UserData into myData 
		LoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 

			//Load the save position
			savePosition = new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);

			//Load the Time
			saveTime = myData._iUser.time;

			//Load the Date
			saveDate = myData._iUser.date;

			//Load the Play Time
			savePlayTime = myData._iUser.playTime;

			//Load Level Name
			saveLevelName = myData._iUser.levelName;

			//Load Latest Time
			saveLatestTime = myData._iUser.latestTime;

			// just a way to show that we loaded in ok 
			//Debug.Log(myData._iUser.x); 
		} 
	}

	//This function is used to 
	public void LoadButtonUpdate(int _saveFileNumber)
	{
		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		// Load our UserData into myData 
		if(isQuicksave)
		{
			Quickload ();
		}
		else if (isAutosave)
		{
			Autoload ();
		}
		else
		{
			LoadXML(); 
		}
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			//Name of Save File
			if(isAutosave)
			{
				buttonFileName.text = "Autosave";
			}
			else if(isQuicksave)
			{
				buttonFileName.text = "Quicksave";
			}
			else
			{
				buttonFileName.text = "Save Game "+saveNumber.ToString ();
			}
			
			//Load the Time
			buttonTime.text = "Time: " + myData._iUser.time;

			
			//Load the Date
			buttonDate.text = "Date: " + myData._iUser.date;
			
			//Load the Play Time
			//Display time
			float min = (myData._iUser.playTime/60f);
			float sec = (myData._iUser.playTime % 60f);
			float fraction = ((myData._iUser.playTime * 10) %10);
			buttonPlayTime.text = "Hours Played: "+string.Format("{0:00}:{1:00}:{2:00}",min,sec,fraction);
			//buttonPlayTime.text = "Hours Played: " + myData._iUser.playTime.ToString ();

			
			//Load Level Name
			buttonLocation.text = myData._iUser.levelName;

			saveLatestTime = myData._iUser.latestTime;
			
			// just a way to show that we loaded in ok 
			//Debug.Log(myData._iUser.x); 
		}
	}

	//Save Function called from save button
	public void Save(int _saveFileNumber)
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		saveNumber = _saveFileNumber;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 

		//Save Position
		characterManager.SavePlayerPosition ();
		myData._iUser.x = savePosition.x; 
		myData._iUser.y = savePosition.y; 
		myData._iUser.z = savePosition.z;  

		//Time
		saveTime = System.DateTime.Now.ToString("HH:mm:ss tt");
		myData._iUser.time = saveTime; 
		print (saveTime);

		//Date
		saveDate = System.DateTime.Now.ToString("dd/MM/yyyy");
		myData._iUser.date = saveDate;
		print (saveDate);

		//Play Time
		savePlayTime += Time.realtimeSinceStartup;
		myData._iUser.playTime = savePlayTime; 

		//Level Name
		saveLevelName = Application.loadedLevelName;
		myData._iUser.levelName = saveLevelName; 

		//Latest Time
		saveLatestTime = long.Parse (System.DateTime.Now.ToString ("yyyyMMddHHmmss"));
		myData._iUser.latestTime = saveLatestTime; 
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}

	//This function creates a new save
	public void NewSave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		//Save Position
		characterManager.SavePlayerPosition ();
		myData._iUser.x = savePosition.x; 
		myData._iUser.y = savePosition.y; 
		myData._iUser.z = savePosition.z;  
		
		//Time
		saveTime = System.DateTime.Now.ToString("HH:mm:ss tt");
		myData._iUser.time = saveTime; 
		print (saveTime);
		
		//Date
		saveDate = System.DateTime.Now.ToString("dd/MM/yyyy");
		myData._iUser.date = saveDate;
		print (saveDate);
		
		//Play Time
		savePlayTime += Time.realtimeSinceStartup;
		myData._iUser.playTime = savePlayTime; 
		
		//Level Name
		saveLevelName = Application.loadedLevelName;
		myData._iUser.levelName = saveLevelName; 
		
		//Latest Time
		saveLatestTime = long.Parse (System.DateTime.Now.ToString ("yyyyMMddHHmmss"));
		myData._iUser.latestTime = saveLatestTime; 
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 	
		CreateNewXML ();

		//Debug.Log(_data); 
	}

	//This function marks this Save as the latest
	public void MarkLatest()
	{
		buttonLatest.gameObject.SetActive (true);
	}

	public void SaveLoadButtonClick()
	{
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 

		DirectoryInfo  di = new DirectoryInfo (_FileLocation);		
		int numXML = di.GetFiles("*.xml", SearchOption.TopDirectoryOnly).Length;

		if(numXML > 0)
		{
			if(isLoadButton)
			{
				GameObject loadTravel;
				loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
				if(isAutosave)
				{
					Autoload ();
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
					loadGame.autoload = true;
				}
				else if(isQuicksave)
				{
					Quickload ();
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
					loadGame.quickload = true;
				}
				else
				{
					Load (saveNumber);
					loadGame.SetSaveNumber (saveNumber, saveLevelName);
				}
				DontDestroyOnLoad (loadTravel);
				Application.LoadLevel ("Loading Scene");
			}
			if(isSaveButton)
			{
				Save (saveNumber);
				LoadButtonUpdate(saveNumber);
				latestSave = saveNumber;
				print (saveLatestTime);
				GameObject.FindGameObjectWithTag("Adventure Manager").SendMessage ("PauseSwitch");
				//Debug.Log("Saved to Game Save " +saveNumber);
			}
	
			if(isContinueButton)
			{
				GameObject loadTravel;
				loadTravel = Instantiate (loadGameTravel.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				LoadGameTravel loadGame = loadTravel.GetComponent<LoadGameTravel>();
				if(latestQuicksave)
				{
					Quickload ();
					loadGame.quickload = true;
				}
				else if (latestAutosave)
				{
					Autosave ();
					loadGame.autoload = true;
				}
				else
				{
					Load (latestSave);
				}
				loadGame.SetSaveNumber (latestSave, saveLevelName);
				DontDestroyOnLoad (loadTravel);
				Application.LoadLevel ("Loading Scene");
			}
		}
	}

	//Autosave function
	public void Autosave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		_FileName="Autosave.xml"; 
		
		//Save Position
		myData._iUser.x = savePosition.x; 
		myData._iUser.y = savePosition.y; 
		myData._iUser.z = savePosition.z;  
		
		//Time
		saveTime = System.DateTime.Now.ToString("HH:mm:ss tt");
		myData._iUser.time = saveTime; 
		print (saveTime);
		
		//Date
		saveDate = System.DateTime.Now.ToString("dd/MM/yyyy");
		myData._iUser.date = saveDate;
		print (saveDate);
		
		//Play Time
		savePlayTime += Time.realtimeSinceStartup;
		myData._iUser.playTime = savePlayTime; 
		
		//Level Name
		saveLevelName = Application.loadedLevelName;
		myData._iUser.levelName = saveLevelName; 
		
		//Latest Time
		saveLatestTime = long.Parse (System.DateTime.Now.ToString ("yyyyMMddHHmmss"));
		myData._iUser.latestTime = saveLatestTime; 
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}

	//Autosave function
	public void Quicksave()
	{
		//Show Save Icon
		AdventureInterface.showSaveIcon = true;

		_FileName="Quicksave.xml"; 
		
		//Save Position
		characterManager.SavePlayerPosition ();
		myData._iUser.x = savePosition.x; 
		myData._iUser.y = savePosition.y; 
		myData._iUser.z = savePosition.z;  
		
		//Time
		saveTime = System.DateTime.Now.ToString("HH:mm:ss tt");
		myData._iUser.time = saveTime; 
		print (saveTime);
		
		//Date
		saveDate = System.DateTime.Now.ToString("dd/MM/yyyy");
		myData._iUser.date = saveDate;
		print (saveDate);
		
		//Play Time
		savePlayTime += Time.realtimeSinceStartup;
		myData._iUser.playTime = savePlayTime; 
		
		//Level Name
		saveLevelName = Application.loadedLevelName;
		myData._iUser.levelName = saveLevelName; 
		
		//Latest Time
		saveLatestTime = long.Parse (System.DateTime.Now.ToString ("yyyyMMddHHmmss"));
		myData._iUser.latestTime = saveLatestTime; 
		
		// Time to creat our XML! 
		_data = SerializeObject(myData); 
		// This is the final resulting XML from the serialization process 
		CreateXML(); 
		//Debug.Log(_data); 
	}

	//Load Function called from start of scene to Initialise level/ game
	public void Quickload()
	{
		_FileName="Quicksave.xml"; 
		// Load our UserData into myData 
		QuickLoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			//Load the save position
			savePosition = new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);
			
			//Load the Time
			saveTime = myData._iUser.time;
			
			//Load the Date
			saveDate = myData._iUser.date;
			
			//Load the Play Time
			savePlayTime = myData._iUser.playTime;
			
			//Load Level Name
			saveLevelName = myData._iUser.levelName;
			
			//Load Latest Time
			saveLatestTime = myData._iUser.latestTime;
			
			// just a way to show that we loaded in ok 
			//Debug.Log(myData._iUser.x); 
		} 
	}

	//Load Function called from start of scene to Initialise level/ game
	public void Autoload()
	{
		_FileName="Autosave.xml"; 
		// Load our UserData into myData 
		QuickLoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			
			//Load the save position
			savePosition = new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);
			
			//Load the Time
			saveTime = myData._iUser.time;
			
			//Load the Date
			saveDate = myData._iUser.date;
			
			//Load the Play Time
			savePlayTime = myData._iUser.playTime;
			
			//Load Level Name
			saveLevelName = myData._iUser.levelName;
			
			//Load Latest Time
			saveLatestTime = myData._iUser.latestTime;
			
			// just a way to show that we loaded in ok 
			//Debug.Log(myData._iUser.x); 
		} 
	}

	/* The following metods came from the referenced URL */ 
	string UTF8ByteArrayToString(byte[] characters) 
	{      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 
	
	byte[] StringToUTF8ByteArray(string pXmlString) 
	{ 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 
	
	// Here we serialize our UserData object of myData 
	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return XmlizedString; 
	} 
	
	// Here we deserialize it back into its original form 
	object DeserializeObject(string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		//XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		return xs.Deserialize(memoryStream); 
	} 
	
	// Finally our save and load methods for the file itself 
	void CreateXML() 
	{ 
		StreamWriter writer; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		if(!t.Exists) 
		{ 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(_data); 
		writer.Close(); 
		Debug.Log("File written."); 
	} 

	void CreateNewXML() 
	{ 
		StreamWriter writer; 
		/*
		DirectoryInfo  di = new DirectoryInfo (_FileLocation);

		int numXML = di.GetFiles("*.xml", SearchOption.AllDirectories).Length;

		saveNumber = numXML + 1;
		*/
		saveNumber = 1;
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 

		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName);
		while(t.Exists)
		{
			saveNumber++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName);
		}

		writer = t.CreateText(); 

		writer.Write(_data); 
		writer.Close(); 

		Debug.Log("File written."); 
	} 
	
	void LoadXML() 
	{ 
		_FileLocation= Application.dataPath; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		DirectoryInfo  f = new DirectoryInfo (_FileLocation);		
		int numXML = f.GetFiles("*.xml", SearchOption.AllDirectories).Length;
		int currentSave = saveNumber;
		int search = 0;
		while (!t.Exists)	//If File does not exist, because it was deleted search next files
		{
			search++;
			saveNumber ++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName); 

			if(search > numXML)
			{
				saveNumber = currentSave - 1;
				_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
				t = new FileInfo(_FileLocation+"\\"+ _FileName); 

				for(int i = currentSave; i > 0 && !t.Exists; i--)
				{
					saveNumber --;
					_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
					t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				}

				if(!t.Exists)
				{
					Debug.Log ("Failed to find load file.... shit");
					break;
				}
			}
		}

		if(t.Exists)
		{
			StreamReader r = File.OpenText(_FileLocation+"\\"+_FileName);
			string _info = r.ReadToEnd(); 
			r.Close(); 
			_data=_info; 
			//Debug.Log("File Read"); 
		}
		else
		{
			_data = "";
		}
	} 

	void QuickLoadXML() 
	{ 
		_FileLocation= Application.dataPath; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 	
		
		if(t.Exists)
		{
			StreamReader r = File.OpenText(_FileLocation+"\\"+_FileName);
			string _info = r.ReadToEnd(); 
			r.Close(); 
			_data=_info; 
			//Debug.Log("File Read"); 
		}
		else
		{
			_data = "";
		}
	} 

	public void SetHover(bool _hover)
	{
		isHover = _hover;
	}

	public void DeleteSave()
	{
		_FileLocation= Application.dataPath; 
		_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		DirectoryInfo  f = new DirectoryInfo (_FileLocation);		
		int numXML = f.GetFiles("*.xml", SearchOption.AllDirectories).Length;
		int currentSave = saveNumber;
		int search = 0;
		while (!t.Exists)	//If File does not exist, because it was deleted search next files
		{
			search++;
			saveNumber ++;
			_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
			t = new FileInfo(_FileLocation+"\\"+ _FileName); 
			
			if(search > numXML)
			{
				saveNumber = currentSave - 1;
				_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
				t = new FileInfo(_FileLocation+"\\"+ _FileName); 

				for(int i = currentSave; i > 0 && !t.Exists; i--)
				{
					saveNumber --;
					_FileName="SaveGame"+saveNumber.ToString()+".xml"; 
					t = new FileInfo(_FileLocation+"\\"+ _FileName); 
				}
				
				if(!t.Exists)
				{
					saveNumber=currentSave;
					Debug.Log ("Failed to find Save File " +saveNumber+ ".... shit");
					break;
				}
			}
		}
		
		//Delete File
		if(t.Exists)
		{
			//Also delete Meta file
			FileInfo m = new FileInfo(_FileLocation+"\\SaveGame"+saveNumber.ToString()+".xml.meta"); 
			t.Delete(); 
			m.Delete ();
			Destroy (gameObject);
		}
	}
} 

// UserData is our custom class that holds our defined objects we want to store in XML format 
public class UserData 
{ 
	// We have to define a default instance of the structure 
	public DemoData _iUser; 
	// Default constructor doesn't really do anything at the moment 
	public UserData() { } 
	
	// Anything we want to store in the XML file, we define it here 
	public struct DemoData 
	{ 
		public float x; 
		public float y; 
		public float z; 
		public string time;
		public string date;
		public float playTime;
		public string levelName;
		public long latestTime;
	} 
}
