using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//IPointerDownHandler e IPointerUpHandler fazem as funções OnPointer funcionarem
public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	//referência global do código do botão
	public static JumpButton Instance {get; set;}
	
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
		PlayerC.jumpbtn = true;
		PlayerC.jumpbtndown = true;
	}
	
	//quando o jogador solta o botão
	public void OnPointerUp(PointerEventData eventData)
    {
		PlayerC.jumpbtn = false;
		PlayerC.jumptime = 0;
	}
}
