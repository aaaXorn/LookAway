using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroRange : MonoBehaviour
{
    [SerializeField]
	private EnemyControl EnemyC;
	
	private Transform EnemyTransf;
	
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
		
		EnemyTransf = EnemyC.transform;
    }
	
	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			CL.AddLockedTarget(EnemyTransf);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			CL.NullLockedTarget();
		}
	}
}
