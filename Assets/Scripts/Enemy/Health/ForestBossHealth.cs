using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBossHealth : EnemyHealth
{
	private ForestBossControl ForestC;
	
	protected override void Start()
	{
		EnemyC = GetComponent<EnemyControl>();
		ForestC = GetComponent<ForestBossControl>();
		
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
		ForestC.Hurt(hp, max_hp);
		
		if(hp > max_hp) hp = max_hp;
		else if(hp <= 0)
		{
			EnemyC.Dead();
		}
	}
}
