using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
	private LaserSO l_atk;
	
	private int duration;
	
	private void Start()
	{
		Reset();
	}
	
	private void FixedUpdate()
	{
		if(duration > 0)
		{
			transform.Translate(Vector3.forward * l_atk.spd, Space.Self);
			duration--;
		}
		else
			Reset();
	}
	
	private void Reset()
	{
		duration = l_atk.duration;
		
		gameObject.SetActive(false);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			PlayerHealth P_HP = other.gameObject.GetComponent<PlayerHealth>();
			if(P_HP != null)
				P_HP.TakeDamage(l_atk.dmg);
		}
	}
}
