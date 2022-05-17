using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetButton : MonoBehaviour, IPointerDownHandler
{
    //referência global do código do botão
    public static TargetButton Instance { get; set; }

    //referência do código do camera Lock
    private CamLock CL;

    void Awake()
    {
        //setta a referência global desse script
        if (Instance == null) Instance = this;
        //garante que só tem um dele na cena
        else Destroy(gameObject);
    }

    void Start()
    {
        //pega o script de camera lock
        if (CamLock.Instance != null) CL = CamLock.Instance;
        else print("CamLock Instance not found.");
    }

    //quando o jogador toca no botão
    public void OnPointerDown(PointerEventData eventData)
    {
        CL.NextTarget();
    }
}