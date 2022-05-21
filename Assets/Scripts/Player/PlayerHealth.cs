using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	//referência global do código do personagem
	public static PlayerHealth Instance {get; set;}
	
	//referência do código do player
	private PlayerControl PlayerC;
	
	[SerializeField]
	private int max_hp;//vida máxima
	private int hp;//vida atual
	
	public int hit_id;

	//se o jogador tem invincibility frames
	public bool invul = false, blocking = false;
	public float block_mult = 1;
	
	private void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
	
	private void Start()
	{
		hp = max_hp;
		
		//pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
	}
	
    public void TakeDamage(int dmg)
	{
		if (!invul)
		{
			if(blocking)
				dmg = (int)Mathf.Round(dmg * block_mult);
			hp -= dmg;

			if (hp > max_hp) hp = max_hp;
			else if (hp <= 0)
			{
				print("morreu");
				
				return;
			}
			
			//faz os efeitos de invul frame e animação
			PlayerC.TookDamage();
		}
	}
}
