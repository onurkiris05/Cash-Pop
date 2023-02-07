using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTrigger : MonoBehaviour
{
    [SerializeField] private Gate gate;

    public void SetWallOnSlot(bool state)
    {
        gate.IsOnSlot = state;
    }
}
