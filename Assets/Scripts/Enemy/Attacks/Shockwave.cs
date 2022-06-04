using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
	private EnemyControl EnemyC;
	
	private Transform PlayerTransf;
	
	private int hit_id;
	
	[Tooltip("Attack damage per hit")]
	[SerializeField]
    private int atk_dmg;
	[Tooltip("Attack duration in frames (24 FPS physics)")]
	[SerializeField]
	private int atk_duration;
	[Tooltip("Attack max range")]
	[SerializeField]
	private float atk_dist;
	
	[Tooltip("Attack vertical reach")]
	[SerializeField]
	private float atk_vReach;
	
	private void FixedUpdate()
	{
		//timer
		if(atk_duration > 0)
		{
			//alcance
			if(Vector3.Distance(transform.position, PlayerTransf.position) < atk_dist)
			{
				//se o jogador está perto do chão
				RaycastHit hit;
				if (Physics.Raycast(PlayerTransf.position - (PlayerTransf.forward * 0.1f) + PlayerTransf.up * 0.3f, Vector3.down, out hit, 1000))
				{
					if (hit.distance < atk_vReach)
					{
						//dano
						PlayerHealth P_HP = PlayerTransf.GetComponent<PlayerHealth>();
						if (P_HP.hit_id != hit_id)
							P_HP.TakeDamage(atk_dmg);
					}
				}
			}
			
			atk_duration--;
		}
		//encerra o ataque
		else
			Destroy(gameObject);//mudar pra setactive
	}
}
