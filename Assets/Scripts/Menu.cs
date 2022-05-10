using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
	//qualidade gráfica
	private int quality_level;
	
    private void Start()
    {
        //config qualidade gráfica
		if(PlayerPrefs.HasKey("GameQuality"))
		{
			int qual = PlayerPrefs.GetInt("GameQuality");
			
			QualitySettings.SetQualityLevel(qual);
		}
    }
	
    private void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Land");
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
	
	//salva as configs do player
	private void SaveConfig()
	{
		PlayerPrefs.SetInt("GameQuality", quality_level);
	}
}
