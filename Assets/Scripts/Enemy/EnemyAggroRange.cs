using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroRange : MonoBehaviour
{
	private EnemyControl[] EnemyC;
	
	[SerializeField]
	private Transform[] EnemyTransf;
	
    private void Start()
    {
		EnemyC = new EnemyControl[EnemyTransf.Length];
		for (int i = 0; i < EnemyTransf.Length; i++)
			EnemyC[i] = EnemyTransf[i].GetComponent<EnemyControl>();
    }
	
	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			//adiciona os alvos pro camera lock
			CamLock.Instance.AddTargets(EnemyTransf);
			//muda a cor do botão
			LockButton.Instance.SetColor(true);
			
			for (int i = 0; i < EnemyTransf.Length; i++)
				EnemyC[i].Activate();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			//tira os alvos do camera lock
			CamLock.Instance.ResetLock();
			//muda a cor do botão
			LockButton.Instance.SetColor(false);
			
			for (int i = 0; i < EnemyTransf.Length; i++)
				EnemyC[i].Deactivate();
		}
	}
}
