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
			transform.Translate(transform.forward * l_atk.spd);
		}
		else
			Reset();
	}
	
	private void Reset()
	{
		duration = 0;
		
		gameObject.SetActive(false);
	}
}
