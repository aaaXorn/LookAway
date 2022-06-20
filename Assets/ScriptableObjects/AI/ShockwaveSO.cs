using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shockwave",
				 menuName = "ScriptableObject/AI/Shockwave")]
public class ShockwaveSO : ScriptableObject
{
    [Tooltip("Attack damage per hit")]
	public int dmg;
	[Tooltip("Attack duration in frames (24 FPS physics)")]
	public int duration;
	[Tooltip("Attack max range")]
	public float max_dist;
	[Tooltip("Attack min range")]
	public float min_dist;
	
	[Tooltip("Attack vertical reach")]
	public float vReach;
	
	[Tooltip("Shockwave speed")]
	public float spd;
	
	[Tooltip("SFX end, continues after duration timer ends")]
	public int sfx_duration;
}
