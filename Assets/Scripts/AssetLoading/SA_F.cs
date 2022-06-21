using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SA_F
{
	public static string FileLocation(string relativePath)
	{
		#if UNITY_EDITOR
		return "file://" + Path.Combine(Application.streamingAssetsPath,
										relativePath);
		#else
		return "jar:file://" + Path.Combine(Application.streamingAssetsPath,
										relativePath);
		#endif
	}
}
