using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	private EnemyControl EnemyC;
	
    [SerializeField]
	private int max_hp;//vida máxima
	private int hp;//vida atual
	
	public int hit_id;
	
	private void Start()
	{
		EnemyC = GetComponent<EnemyControl>();
		
		hp = max_hp;
	}
	
    public void TakeDamage(int dmg)
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
	
	public void ResetHP()
	{
		hp = max_hp;
	}
}
