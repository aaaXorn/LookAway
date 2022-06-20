using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
	[SerializeField]
	private SpikesSO s_atk;
	
	//layer do player
    [SerializeField]
    private LayerMask ground_layer;
	
	[SerializeField]
	//prefab do espinho
	private GameObject P_Spike;
	//gameobjs dos espinhos
	private GameObject[] obj_Spikes;
	
	private int startup;
	private int duration;
	
	private void Awake()
	{
		obj_Spikes = new GameObject[s_atk.number];
		
		for(int i = 0; i < s_atk.number; i++)
		{
			GameObject spike = Instantiate(P_Spike, transform);
			obj_Spikes[i] = spike;
		}
		
		Reset();
	}
	
    private void FixedUpdate()
	{
		if(startup > 0)
		{
			startup--;
		}
		else if(duration > 0)
		{
			for(int i = 0; i < s_atk.number; i++)
			{
				obj_Spikes[i].transform.Translate(obj_Spikes[i].transform.up * s_atk.height / s_atk.duration);
			}
			
			duration--;
		}
		else
			Reset();
	}
	
	//quando o objeto é ativado
    private void OnEnable()
    {
		Vector3 Center = PlayerControl.Instance.transform.position + Vector3.up * 5;
		
		for(int i = 0; i < s_atk.number; i++)
		{
			Vector3 pos = Center + (Vector3.forward * Random.Range(-s_atk.radius, s_atk.radius)) +
								   (Vector3.right * Random.Range(-s_atk.radius, s_atk.radius));
			
			RaycastHit hit;
			if(Physics.Raycast(pos, -Vector3.up, out hit, 100f, ground_layer))
			{
				obj_Spikes[i].SetActive(true);
				
				obj_Spikes[i].transform.position = hit.point;
				obj_Spikes[i].transform.eulerAngles =
							new Vector3(Random.Range(-s_atk.max_rot_mod, s_atk.max_rot_mod), 
										Random.Range(-s_atk.max_rot_mod, s_atk.max_rot_mod),
										Random.Range(-s_atk.max_rot_mod, s_atk.max_rot_mod));
			}
		}
	}
	
	private void Reset()
	{
		startup = s_atk.startup;
		duration = s_atk.duration;
		
		for(int i = 0; i < s_atk.number; i++)
		{
			obj_Spikes[i].SetActive(false);
		}
		
		gameObject.SetActive(false);
	}
	
	private void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("Player") && startup <= 0)
		{
			PlayerHealth P_HP = other.gameObject.GetComponent<PlayerHealth>();
			if(P_HP != null)
				P_HP.TakeDamage(s_atk.dmg);
		}
	}
}
