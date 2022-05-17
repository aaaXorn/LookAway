using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroRange : MonoBehaviour
{
	private EnemyControl EnemyC;
	
	[SerializeField]
	private Transform[] EnemyTransf;
	
	private PlayerControl PlayerC;
	private CamLock CL;
	
    private void Start()
    {
        //pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
		
		//pega o script de camera lock
		if(CamLock.Instance != null) CL = CamLock.Instance;
		else print("CameraLock Instance not found.");
    }
	
	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			CL.LockOn(EnemyTransf);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			CL.LockOff();
		}
	}
}
