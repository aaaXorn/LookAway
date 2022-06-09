using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_AggroStart : MonoBehaviour
{
    private EnemyAggroRange EARange;
	
    private void Start()
    {
        EARange = GetComponentInParent<EnemyAggroRange>();
    }
	
	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Player"))
		{
			EARange.AggroStart();
		}
	}
}
