using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleControl : EnemyControl
{
	[SerializeField]
	private int hurt_frames;
	private int hurt_f;
	
    protected override void StateApproach()
	{
		Vector3 go_to = PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		
		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);
		
		//espera
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//distância entre o inimigo e o player
			float dist = go_to.magnitude;
			
			if(currAtk > 0)
			{
				//speeeeeeeeeeeeeeeeeeeeeeeeeeeeeen
				AnimHit(1);
				currentState = State.Attack;
				
				currAtk = 0;
				return;
			}
			
			//melee
			if(dist <= melee_atk_range)
			{
				//speeeeeen
				AnimHit(0);
				currentState = State.Attack;
			}
			//ranged
			else if(dist <= ranged_atk_range)
			{
				if(currSpAtk > 0)
				{
					//erupcao
					SpecialHit(0);
					currentState = State.Special;
					
					currSpAtk = 0;
				}
				else
				{
					//shockwave
					SpecialHit(1);
					currentState = State.Special;
					
					currSpAtk++;
				}
			}
		}
	}
	
	protected override void StateSpecial()
	{
		if(attacking)
		{
			SpAttackEffect();
		}

		//encerra o ataque
		if (atk_last_frame <= 0)
		{
			attacking = false;
			atk_cancel = false;

			anim.SetTrigger("Free");

			PostSpecialState();
		}
		else atk_last_frame--;
	}
	
	public void Hurt(int hp, int max_hp)
	{
		switch(pattern)
		{
			case 0:
				if(hp < max_hp * 3 / 4)
				{
					pattern++;
					currAtk = 1;
					hurt_f = hurt_frames;
				}
				break;
			
			case 1:
				if(hp < max_hp / 2)
				{
					pattern++;
					currAtk = 1;
					hurt_f = hurt_frames;
				}
				break;
			
			case 2:
				if(hp < max_hp / 3)
				{
					pattern++;
					currAtk = 1;
					hurt_f = hurt_frames;
				}
				break;
			
			default:
				break;
		}
	}
	protected override void StateHurt()
	{
		if(hurt_f > 0) hurt_f--;
		else
		{
			anim.SetTrigger("Free");
			
			currentState = State.Approach;
		}
	}
	
	protected override void PostAttackState()
	{
		currentState = State.Approach;
	}
}
