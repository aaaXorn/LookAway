using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingConeDecoupler : MonoBehaviour
{
    public void Detach()
    {
        transform.parent = null;
    }
}
