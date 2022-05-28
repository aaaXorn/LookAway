using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroRange : MonoBehaviour
{
	private EnemyControl[] EnemyC;
	
	[SerializeField]
	private Transform[] EnemyTransf;
	
	private PlayerControl PlayerC;
	private CamLock CL;
	private LockButton LB;
	
    private void Start()
    {
        //pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
		
		//pega o script de camera lock
		if(CamLock.Instance != null) CL = CamLock.Instance;
		else print("CameraLock Instance not found.");
		
		//pega o script do botão de camera lock
		if(LockButton.Instance != null) LB = LockButton.Instance;
		else print("LockButton Instance not found.");

		EnemyC = new EnemyControl[EnemyTransf.Length];
		for (int i = 0; i < EnemyTransf.Length; i++)
			EnemyC[i] = EnemyTransf[i].GetComponent<EnemyControl>();
    }
	
	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			//adiciona os alvos pro camera lock
			CL.AddTargets(EnemyTransf);
			//muda a cor do botão
			LB.SetColor(true);
			
			for (int i = 0; i < EnemyTransf.Length; i++)
				EnemyC[i].Activate();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			//tira os alvos do camera lock
			CL.ResetLock();
			//muda a cor do botão
			LB.SetColor(false);
			
			for (int i = 0; i < EnemyTransf.Length; i++)
				EnemyC[i].Deactivate();
		}
	}
}
