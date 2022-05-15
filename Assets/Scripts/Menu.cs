using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	//qualidade gráfica
	private int quality_level;
	//sensividade da camera
	private int cam_sensivity;
	
	//painéis do menu
	[SerializeField]
	private GameObject MainPanel, OptionsPanel;
	//configurações
	[SerializeField]
	private Dropdown Dd_Graphics;
	[SerializeField]
	private Slider Sld_CamSensivity;
		//texto que mostra quanto % de sensibilidade a camera tem
		[SerializeField]
		private Text Sld_CSens_txt;
	
    private void Start()
    {
        if(PlayerPrefs.HasKey("GameQuality"))
		{
			quality_level = PlayerPrefs.GetInt("GameQuality");
			
			Dd_Graphics.value = quality_level;
		}
		else quality_level = 0;
		
		if(PlayerPrefs.HasKey("CameraSensivity"))
		{
			cam_sensivity = PlayerPrefs.GetInt("CameraSensivity");
			
			Sld_CamSensivity.value = (float)cam_sensivity;
			Sld_CSens_txt.text = cam_sensivity + "%";
		}
		else cam_sensivity = 100;
		
		//desativa o painel de opções
		OptionsPanel.SetActive(false);
    }
	
	#region main menu
    public void StartGame()
    {
		SaveSystem.SaveConfig(quality_level, cam_sensivity);
		
        SceneManager.LoadScene("Land");
    }
	
	//usado pra abrir/fechar o menu de opções
    public void Options()
    {
		//troca o painel principal e o de opções
		MainPanel.SetActive(!MainPanel.activeSelf);
		OptionsPanel.SetActive(!OptionsPanel.activeSelf);
    }

    public void Quit()
    {
		SaveSystem.SaveConfig(quality_level, cam_sensivity);
		
        Application.Quit();
    }
	#endregion
	
	#region option menu
	//qualidade gráfica
	public void Graphics(int qual)
	{
		quality_level = qual;
	}
	
	//sensibilidade da camera
	public void CamSensivity(float sensv)
	{
		cam_sensivity = (int)sensv;
		
		Sld_CSens_txt.text = cam_sensivity + "%";
	}
	
	public void Volume()
	{
		
	}
	#endregion
}
