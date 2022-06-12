using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCol : MonoBehaviour
{
	public int dmg;
	
    private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			PlayerHealth P_HP = other.gameObject.GetComponent<PlayerHealth>();
			if(P_HP != null)
				P_HP.TakeDamage(dmg);
		}
	}
}
