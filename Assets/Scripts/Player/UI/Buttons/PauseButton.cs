using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour, IPointerDownHandler
{
    private PauseMenu PMenu;

    private void Start()
    {
        //pega o script de controle do menu de pause
        if (PauseMenu.Instance != null) PMenu = PauseMenu.Instance;
        else print("PauseMenu Instance not found.");
    }

    //quando o jogador toca no botão
    public void OnPointerDown(PointerEventData eventData)
    {
        PMenu.Pause();
    }
}
