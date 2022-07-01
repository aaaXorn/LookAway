using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LockButton : MonoBehaviour, IPointerDownHandler
{
	//referência global do código do botão
	public static LockButton Instance {get; set;}
	
	[Tooltip("ta como txt pq eu tava com preguica de mudar")]
	[SerializeField]
	private Image txt;
	[SerializeField]
	private Color color_off, color_on;
	
    //referência do código do camera Lock
    private CamLock CL;
	
	void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
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
        CL.EnableLock();
    }
	
	public void SetColor(bool on)
	{
		if(txt == null) txt = GetComponent<Image>();
		
		if(on)
			txt.color = color_on;
		else
			txt.color = color_off;
	}
}