using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	//referência global do código do personagem
	public static PlayerHealth Instance {get; set;}
	
	public int max_hp;//vida máxima
	private int hp;//vida atual

	//se o jogador tem invincibility frames
	public bool invul = false, blocking = false;
	public float block_mult = 1;

	//barra de hp
	[SerializeField]
	private Image hp_img;

	//número de poções sobrando
	public Text pot_txt;
	
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

		//UI
		pot_txt.text = "" + PlayerEquipment.Instance.potions;
	}
	
    public void TakeDamage(int dmg)
	{
		if (!invul)
		{
			//se dando block, diminui o dano
			if(blocking && dmg > 0)
				dmg = (int)Mathf.Round((float)dmg * block_mult);
			//diminui/aumenta o HP
			if(hp > 0) hp -= dmg;

			//limita o HP pra não ultrapassar max_hp
			if (hp > max_hp) hp = max_hp;
			//player morre
			else if (hp <= 0)
			{
				hp = 0;

				PlayerControl.Instance.Dying();
				
				//UI
				hp_img.fillAmount = 0;
				
				return;
			}
			
			if(!blocking)
				//faz os efeitos de invul frame e animação
				PlayerControl.Instance.TookDamage();

			//UI
			hp_img.fillAmount = (float)hp / max_hp;
		}
	}
	
	public void ReceiveHealing(int heal)
	{
		UpdateHealth(heal);

		//diminui as poções restantes por 1
		PlayerEquipment.Instance.potions--;

		//UI
		pot_txt.text = "" + PlayerEquipment.Instance.potions;
	}
	
	public void UpdateHealth(int heal)
	{
		//aumenta o HP
		if (hp > 0) hp += heal;

		//limita o HP pra não ultrapassar max_hp
		if (hp > max_hp) hp = max_hp;
		
		//UI
		hp_img.fillAmount = (float)hp / max_hp;
	}
}
