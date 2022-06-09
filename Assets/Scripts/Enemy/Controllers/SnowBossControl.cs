using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBossControl : EnemyControl
{
	[SerializeField]
	private int thrown_cd_total;
	private int thrown_cd;
	
	protected override void OnFUpdate()
	{
		if(thrown_cd > 0) thrown_cd--;
	}
	
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
				//checa o padrão atual do boss
				if(pattern >= 5)
				{
					//jump attack
					AnimHit(1);
					currentState = State.Attack;
					
					pattern = 0;
				}
				else
				{
					//se a parte fracionaria de pattern / 2 = 0 (se for par)
					if(pattern % 2 == 0)
					{
						//swipe
						AnimHit(0);
						currentState = State.Attack;
					}
					//se for impar
					else
					{
						//shockwave
						SpecialHit(0);
						currentState = State.Special;
					}
					
					pattern++;
				}
			}
			//ranged
			else if(dist <= ranged_atk_range)
			{
				if(thrown_cd <= 0)
				{
					SpecialHit(1);
					currentState = State.Special;
					
					thrown_cd = thrown_cd_total;
				}
			}
		}
	}
	
	protected override void PostAttackState()
	{
		if(pattern == 0)
			currentState = State.Reposition;
		else
			currentState = State.Approach;
	}
}
