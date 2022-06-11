using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser",
                 menuName = "ScriptableObject/AI/Laser")]
public class LaserSO : ScriptableObject
{
    [Tooltip("Attack damage per hit")]
    public int dmg;
	[Tooltip("Attack duration in frames (24 FPS physics)")]
    public int duration;
	[Tooltip("Projectile speed")]
	public float spd;
}
