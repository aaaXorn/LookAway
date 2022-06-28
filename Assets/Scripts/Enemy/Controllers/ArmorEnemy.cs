using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorEnemy : EnemyControl
{
	private bool move_back;

	protected override void OnStart()
    {
		InvokeRepeating("CheckDist", 1f, 1f);
    }

	private void CheckDist()
    {
		Vector3 go_to = PlayerTransf.position - transform.position;
		//distância entre o inimigo e o player
		float dist = go_to.magnitude;

		move_back = (dist <= max_approach_range);
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

		if(move_back)
			//movimento
			Control.SimpleMove(-dir * base_speed);
		else
			//movimento
			Control.SimpleMove(dir * base_speed);

		//continua se movendo
		if (atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//distância entre o inimigo e o player
			float dist = go_to.magnitude;
			//ranged
			if (dist <= ranged_atk_range)
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

	protected override void AtkTypeSwitch(GameObject obj)
	{
		switch (atk_type)
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
