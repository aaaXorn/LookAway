using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadConfig : MonoBehaviour
{
    //inicializa as configs
    private void Start()
    {
        SaveSystem.LoadConfig();
    }
}
