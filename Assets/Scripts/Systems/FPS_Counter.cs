using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
	private Text txt;
	
	private int current_frame;
	
	//contagem de fps pra mudança de qualidade dinâmica
	private int dyn_fps_count;
	
	//quantas vezes precisa checar pra cada opção
	[SerializeField]
	private int dyn_checks_decrease = 3, dyn_checks_increase = 10;
	
	[SerializeField]
	private float dyn_check_cd = 1, dyn_change_cd = 1;
	
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
		current_frame = (int)(1f / Time.unscaledDeltaTime);
		txt.text = "FPS: " + current_frame;
	}
	
	private IEnumerator DynamicQuality()
	{
		if(current_frame < 20)
		{
			dyn_fps_count--;
			
			if(dyn_fps_count < -dyn_checks_decrease)
			{
				
				
				yield return new WaitForSeconds(dyn_change_cd);
			}
		}
		else if(current_frame > 28)
		{
			dyn_fps_count++;
			
			if(dyn_fps_count > dyn_checks_increase)
			{
				
				
				yield return new WaitForSeconds(dyn_change_cd);
			}
		}
		
		yield return new WaitForSeconds(dyn_check_cd);
	}
}
