using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_AggroEnd : MonoBehaviour
{
    private EnemyAggroRange EARange;
	
    private void Start()
    {
        EARange = GetComponentInParent<EnemyAggroRange>();
    }
	
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			EARange.AggroEnd();
		}
	}
}
