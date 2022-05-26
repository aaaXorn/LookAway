using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //referência global do código do botão
    public static PauseMenu Instance { get; set; }

    //tempo fora do pause
    private float base_timeScale = 1.2f;

    [SerializeField]
    private GameObject MenuObj, DeathMenuObj;

    private void Awake()
    {
        //setta a referência global desse script
        if (Instance == null) Instance = this;
        //garante que só tem um dele na cena
        else Destroy(gameObject);

        Time.timeScale = base_timeScale;
    }

    private void Start()
    {
        //pega o componente de rect transform
        RectTransform rectP = MenuObj.GetComponent<RectTransform>();
		RectTransform rectD = DeathMenuObj.GetComponent<RectTransform>();
        //pega a escala do canvas, usado depois pra sempre cobrir a tela inteira
        float canvas_scale = transform.GetComponent<RectTransform>().localScale.x;

        //muda a largura e altura do rect transform
        //para poder ser usado em qualquer parte da tela
		Vector2 canvas_size = new Vector2(Screen.width / canvas_scale, Screen.height / canvas_scale);
        rectP.sizeDelta = canvas_size;
		rectD.sizeDelta = canvas_size;

        MenuObj.SetActive(false);
		DeathMenuObj.SetActive(false);
    }
	
	#region pause
    public void Pause()
    {
        //despausa
        if(MenuObj.activeSelf)
        {
            Time.timeScale = base_timeScale;

            MenuObj.SetActive(false);
        }
        //pausa
        else
        {
            Time.timeScale = 0f;

            MenuObj.SetActive(true);
        }
    }

    //vai pro menu e salva o jogo
    public void MainMenu()
    {
        SaveSystem.SaveGame();

        SaveSystem.NextScene = "Menu";
        SceneManager.LoadScene("LoadScene");
    }
	#endregion
	
	#region death
	//volta pro jogo
	public void TryAgain()
	{
		SaveSystem.SavePlayerStats();
		
		//talvez seja trocado por uma simples mudança de posição
		SaveSystem.NextScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene("LoadScene");
	}
	
	public void GameOverScreen()
	{
		Time.timeScale = 0f;
		
		MenuObj.SetActive(false);
		
		DeathMenuObj.SetActive(true);
	}
	#endregion
}
