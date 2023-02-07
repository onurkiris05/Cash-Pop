using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallSlot : MonoBehaviour
{
    [SerializeField] private GateManager gateManager;

    public bool IsEmpty = true;
    public int CurrentIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SlotTrigger"))
        {
            CurrentIndex++;
            if (CurrentIndex >= 2)
            {
                IsEmpty = false;
                gateManager.SetSlot(IsEmpty);

                SlotTrigger slot = other.GetComponent<SlotTrigger>();
                slot.SetWallOnSlot(!IsEmpty);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SlotTrigger"))
        {
            CurrentIndex--;
            if (CurrentIndex <= 0)
            {
                IsEmpty = true;
                gateManager.SetSlot(IsEmpty);
                gateManager.CheckWall();

                SlotTrigger slot = other.GetComponent<SlotTrigger>();
                slot.SetWallOnSlot(!IsEmpty);
            }
        }
    }
}