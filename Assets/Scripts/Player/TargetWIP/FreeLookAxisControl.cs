using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookAxisControl : MonoBehaviour
{
	public void Start()
	{
		CinemachineCore.GetInputAxis = GetAxisCustom;
	}

	public float GetAxisCustom(string axisName)
	{
		if (axisName == "X Axis")
		{
			return InputManager.SubHorizontal();
		}
		else if (axisName == "Y Axis")
		{
			return InputManager.SubVertical();
		}

		return 0;
	}
}
