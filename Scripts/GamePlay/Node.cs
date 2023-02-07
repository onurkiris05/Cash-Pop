using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VP.Nest.Haptic;

public class Node : MonoBehaviour
{
    [Header("General Components")]
    [SerializeField] private Gate gate;
    [SerializeField] private MeshRenderer meshRenderer;

    [Space][Header("Node Settings")]
    [SerializeField] private float radius;

    public SphereCollider Collider;
    public bool CanMove = true;
    public bool IsSnapped;
    public Pin SnappedPin;
    public Action OnPinToNewSlot;
    public Gate Gate => gate;
    public Vector3 OriginalPos;

    public bool Interactable
    {
        get => Collider.enabled;
        set => Collider.enabled = value;
    }

    public void ReleaseNode()
    {
        IsSnapped = false;

        if (SnappedPin != null)
        {
            SnappedPin.IsPinned = false;
            SnappedPin = null;
        }
    }

    public void CheckPosition()
    {
        if (!IsSnapped)
        {
            transform.position = OriginalPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pin") && CanMove)
        {
            Pin pin = other.GetComponent<Pin>();

            if (pin != null && !pin.IsPinned)
            {
                IsSnapped = true;   
                pin.IsPinned = true;
                pin.OnNodePin?.Invoke(this, pin);
                SnappedPin = pin;
                OnPinToNewSlot?.Invoke();
                transform.position = pin.transform.position;
                OriginalPos = transform.position;

                HapticManager.Haptic(HapticType.SoftImpact);
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, radius);
    // }
}