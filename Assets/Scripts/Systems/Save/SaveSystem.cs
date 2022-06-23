using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
settings de PlayerPrefs:
GameQuality (int): qualidade gráfica
CameraSensivity (int): sensividade da camera
DynamicQuality (bool): se o jogo tem ou não qualidade dinâmica
OldPlayerPosition (Vector3): última posição do player na Scene Land
SwordLevel, ArmorLevel, ShieldLevel (int): níveis de equipamento
CurrPotions (int): poções que o jogador tem atualmente
MaxPotions (int): número máximo de poções que o jogador tem
*/

public class SaveSystem
{
	//próxima scene
	public static string NextScene;
	
	//salva as configs do player
	public static void SaveConfig(int quality, int sensivity, bool dynamic_qual)
	{
		PlayerPrefs.SetInt("GameQuality", quality);
		PlayerPrefs.SetInt("CameraSensivity", sensivity);
		PlayerPrefsX.SetBool("DynamicQuality", dynamic_qual);
	}
	
	//salva os dados do player
    public static void SaveGame()
	{
		if (SceneManager.GetActiveScene().name.Equals("Land") && PlayerControl.Instance != null)
        {
			PlayerPrefsX.SetVector3("OldPlayerPosition", PlayerControl.Instance.transform.position);
		}
		
		SavePlayerStats();
	}
	
	//salva somente os stats do player
	public static void SavePlayerStats()
	{
		//equipamentos
		PlayerPrefs.SetInt("SwordLevel", PlayerEquipment.Instance.sword_lvl);
		PlayerPrefs.SetInt("ArmorLevel", PlayerEquipment.Instance.armor_lvl);
		PlayerPrefs.SetInt("ShieldLevel", PlayerEquipment.Instance.shield_lvl);
		//poção
		PlayerPrefs.SetInt("MaxPotions", PlayerEquipment.Instance.max_potions);
		PlayerPrefs.SetInt("CurrPotions", PlayerEquipment.Instance.potions);
		//xp
		PlayerPrefs.SetInt("SwordXP", PlayerEquipment.Instance.sword_xp);
		PlayerPrefs.SetInt("ArmorXP", PlayerEquipment.Instance.armor_xp);
		PlayerPrefs.SetInt("ShieldXP", PlayerEquipment.Instance.shield_xp);
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
				
				//carrega o multiplicador de sensibilidade da camera
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

		LoadPlayerStats();
	}

	//carrega somente os stats do player
	public static void LoadPlayerStats()
    {
		//equipamento
		if (PlayerPrefs.HasKey("SwordLevel"))
		{
			PlayerEquipment.Instance.sword_lvl = PlayerPrefs.GetInt("SwordLevel");
		}
		if (PlayerPrefs.HasKey("ArmorLevel"))
		{
			PlayerEquipment.Instance.armor_lvl = PlayerPrefs.GetInt("ArmorLevel");
		}
		if (PlayerPrefs.HasKey("ShieldLevel"))
		{
			PlayerEquipment.Instance.shield_lvl = PlayerPrefs.GetInt("ShieldLevel");
		}
		
		//atualiza os stats
		PlayerEquipment.Instance.SwordStats();
		PlayerEquipment.Instance.ArmorStats();
		
		//poção
		if (PlayerPrefs.HasKey("MaxPotions"))
		{
			if (PlayerPrefs.GetInt("MaxPotion") < 4)
				PlayerPrefs.SetInt("MaxPotions", 4);
			PlayerEquipment.Instance.max_potions = PlayerPrefs.GetInt("MaxPotions");
		}
		if (PlayerPrefs.HasKey("CurrPotions"))
		{
			if (PlayerPrefs.GetInt("CurrPotions") < 0)
				PlayerPrefs.SetInt("CurrPotions", PlayerEquipment.Instance.max_potions);
			 PlayerEquipment.Instance.potions = PlayerPrefs.GetInt("CurrPotions");
		}

		//xp
		if (PlayerPrefs.HasKey("SwordXP"))
		{
			PlayerEquipment.Instance.sword_xp = PlayerPrefs.GetInt("SwordXP");
		}
		if (PlayerPrefs.HasKey("ArmorXP"))
		{
			PlayerEquipment.Instance.armor_xp = PlayerPrefs.GetInt("ArmorXP");
		}
		if (PlayerPrefs.HasKey("ShieldXP"))
		{
			PlayerEquipment.Instance.shield_xp = PlayerPrefs.GetInt("ShieldXP");
		}
	}
}
