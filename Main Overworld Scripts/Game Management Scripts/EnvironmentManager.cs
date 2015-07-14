using UnityEngine;
using System.Collections;

//Script Objective: Manages Environment

public class EnvironmentManager : MonoBehaviour 
{
	public static int waterQuality;

	void Awake ()
	{

	}

	// Use this for initialization
	void Start () 
	{
		UpdateEnvironmentQuality();

		//Warm Up all Shaders To Prevent future hiccups
		Shader.WarmupAllShaders ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Pause.isPaused && Input.GetButtonDown ("Cancel"))
		{
			UpdateEnvironmentQuality ();
		}
	}

	void UpdateEnvironmentQuality()
	{
		//Shadow Quality: This will change the Unity Quality Settings So this must be Applied before everything else
		//Potato = 0, Low = 1, Med = 2, High = 3, Very High = 4, Ultra = 5
		int shadowQuality = PlayerPrefs.GetInt ("ShadowQuality");
		QualitySettings.SetQualityLevel (shadowQuality, true); 

		if(shadowQuality > 1)
		{
			Terrain.activeTerrain.castShadows = true;
		}
		else
		{
			Terrain.activeTerrain.castShadows = false;
		}

		//Mesh Quality: Maximum LOD - 2 = LOW, 1 = Medium, 0 = High
		QualitySettings.maximumLODLevel = PlayerPrefs.GetInt ("MeshQuality");
		
		//Level of Detail: Low = 150, Medium = 500, High = 650, Ultra = 1000
		Shader.globalMaximumLOD = PlayerPrefs.GetInt ("LevelOfDetail");
		
		//Texture Quality: Low = 2, Med = 1, High = 0
		QualitySettings.masterTextureLimit = PlayerPrefs.GetInt ("TextureQuality");
		
		//Terrain Quality: Editing the Active Terrain Controls - Low = 0, Med = 1, High = 2, Ultra = 3
		int terrainQuality = PlayerPrefs.GetInt ("TerrainQuality");

		switch(terrainQuality)
		{
		case 3:
			Terrain.activeTerrain.basemapDistance = 2000f;
			Terrain.activeTerrain.heightmapMaximumLOD = 0;
			break;
		case 2:
			Terrain.activeTerrain.basemapDistance = 1500f;
			Terrain.activeTerrain.heightmapMaximumLOD = 0;
			break;
		case 1:
			Terrain.activeTerrain.basemapDistance = 1000f;
			Terrain.activeTerrain.heightmapMaximumLOD = 1;
			break;
		case 0:
			Terrain.activeTerrain.basemapDistance = 200f;
			Terrain.activeTerrain.heightmapMaximumLOD = 2;
			break;
		}
		
		//Vegetation Quality: Affect Terrain details, trees and soft vegetation
		//Low = 0, Med = 1, High = 2, Ultra = 3
		int vegetationQuality = PlayerPrefs.GetInt ("VegetationQuality");

		switch(vegetationQuality)
		{
		case 0:
			QualitySettings.softVegetation = false;
			Terrain.activeTerrain.detailObjectDensity = 0.25f;
			Terrain.activeTerrain.detailObjectDistance = 50f;
			Terrain.activeTerrain.treeDistance = 500f;
			Terrain.activeTerrain.treeBillboardDistance = 100f;
			Terrain.activeTerrain.treeMaximumFullLODCount = 10;
			break;
		case 1:
			QualitySettings.softVegetation = false;
			Terrain.activeTerrain.detailObjectDensity = 0.5f;
			Terrain.activeTerrain.detailObjectDistance = 150f;
			Terrain.activeTerrain.treeDistance = 1000f;
			Terrain.activeTerrain.treeBillboardDistance = 250f;
			Terrain.activeTerrain.treeMaximumFullLODCount = 50;
			break;
		case 2:
			QualitySettings.softVegetation = true;
			Terrain.activeTerrain.detailObjectDensity = 0.75f;
			Terrain.activeTerrain.detailObjectDistance = 200f;
			Terrain.activeTerrain.treeDistance = 1500f;
			Terrain.activeTerrain.treeBillboardDistance = 500f;
			Terrain.activeTerrain.treeMaximumFullLODCount = 120;
			break;
		case 3:
			QualitySettings.softVegetation = true;
			Terrain.activeTerrain.detailObjectDensity = 1f;
			Terrain.activeTerrain.detailObjectDistance = 250f;
			Terrain.activeTerrain.treeDistance = 2000f;
			Terrain.activeTerrain.treeBillboardDistance = 800f;
			Terrain.activeTerrain.treeMaximumFullLODCount = 200;
			break;
		}
		
		//Water Quality 0 = Low, 1 = Med, 2 = High, 3 = Very High, 4 = Ultra
		waterQuality = PlayerPrefs.GetInt ("WaterQuality");
		
		//Particle Quality: 0 = Potato, 1 = Low, 2 = Med, 3 = high, 4 = very high, 5 = ultra
		int particleQuality = PlayerPrefs.GetInt ("ParticleQuality");	

		switch(particleQuality)
		{
		case 0:
			QualitySettings.particleRaycastBudget = 4;
			break;
		case 1:
			QualitySettings.particleRaycastBudget = 16;
			break;
		case 2:
			QualitySettings.particleRaycastBudget = 64;
			break;
		case 3:
			QualitySettings.particleRaycastBudget = 256;
			break;
		case 4:
			QualitySettings.particleRaycastBudget = 1024;
			break;
		case 5:
			QualitySettings.particleRaycastBudget = 4096;
			break;
		}
		
		//Multisample Quality: x0 , x2, x4, x8
		QualitySettings.antiAliasing = PlayerPrefs.GetInt ("MSAA");	
	}
}
