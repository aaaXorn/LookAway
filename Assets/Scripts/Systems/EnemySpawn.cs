using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	//pools dos inimigos
	[SerializeField]
	private PoolManager[] Pool;
	
	private Transform playerT;
	
	//layer do chao
    [SerializeField]
    private LayerMask ground_layer;
	
	[SerializeField]
	private float spawn_rad;
	
	[SerializeField]
	private float spawn_timer;
	
	private void Start()
	{
		playerT = PlayerControl.Instance.transform;
		
		InvokeRepeating("Spawn", spawn_timer, spawn_timer);
	}
	
	private void Spawn()
	{
		Vector3 pos = playerT.position + playerT.up * 20;
		
		pos += (Quaternion.Euler(0, Random.Range(0, 360), 0) * playerT.forward) * spawn_rad;
		
		RaycastHit hit;
		if(Physics.Raycast(pos, -Vector3.up, out hit, 100f, ground_layer))
		{
			int e_type = Random.Range(0, 3);
			
			GameObject Enemy = null;
			
			int i = 0;
			bool e_null = true;
			while(e_null)
			{
				Enemy = Pool[e_type].GetFromPool();
				
				if(Enemy != null) e_null = false;
				
				e_type++;
				if(e_type > 2) e_type = 0;
				
				i++;
				if(i >= 2) e_null = false;
			}
			
			if(Enemy != null)
			{
				Enemy.SetActive(true);
				Enemy.transform.position = hit.point;
				
				Enemy.GetComponent<EnemyHealth>().ResetHP();
				Enemy.GetComponent<EnemyControl>().currentState = EnemyControl.State.Active;
			}
		}
	}
}
