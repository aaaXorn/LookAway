using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
settings de PlayerPrefs:
GameQuality (int): qualidade gráfica
OldPlayerPosition (Vector3): última posição do player na Scene Land
*/

public class SaveSystem
{
	//salva as configs do player
	public static void SaveConfig(int quality)
	{
		PlayerPrefs.SetInt("GameQuality", quality);
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
}
