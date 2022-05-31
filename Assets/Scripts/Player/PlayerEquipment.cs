using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    //referência global do código do personagem
    public static PlayerEquipment Instance { get; set; }

    //niveis de equipamento
    public int sword_lvl, armor_lvl, shield_lvl;
    //poções que o jogador tem agora
    public int potions, max_potions;

    private void Awake()
    {
        //setta a referência global desse script
        if (Instance == null) Instance = this;
        //garante que só tem um dele na cena
        else Destroy(gameObject);
    }
}
