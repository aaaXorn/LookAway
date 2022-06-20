using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoE_Spikes : MonoBehaviour
{
	[SerializeField]
	//gameobjs dos espinhos
	private GameObject[] obj_Spikes;

	private int startup;
	private int duration;
	[SerializeField]
	private int total_startup;
	[SerializeField]
	private int total_duration;
	[SerializeField]
	//ate onde o obj sobe
	private int height;

	//número de espinhos
	private int number;

	private void Start()
    {
		number = obj_Spikes.Length;

		duration = total_duration + 1;
		startup = total_startup;
		gameObject.SetActive(false);
	}

	private void FixedUpdate()
    {
		if (startup > 0)
		{
			startup--;
		}
		else if (duration > 0)
		{
			for (int i = 0; i < number; i++)
			{
				obj_Spikes[i].transform.Translate(obj_Spikes[i].transform.up * height / total_duration);
			}

			duration--;
		}
		else
			Reset();
	}

	private void Reset()
    {
		duration = total_duration + 1;
		startup = total_startup;

		for (int i = 0; i < number; i++)
		{
			obj_Spikes[i].transform.Translate(obj_Spikes[i].transform.up * -height);
		}

		gameObject.SetActive(false);
	}
}
