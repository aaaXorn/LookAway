using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleControl : EnemyControl
{
    protected override void StateApproach()
	{
		//espera
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			Vector3 go_to = PlayerTransf.position - transform.position;
			
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
	
	public void Hurt(int hp, int max_hp)
	{
		switch(pattern)
		{
			case 0:
				if(hp < max_hp * 3 / 4)
				{
					pattern++;
					currAtk = 1;
				}
				break;
			
			case 1:
				if(hp < max_hp / 2)
				{
					pattern++;
					currAtk = 1;
				}
				break;
			
			case 2:
				if(hp < max_hp / 3)
				{
					pattern++;
					currAtk = 1;
				}
				break;
			
			default:
				break;
		}
	}
	
	protected override void PostAttackState()
	{
		currentState = State.Approach;
	}
}
