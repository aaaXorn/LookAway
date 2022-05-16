using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamLock : MonoBehaviour
{
	//referência global do código da camera
	public static CamLock Instance {get; set;}
	
    //se a câmera está lockada num alvo
	public bool cam_lock;
	
	//alvo da camera
	private Transform cam_target;
	
	[SerializeField]
	//referência do cinemachine
	private CinemachineFreeLook cine_FL;
	[SerializeField]
	private CinemachineVirtualCamera cine_T;
	
	private void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
	
	//ativa o lock
	public void LockOn()
	{
		if(cam_target != null && !cam_lock)
		{
			cam_lock = true;
			
			cine_FL.gameObject.SetActive(false);
			cine_T.gameObject.SetActive(true);
			
			//define para onde o cinemachine de lock vai olhar
			cine_T.LookAt = cam_target;
		}
	}
	//desativa o lock
	public void LockOff()
	{
		if(cam_lock)
		{
			cam_lock = false;
			
			cine_FL.gameObject.SetActive(true);
			cine_T.gameObject.SetActive(false);
		}
	}
	
	//muda o alvo do lock
	public void AddLockedTarget(Transform enemy)
	{
		if(cam_target == null)
		{
			cam_target = enemy;
		}
		
		LockOn();
	}
	//tira o alvo do lock
	public void NullLockedTarget()
	{
		if(cam_target != null)
		{
			cam_target = null;
		}
		
		LockOff();
	}
}
