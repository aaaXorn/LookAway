using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack",
				 menuName = "ScriptableObject/NormalAttack")]
public class NormalAttackSO : ScriptableObject
{
    [Tooltip("How many hits the attack has")]
	public int hit_count;
	
	[Tooltip("Attack damage per hit")]
	public int[] dmg;
	[Tooltip("Hitbox duration in frames (24 FPS physics)")]
	public int[] duration;
	[Tooltip("Hitbox radius")]
	public float[] size;
	[Tooltip("Hitbox length, 0 makes it a sphere")]
	public float[] length;
	[Tooltip("Delay between the attacks")]
	public int[] delay;
	[Tooltip("When the attack ends if not canceled")]
	public int last_frame;
	
	[Tooltip("Mid attack movement")]
	public float movement;
}