using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SA_Audio : MonoBehaviour
{
    private UnityWebRequest uwr;

	[SerializeField]
	//audio
	private AudioSource[] audio;

	[SerializeField]
	//nomes dos arquivos de audio
	private string[] names;

	private void Start()
	{
		StartCoroutine("LoadAudio");
	}

	private IEnumerator LoadAudio()
    {
		int i = 0;
		foreach (string s in names)
		{
			//pega o audio da pasta streaming assets
			using (uwr = UnityWebRequestMultimedia.GetAudioClip(SA_F.FileLocation(names[i]), AudioType.MPEG))
			{
				yield return uwr.SendWebRequest();

				if (uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.LogError(uwr.error);
				}
				else
				{
					//muda o audio clip do audio source
					AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);

					audio[i].clip = clip;
				}
			}

			i++;
		}

		yield break;
    }
}
