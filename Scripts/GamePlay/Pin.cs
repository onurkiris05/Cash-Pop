using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public bool IsPinned;
    public Action<Node, Pin> OnNodePin;

    private void OnDrawGizmos()
    {
        if (!IsPinned)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}