using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	private Camera cam;
	public Vector3 camForward, camRight;

	private void Awake()
	{
		cam = Camera.main;
	}

	private void Update()
	{
		camForward = cam.transform.forward;
		camRight = cam.transform.right;
		camForward.y = 0f;
		camRight.y = 0f;
		camForward.Normalize();
		camRight.Normalize();
	}
}
