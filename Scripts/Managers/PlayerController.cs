using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float snapSensitivity;
    [SerializeField] private float minY, maxY;

    private Node nodeSelected;
    private Finger movementFinger;
    private RaycastHit hit;
    private Ray ray;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;

        EnhancedTouchSupport.Disable();
    }

    void HandleFingerDown(Finger touchedFinger)
    {
        if (movementFinger == null)
        {
            movementFinger = touchedFinger;

            ray = Camera.main.ScreenPointToRay(movementFinger.currentTouch.screenPosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                nodeSelected = hit.transform.gameObject.GetComponent<Node>();
            }
        }
    }

    void HandleFingerUp(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;

            if (nodeSelected != null)
            {
                nodeSelected.CheckPosition();
                nodeSelected = null;
            }
        }
    }

    void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
            if (nodeSelected != null && nodeSelected.CanMove)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        Vector2 touchPos = movementFinger.currentTouch.screenPosition;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10f));

        //Return if input comes outside of screen bounds
        if (targetPos.y < minY || targetPos.y > maxY) return;

        //Check distance between input and item for snap sensitivity
        if (nodeSelected.IsSnapped)
        {
            Vector3 snappedPinPos = nodeSelected.SnappedPin.transform.position;

            if (Vector3.Distance(targetPos, snappedPinPos) > snapSensitivity)
            {
                nodeSelected.ReleaseNode();
            }
            else return;
        }

        //Lerp item position to input position
        nodeSelected.transform.position = Vector3.Lerp(nodeSelected.transform.position, targetPos,
            moveSpeed * Time.deltaTime);
    }
}