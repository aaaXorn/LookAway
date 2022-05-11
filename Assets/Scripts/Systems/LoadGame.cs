using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    //inicializa as configs e as info do player
    private void Start()
    {
        SaveSystem.LoadConfig();
		SaveSystem.LoadGame();
    }
}
