using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
	private Text txt;
	
    private void Start()
    {
		txt = GetComponent<Text>();
		
		//repete o void count fps a cada X sec
        //InvokeRepeating("CountFPS", 0, 0.1f);
    }

    //private void CountFPS()
	private void Update()
	{
		//mostra o FPS
		int current_frame = (int)(1f / Time.unscaledDeltaTime);
		txt.text = "FPS: " + current_frame;
	}
}
