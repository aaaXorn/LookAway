using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Stops cinemachine Y rotation from going too far.
/// </summary>
[SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class CinemachineClampY : CinemachineExtension
{
    [Tooltip("Camera clamp value")]
	public float m_YClamp = 45, m_neg_YClamp = -45;

    protected override void PostPipelineStageCallback
					(CinemachineVirtualCameraBase vcam,
					CinemachineCore.Stage stage,
					ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            if(pos.y > m_YClamp)
			{
				pos.y = m_YClamp;
				state.RawPosition = pos;
			}
			else if(pos.y < m_neg_YClamp)
			{
				pos.y = m_neg_YClamp;
				state.RawPosition = pos;
			}
        }
    }
}
