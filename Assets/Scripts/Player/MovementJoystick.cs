using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementJoystick : Joystick
{
	//referência global do código do joystick
	public static MovementJoystick Instance {get; set;}
	
    void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
}
