using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class CameraDyJoystick : Joystick
{
	//referência global do código da rotação da camera
	public static CameraDyJoystick Instance {get; set;}
	
    //protected Vector2 input = Vector2.zero;
	
	//public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
	
    //[SerializeField]
	//private float moveThreshold = 1;
	
	//referência do cinemachine
	private CinemachineFreeLook cine;

	//sensividade da camera
	public float sensivity;
	private float sens_mod = 3f;
	
	private void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
	
    protected override void Start()
    {
        //MoveThreshold = moveThreshold;
        base.Start();
        background.gameObject.SetActive(false);
		
		//pega o componente de rect transform
		RectTransform rect = GetComponent<RectTransform>();
		//pega a escala do canvas, usado depois pra sempre cobrir a tela inteira
		float canvas_scale = transform.parent.GetComponent<RectTransform>().localScale.x;
		
		//muda a largura e altura do rect transform
		//para poder ser usado em qualquer parte da tela
		rect.sizeDelta = new Vector2(Screen.width / canvas_scale, Screen.height / canvas_scale);
		
		//encontra e pega o componente do cinemachine
		cine = GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineFreeLook>();
    
		#if UNITY_ENGINE
		sens_mod = 0.5f;
		#endif
	}
	
	private void Update()
	{
		//rotaciona a camera com base no input
		if(Horizontal != 0 || Vertical != 0)
		{
			cine.m_XAxis.Value += Horizontal * 1.5f * sensivity * sens_mod;
			cine.m_YAxis.Value += -Vertical / 30 * sensivity * sens_mod;
		}
	}
	
    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }

    /*protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }*/
}
