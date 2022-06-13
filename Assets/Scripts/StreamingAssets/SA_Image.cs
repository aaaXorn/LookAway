using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SA_Image : MonoBehaviour
{
	private UnityWebRequest uwr;
	
    [SerializeField]
	//imagens
	private Image[] imgs;
	
	[SerializeField]
	//nomes dos arquivos das imagens
	private string[] names;
	
	[SerializeField]
	//pixels por unidade dos sprites
	private int[] pxs;
	
	private void Start()
	{
		StartCoroutine("LoadImages");
	}
	
	private IEnumerator LoadImages()
	{
		int i = 0;
		foreach(string s in names)
		{
			//pega a textura da pasta streaming assets
			using(uwr = UnityWebRequestTexture.GetTexture(SA_F.FileLocation(names[i])))
			{
				yield return uwr.SendWebRequest();

				while (!uwr.isDone) yield return null;

				if(uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.Log(uwr.error);
				}
				else
				{
					//muda a textura pra um sprite e coloca no componente Image
					Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
					Sprite spr = CreateSprite(texture, pxs[i]);
					
					imgs[i].sprite = spr;
				}
			}
			
			i++;
		}
		
		yield break;
	}
	
	private Sprite CreateSprite(Texture2D texture, int pixels)
	{
		return Sprite.Create(texture, new Rect(0.0f, 0.0f,
							 texture.width, texture.height),
							 new Vector2(0.5f, 0.5f), pixels);
	}
}
