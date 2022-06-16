using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thrown",
                 menuName = "ScriptableObject/AI/Thrown")]
public class ThrownSO : ScriptableObject
{
    [Tooltip("Attack damage per hit")]
    public int dmg;
    [Tooltip("Horizontal movement duration")]
    public float h_move_time;
    [Tooltip("Gravity")]
    public Vector3 grav;
    [Tooltip("Explosion radius when landing")]
    public float radius;
    [Tooltip("Attack duration in frames (24 FPS physics)")]
    public int duration;
	[Tooltip("Initial vertical force")]
	public float v_force;
}
