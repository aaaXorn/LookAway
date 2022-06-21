using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class R_Image : MonoBehaviour
{
    //imagem
	private Image img;
	
	[SerializeField]
	private string path;
	
    private void Start()
    {
		img = GetComponent<Image>();
		img.sprite = Resources.Load<Sprite>(path);
	}
}
