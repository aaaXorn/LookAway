using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
	//Axes

	public static float MainHorizontal() { return 0; }
	public static float MainVertical() { return 0; }
	public static float SubHorizontal()
	{
		float r = 0f;
		r += Input.GetAxis("RSX");
		return Mathf.Clamp(r, -1f, 1f);
	}
	public static float SubVertical()
	{
		float r = 0f;
		r += Input.GetAxis("RSY");
		return Mathf.Clamp(r, -1f, 1f);
	}

	public static bool CameraButton()
	{
		return Input.GetButtonDown("L1");
	}
	public static bool CameraButtonHeld()
	{
		return Input.GetButton("L1");
	}
}
