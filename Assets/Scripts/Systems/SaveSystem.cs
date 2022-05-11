using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
settings de PlayerPrefs:
GameQuality (int): qualidade gráfica
CameraSensivity (int): sensividade da camera
OldPlayerPosition (Vector3): última posição do player na Scene Land
*/

public class SaveSystem
{
	//salva as configs do player
	public static void SaveConfig(int quality, int sensivity)
	{
		PlayerPrefs.SetInt("GameQuality", quality);
		PlayerPrefs.SetInt("CameraSensivity", sensivity);
	}
	
	//salva os dados do player
    public static void SaveGame()
	{
		if (SceneManager.GetActiveScene().name.Equals("Land") && PlayerControl.Instance != null)
        {
			PlayerPrefsX.SetVector3("OldPlayerPosition", PlayerControl.Instance.transform.position);
		}
		
		//stats espada, armadura, escudo
	}
	
	//carrega as configs
	public static void LoadConfig()
	{
		//config qualidade gráfica
		if(PlayerPrefs.HasKey("GameQuality"))
		{
			int qual = PlayerPrefs.GetInt("GameQuality");
			
			QualitySettings.SetQualityLevel(qual);
			
			if (SceneManager.GetActiveScene().name.Equals("Land"))
			{
				//script do terreno
				TerrainSettings TS = TerrainSettings.Instance;
				
				//config qualidade terreno
				TS.SetTerrain(qual);
			}
		}
		//pro valor default do terreno ser 0
		else
		{
			if (SceneManager.GetActiveScene().name.Equals("Land"))
			{
				//script do terreno
				TerrainSettings TS = TerrainSettings.Instance;
				
				//config qualidade terreno
				TS.SetTerrain(0);
			}
		}
		
		//config sensibilidade da camera
		if(PlayerPrefs.HasKey("CameraSensivity"))
		{
			if (!SceneManager.GetActiveScene().name.Equals("Menu"))
			{
				CameraDyJoystick CDJ = CameraDyJoystick.Instance;
				
				// 
				CDJ.sensivity = (float)
							    (PlayerPrefs.GetInt("CameraSensivity"))/100;
			}
		}
	}
	
	//carrega os dados do player
	public static void LoadGame()
	{
		//script do player
		PlayerControl PC = PlayerControl.Instance;
		
		//config posição no mapa
		if (SceneManager.GetActiveScene().name.Equals("Land"))
        {	
            if (PlayerPrefs.HasKey("OldPlayerPosition"))
            {
                Debug.Log("movendo "+ PlayerPrefsX.GetVector3("OldPlayerPosition"));
                PC.transform.position = PlayerPrefsX.GetVector3("OldPlayerPosition");
            }
        }
		//config stats de arma/armadur/escudo
	}
}
