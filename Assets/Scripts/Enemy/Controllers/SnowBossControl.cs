using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBossControl : EnemyControl
{
	[SerializeField]
	float swipe_angle, swipe_range, min_ranged_range, jump_range;
	
	protected override void StateApproach()
	{
		Vector3 go_to = PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		
		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		if(go_to.magnitude > max_approach_range)
			//movimento
			Control.SimpleMove(dir * base_speed);
		
		//continua se movendo
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//distância entre o inimigo e o player
			float dist = go_to.magnitude;
			
			//melee
			if(dist <= melee_atk_range)
			{
				//ângulo entre o boss e o jogador
				float angle = Vector3.Angle(transform.forward, dir);
				print(angle);
				//se estiver na frente
				if(dist <= swipe_range && angle < swipe_angle)
				{
					//swipe
					AnimHit(0);
					currentState = State.Attack;
				}
				//se estiver do lado/atrás
				else
				{
					//shockwave
					SpecialHit(0);
					currentState = State.Special;
				}
			}
			//ranged
			else if(dist >= min_ranged_range && dist <= ranged_atk_range)
			{
				if(currSpAtk > 0 && dist <= jump_range)
				{
					AnimHit(1);
					currentState = State.Attack;
					
					currSpAtk = 0;
				}
				else
				{
					SpecialHit(1);
					currentState = State.Special;
					
					currSpAtk++;
				}
			}
		}
	}
	
	protected override void PostAttackState()
	{
		currentState = State.Approach;
	}
	
	protected override void AtkTypeSwitch(GameObject obj)
	{
		switch(atk_type)
		{
			case "Shockwave":
				obj.transform.position = atk_origin[curr_hit].position;
				obj.transform.rotation = atk_origin[curr_hit].rotation;
				break;

			case "Thrown":
				obj.transform.position = atk_origin[curr_hit].position;
				obj.transform.LookAt(PlayerControl.Instance.transform.position);
				obj.GetComponent<Thrown>().StartPos = atk_origin[curr_hit].position;
				break;
				
			case "Laser":
				obj.transform.position = atk_origin[curr_hit].position;
				obj.transform.LookAt(PlayerControl.Instance.transform.position);
				obj.transform.eulerAngles = new Vector3(4,
														obj.transform.eulerAngles.y,
														0);
				break;
			
			case "Spikes":
				
				break;
			
			default:
				obj.transform.position = atk_origin[curr_hit].position;
				obj.transform.rotation = atk_origin[curr_hit].rotation;
				break;
		}
	}
}
