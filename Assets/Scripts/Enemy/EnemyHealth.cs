using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
	private int max_hp;//vida máxima
	private int hp;//vida atual
	
	public int hit_id;
	
	private void Start()
	{
		hp = max_hp;
	}
	
    public void TakeDamage(int dmg)
	{
		hp -= dmg;
		
		if(hp > max_hp) hp = max_hp;
		else if(hp <= 0) print("morreu");
	}
	
	public void ResetHP()
	{
		hp = max_hp;
	}
}
