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
	
	//alvos da camera
	private List<Transform> cam_target = new List<Transform>();
	//alvo atual da camera
	private int curr_target;

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
	
	//muda a câmera pro próximo alvo
	public void NextTarget()
    {
		if (cam_lock && cam_target.Count > 1)
		{
			curr_target++;
			if (curr_target >= cam_target.Count) curr_target = 0;
			
			cine_T.LookAt = cam_target[curr_target];
		}
	}

	//ativa o lock
	public void LockOn(Transform[] enemies)
	{
		if(!cam_lock && cam_target.Count <= 0)
		{
			cam_lock = true;
			
			cine_FL.gameObject.SetActive(false);
			cine_T.gameObject.SetActive(true);
			
			foreach (Transform transf in enemies)
			{
				cam_target.Add(transf);
			}

			curr_target = 0;
			//define para onde o cinemachine de lock vai olhar
			cine_T.LookAt = cam_target[0];
		}
	}
	//desativa o lock
	public void LockOff()
	{
		if(cam_lock && cam_target.Count > 0)
		{
			cam_lock = false;

			cam_target.Clear();

			cine_FL.gameObject.SetActive(true);
			cine_T.gameObject.SetActive(false);
        }
    }

	//remove o alvo do array
    public void RemoveTarget(Transform transf)
    {
		cam_target.Remove(transf);
    }
}
