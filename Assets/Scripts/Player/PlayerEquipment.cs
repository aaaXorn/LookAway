using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    //referência global do código do personagem
    public static PlayerEquipment Instance { get; set; }

	[SerializeField]
	//xp pro nível 1 ao 3
	private int xp_l1, xp_l2, xp_l3;
	//xp dos stats
	public int sword_xp, armor_xp, shield_xp;

    //niveis de equipamento
    public int sword_lvl, armor_lvl, shield_lvl;
    //poções que o jogador tem agora
    public int potions, max_potions;
	
	[SerializeField]
	private GameObject[] Swords, Armors, Shields;
	
    private void Awake()
    {
        //setta a referência global desse script
        if (Instance == null) Instance = this;
        //garante que só tem um dele na cena
        else Destroy(gameObject);
    }

	#region xp
	public void SwordXP(int xp)
    {
		if(sword_lvl >= 3) return;
		
		//aumenta o xp
		sword_xp += xp;
		if(sword_xp > xp_l3) sword_xp = xp_l3;
		
		//muda o nivel quando o xp passar de um ponto
		switch(sword_lvl)
        {
			case 0:
				if (sword_xp >= xp_l1)
				{
					sword_lvl++;
					SwordStats();
				}
				break;

			case 1:
				if (sword_xp >= xp_l2)
				{
					sword_lvl++;
					SwordStats();
				}
				break;

			case 2:
				if (sword_xp >= xp_l3)
				{
					sword_lvl++;
					SwordStats();
				}
				break;

			default:
				sword_lvl = 0;
				break;
		}
    }

	public void ArmorXP(int xp)
	{
		if(armor_lvl >= 3) return;
		
		armor_xp += xp;
		if (armor_xp > xp_l3) armor_xp = xp_l3;

		switch (armor_lvl)
		{
			case 0:
				if (armor_xp >= xp_l1)
				{
					armor_lvl++;
					ArmorStats();
				}
				break;

			case 1:
				if (armor_xp >= xp_l2)
				{
					armor_lvl++;
					ArmorStats();
				}
				break;

			case 2:
				if (armor_xp >= xp_l3)
				{
					armor_lvl++;
					ArmorStats();
				}
				break;

			default:
				armor_lvl = 0;
				break;
		}
	}

	public void ShieldXP(int xp)
	{
		if(shield_lvl >= 3) return;
		
		shield_xp += xp;
		if (shield_xp > xp_l3) shield_xp = xp_l3;

		switch (shield_lvl)
		{
			case 0:
				if (shield_xp >= xp_l1)
				{
					shield_lvl++;
					ShieldChange();
				}
				break;

			case 1:
				if (shield_xp >= xp_l2)
				{
					shield_lvl++;
					ShieldChange();
				}
				break;

			case 2:
				if (shield_xp >= xp_l3)
				{
					shield_lvl++;
					ShieldChange();
				}
				break;
			
			default:
				shield_lvl = 0;
				break;
		}
	}

	#endregion

	#region stat modifiers
	public void SwordStats()
	{
		switch(sword_lvl)
		{
			case 0:
				PlayerControl.Instance.dmg_mod = 1f;
				break;
			
			case 1:
				PlayerControl.Instance.dmg_mod = 1.25f;
				break;
			
			case 2:
				PlayerControl.Instance.dmg_mod = 1.5f;
				break;
			
			default:
				PlayerControl.Instance.dmg_mod = 1f;
				break;
		}
		
		for(int i = 0; i < 3; i++)
		{
			if(i == sword_lvl)
				Swords[i].SetActive(true);
			else
				Swords[i].SetActive(false);
		}
	}
	
	public void ArmorStats()
	{
		switch(armor_lvl)
		{
			case 0:
				PlayerHealth.Instance.max_hp = 100;
				break;
			
			case 1:
				PlayerHealth.Instance.max_hp = 125;
				PlayerHealth.Instance.UpdateHealth(25);
				break;
			
			case 2:
				PlayerHealth.Instance.max_hp = 150;
				PlayerHealth.Instance.UpdateHealth(25);
				break;
			
			default:
				
				break;
		}
		
		for(int i = 0; i < 3; i++)
		{
			if(i == armor_lvl)
				Armors[i].SetActive(true);
			else
				Armors[i].SetActive(false);
		}
	}

	//stats do escudo em PlayerControl.AnimBlock()
	public void ShieldChange()
	{
		for(int i = 0; i < 3; i++)
		{
			if(i == shield_lvl)
				Shields[i].SetActive(true);
			else
				Shields[i].SetActive(false);
		}
	}
	#endregion
}
