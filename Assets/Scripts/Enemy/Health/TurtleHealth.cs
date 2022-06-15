using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleHealth : EnemyHealth
{
    private TurtleControl TurtleC;
	
	protected override void Start()
	{
		EnemyC = GetComponent<EnemyControl>();
		TurtleC = GetComponent<TurtleControl>();
		
		hp = max_hp;
	}
	
	public override void TakeDamage(int dmg)
	{
		//não toma dano se inativo ou resetando
		if(EnemyC.currentState == EnemyControl.State.Inactive ||
		   EnemyC.currentState == EnemyControl.State.Reset ||
		   EnemyC.currentState == EnemyControl.State.Dead)
			return;
		
		hp -= dmg;
		TurtleC.Hurt(hp, max_hp);
		
		if(hp > max_hp) hp = max_hp;
		else if(hp <= 0)
		{
			EnemyC.Dead();
		}
	}
}
