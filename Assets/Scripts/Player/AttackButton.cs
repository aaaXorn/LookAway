using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler
{
	//referência global do código do botão
	public static AttackButton Instance {get; set;}
	
	//referência do código do player
	private PlayerControl PlayerC;
	
	void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
	
    void Start()
    {
        //pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
    }

    //quando o jogador toca no botão
    public void OnPointerDown(PointerEventData eventData)
	{
		PlayerC.attackbtndown = true;
	}
}
