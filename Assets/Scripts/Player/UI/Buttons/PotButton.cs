using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotButton : MonoBehaviour, IPointerDownHandler
{
    //referência do código do player
    private PlayerControl PlayerC;

    void Start()
    {
        //pega o script de controle do jogador
        if (PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
        else print("PlayerControl Instance not found.");
    }

    //quando o jogador toca no botão
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerC.PotDown();
    }
}
