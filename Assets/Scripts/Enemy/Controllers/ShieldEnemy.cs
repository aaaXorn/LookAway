using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : EnemyControl
{
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
		{
			//movimento
			Control.SimpleMove(dir * base_speed);
		}
		
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
				AnimHit(0);
				
				currentState = State.Attack;
			}
			else if(dist <= ranged_atk_range)
			{
				AnimHit(1);
				
				currentState = State.Attack;
			}
		}
	}
	
    protected override void PostAttackState()
	{
		currentState = State.Approach;
	}
	
	public override void Dead()
	{
		anim.SetTrigger("Dead");
			
		currentState = State.Dead;
		
		PlayerEquipment.Instance.ShieldXP(1);
	}
}
