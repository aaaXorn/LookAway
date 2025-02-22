using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//IPointerDownHandler e IPointerUpHandler fazem as funções OnPointer funcionarem
public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	//referência do código do player
	private PlayerControl PlayerC;
	
	private void Start()
	{
		//pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
	}
	
	//quando o jogador toca no botão
    public void OnPointerDown(PointerEventData eventData)
	{
		PlayerC.JumpDown();
	}
	
	//quando o jogador solta o botão
	public void OnPointerUp(PointerEventData eventData)
    {
		PlayerC.JumpUp();
	}
}
