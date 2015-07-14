using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Script Objective: Async Load to the next specified level by the interlevel traveller (a game object from another level)

public class LoadingScreenTravel : MonoBehaviour 
{
	public Slider loadProgress;

	public GameObject[] images;

	// Use this for initialization
	void Start () 
	{
		images[Random.Range (0,images.Length)].SetActive (true);
		loadProgress.value = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void LoadTargetLevel(string levelToLoad)
	{
		StartCoroutine (DisplayLoadingScreen(levelToLoad));
		Application.backgroundLoadingPriority = ThreadPriority.High;
	}

	IEnumerator DisplayLoadingScreen(string level)
	{
		yield return new WaitForSeconds(1f);
		AsyncOperation async = Application.LoadLevelAsync (level);
		while(!async.isDone)
		{
			loadProgress.value = async.progress;
			yield return null;
		}
	}
}
