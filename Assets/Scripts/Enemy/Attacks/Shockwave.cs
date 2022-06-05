using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
	private Transform PlayerTransf;
	
	[SerializeField]
	private ShockwaveSO s_atk;
	
	//duração
	private int atk_duration;
	//distância máxima/mínima
	private float atk_max_dist, atk_min_dist;
	
	//se acertou/errou
	private bool has_hit;
	
	private void Start()
	{
		PlayerTransf = PlayerControl.Instance.transform;
		
		Reset();
	}
	
	private void FixedUpdate()
	{
		//timer
		if(atk_duration > 0)
		{
			//alcance
			if(!has_hit)
			{
				float dist = Vector3.Distance(transform.position, PlayerTransf.position);
				
				if(dist < atk_max_dist && dist > atk_min_dist)
				{
					//se o jogador está perto do chão
					RaycastHit hit;
					if (Physics.Raycast(PlayerTransf.position - (PlayerTransf.forward * 0.1f) + PlayerTransf.up * 0.3f, Vector3.down, out hit, 1000)
						&& hit.distance < s_atk.vReach)
					{
						//dano
						PlayerHealth P_HP = PlayerTransf.GetComponent<PlayerHealth>();
						P_HP.TakeDamage(s_atk.dmg);
						
						has_hit = true;
					}
				}
			}
			
			//movimento da shockwave
			if(atk_min_dist > 0)
			{
				atk_max_dist += s_atk.spd;
				atk_min_dist += s_atk.spd;
			}
			
			atk_duration--;
		}
		//encerra o ataque
		else
			Reset();
	}
	
	//volta pro estado inicial
	private void Reset()
	{
		has_hit = false;
		
		atk_duration = s_atk.duration;
		atk_max_dist = s_atk.max_dist;
		atk_min_dist = s_atk.min_dist;
		
		gameObject.SetActive(false);
	}
}
