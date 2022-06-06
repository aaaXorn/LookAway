using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	protected EnemyControl EnemyC;
	
    [SerializeField]
	protected int max_hp;//vida máxima
	protected int hp;//vida atual
	
	public int hit_id;
	
	protected void Start()
	{
		EnemyC = GetComponent<EnemyControl>();
		
		hp = max_hp;
	}
	
	protected virtual void OnStart()
	{
		
	}
	
    public virtual void TakeDamage(int dmg)
	{
		//não toma dano se inativo ou resetando
		if(EnemyC.currentState == EnemyControl.State.Inactive || EnemyC.currentState == EnemyControl.State.Reset)
			return;
		
		hp -= dmg;
		
		if(hp > max_hp) hp = max_hp;
		else if(hp <= 0)
		{
			EnemyC.currentState = EnemyControl.State.Dead;
		}
	}
	
	public virtual void ResetHP()
	{
		hp = max_hp;
	}
}
