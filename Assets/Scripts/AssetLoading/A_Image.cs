using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class A_Image : MonoBehaviour
{
	//imagem
	private Image img;
	
	[SerializeField]
	//referência do asset addressable
	private AssetReference sprite;
	
    private void Start()
    {
		img = GetComponent<Image>();
		sprite.LoadAssetAsync<Sprite>().Completed += SpriteLoaded;
	}
	
	private void SpriteLoaded(AsyncOperationHandle<Sprite> spr)
	{
		switch(spr.Status)
		{
			case AsyncOperationStatus.Succeeded:
				img.sprite = spr.Result;
				Destroy(this);
				break;
			case AsyncOperationStatus.Failed:
				Debug.LogError("Sprite load failed.");
				break;
			default:
				break;
		}
	}
}
