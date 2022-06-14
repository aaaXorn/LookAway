using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBossControl : EnemyControl
{
	//velocidade do reposition
	[SerializeField]
	private float repos_spd;
	
	//quando a AI usa o padrão de reposição
	private int repos;
	
    protected override void StateApproach()
	{
		//vai para trás em vez de para frente
		Vector3 go_to = transform.position - PlayerTransf.position;//PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		
		//distância entre o inimigo e o player
		float dist = go_to.magnitude;
		
		if(dist < melee_atk_range)
		{
			//rotação
			Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
			rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);
			
			//movimento
			Control.SimpleMove(dir * base_speed);
		}
		else
		{
			go_to = PlayerTransf.position - transform.position;
			dir = go_to.normalized;
			
			//rotação
			Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
			rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);
			
			//movimento
			Control.SimpleMove(dir * base_speed);
		}
		
		//continua se movendo
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//AoE quando é atingido
			if(currSpAtk == 1)
			{
				AnimHit(0);
				currentState = State.Attack;
				
				currSpAtk = 0;
			}
			//ataques a distância
			else if(currSpAtk <= ranged_atk_range)
			{
				//espinho
				if(currAtk > 0)
				{
					SpecialHit(0);
					currentState = State.Special;
					
					currAtk = 0;
				}
				//pew pew
				else
				{
					SpecialHit(1);
					currentState = State.Special;
					
					currAtk++;
				}
			}
		}
	}
	
	protected override void PostAttackState()
	{
		currentState = State.Approach;
	}
	
	protected virtual void PostSpecialState()
	{
		if(repos < 2)
		{
			currentState = State.Approach;
			repos++;
		}
		else
		{
			RepositionStart();
			repos = 0;
		}
	}
	
	protected override void StateReposition()
	{
		//movimento
		Control.SimpleMove(move_dir * repos_spd);

		//rotação
		Quaternion rot = Quaternion.LookRotation(move_dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		//quando chega no ponto escolhido, para
		if (Vector3.Distance(transform.position, move_target) <= 1)
			currentState = State.Active;
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
				obj.transform.rotation = new Quaternion(0,
														obj.transform.rotation.y,
														obj.transform.rotation.z,
														obj.transform.rotation.w);
				break;
			
			case "Spikes":
				
				break;
			
			default:
				obj.transform.position = atk_origin[curr_hit].position;
				obj.transform.rotation = atk_origin[curr_hit].rotation;
				break;
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
					currSpAtk = 1;
				}
				break;
			
			case 1:
				if(hp < max_hp / 2)
				{
					pattern++;
					currSpAtk = 1;
				}
				break;
			
			case 2:
				if(hp < max_hp / 3)
				{
					pattern++;
					currSpAtk = 1;
				}
				break;
			
			default:
				break;
		}
	}
}
