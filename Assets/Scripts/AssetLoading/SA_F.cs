using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SA_F
{
	public static string FileLocation(string relativePath)
	{
		return "file://" + Path.Combine(Application.streamingAssetsPath,
										relativePath);
	}
}
