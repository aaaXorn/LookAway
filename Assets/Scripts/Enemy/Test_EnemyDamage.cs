using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EnemyDamage : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			PlayerHealth P_HP = other.gameObject.GetComponent<PlayerHealth>();
			if(P_HP != null)
				P_HP.TakeDamage(1);
		}
	}
}
