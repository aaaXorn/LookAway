using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorEnemy : EnemyControl
{
    protected virtual void StateApproach()
	{
		Vector3 go_to = PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		
		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		//movimento
		Control.SimpleMove(-dir * base_speed);
		
		//continua se movendo
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//distância entre o inimigo e o player
			float dist = go_to.magnitude;
			//ranged
			if(dist <= ranged_atk_range)
			{
				SpecialHit(0);
				
				currentState = State.Special;
			}
		}
	}
	
	public override void Dead()
	{
		anim.SetTrigger("Dead");
			
		currentState = State.Dead;
		
		PlayerEquipment.Instance.ArmorXP(1);
	}
}
