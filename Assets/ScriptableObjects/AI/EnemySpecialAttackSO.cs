using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialAttack",
				 menuName = "ScriptableObject/AI/SpecialAttack")]
public class EnemySpecialAttackSO : ScriptableObject
{
    [Tooltip("Type of special attack")]
	public string type;
	
	[Tooltip("How many hits the attack has")]
	public int hit_count;
	
	[Tooltip("Delay between the attacks")]
	public int[] delay;
	[Tooltip("When the attack ends if not canceled")]
	public int last_frame;
	[Tooltip("The cooldown of the attack")]
	public int cooldown;
	
	[Tooltip("Mid attack movement")]
	public float movement;
}
