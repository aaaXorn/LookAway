using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thrown",
                 menuName = "ScriptableObject/AI/ThrownSO")]
public class SpikesSO : ScriptableObject
{
    [Tooltip("Attack damage per hit")]
    public int dmg;
	[Tooltip("Number of spikes")]
	public int number;
	[Tooltip("Time until the attack becomes active")]
	public int startup;
	[Tooltip("Attack duration in frames (24 FPS physics)")]
    public int duration;
	[Tooltip("Spike spawn radius")]
    public float radius;
	[Tooltip("Spike movement height")]
	public float height;
	[Tooltip("Maximum spawn rotation")]
	public int max_rot_mod;
}
