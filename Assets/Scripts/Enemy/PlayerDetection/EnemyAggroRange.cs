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
	
	public void AggroStart()
	{
		//adiciona os alvos pro camera lock
		CamLock.Instance.AddTargets(EnemyTransf);
		//muda a cor do botão
		LockButton.Instance.SetColor(true);
		
		for (int i = 0; i < EnemyTransf.Length; i++)
			EnemyC[i].Activate();
	}
	
	public void AggroEnd()
	{
		//tira os alvos do camera lock
		CamLock.Instance.ResetLock();
		//muda a cor do botão
		LockButton.Instance.SetColor(false);
		
		for (int i = 0; i < EnemyTransf.Length; i++)
			EnemyC[i].Deactivate();
	}
}
