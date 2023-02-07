using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Gate : MonoBehaviour
{
    [Header("General Components")]
    [SerializeField] private GateAnimationHandler gateAnimationHandler;
    [SerializeField] private BoxCollider collider;
    [SerializeField] private Material activeMat, activeTransMat, inactiveMat, onSlotMat;
    [SerializeField] private Node[] nodes;

    [Space] [Header("Gate Settings")]
    [SerializeField] private float gateLength;

    public Node[] Nodes => nodes;
    public GateAnimationHandler GateAnimationHandler => gateAnimationHandler;
    public int Level = 1;
    public bool IsOnBoard;
    public bool IsGateInteractable;
    public bool IsOnSlot;

    private GateManager _gateManager;
    private LineRenderer _lineRenderer;
    private bool _isMeshBaked;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineRenderer.positionCount = nodes.Length;
        _gateManager = GetComponentInParent<GateManager>();

        //First initialize for the walls in start
        if (IsOnBoard)
        {
            _gateManager.Gates.Add(this);
            collider.enabled = true;

            foreach (Node node in nodes)
            {
                node.Collider.enabled = true;
            }
        }

        SetLinePosition();
    }

    private void Update()
    {
        SetLinePosition();
        SetLineMaterial();
        SetLineCollider(IsGateInteractable);
    }

    public WallPosition GetNodePosition()
    {
        Vector3[] nodePositions = new Vector3[nodes.Length];

        for (int i = 0; i < nodes.Length; i++)
        {
            nodePositions[i] = nodes[i].transform.position;
        }

        WallPosition pos = new WallPosition(nodePositions);

        return pos;
    }

    public void SetSettings(int level, float gateIncrement)
    {
        Level = level;
        gateLength += (gateIncrement * (Level - 1));
    }

    public void SetUpgradedMaterial(Material material)
    {
        activeMat = material;
        _lineRenderer.material = activeMat;
    }

    public void SetNodes(bool state)
    {
        foreach (Node node in nodes)
        {
            node.Collider.enabled = state;
            node.CanMove = state;
            node.OriginalPos = node.transform.position;
            node.ReleaseNode();
            IsGateInteractable = state;
        }
    }

    private void SetLinePosition()
    {
        //Set line according to positions of nodes
        for (int i = 0; i < nodes.Length; i++)
        {
            _lineRenderer.SetPosition(i, nodes[i].transform.position);
        }
    }

    private void SetLineMaterial()
    {
        if (!nodes[0].CanMove) return;

        //Set line material according to its length
        if (Vector3.Distance(nodes[0].transform.position, nodes[1].transform.position) > gateLength)
        {
            _lineRenderer.material = inactiveMat;
            IsGateInteractable = false;
        }
        else
        {
            if (IsOnSlot)
            {
                _lineRenderer.material = onSlotMat;
                return;
            }

            if (!nodes[0].IsSnapped || !nodes[1].IsSnapped)
            {
                _lineRenderer.material = activeTransMat;
                IsGateInteractable = false;
            }
            else
            {
                _lineRenderer.material = activeMat;
                IsGateInteractable = true;
            }
        }
    }

    private void SetLineCollider(bool isGateActive)
    {
        if (isGateActive)
        {
            Vector3 startPos = nodes[0].transform.position;
            Vector3 endPos = nodes[1].transform.position;

            // Length of line
            float lineLength = Vector3.Distance(startPos, endPos);

            // Size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
            collider.size = new Vector3(lineLength, 0.1f, 1f);

            // Setting position of collider object
            Vector3 midPoint = (startPos + endPos) / 2;
            collider.transform.position = midPoint;

            // Following lines calculate the angle between startPos and endPos
            float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));

            if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
            {
                angle *= -1;
            }

            angle = Mathf.Rad2Deg * Mathf.Atan(angle);

            //Lastly set collider angle and enable
            collider.transform.eulerAngles = new Vector3(0, 0, angle);
            collider.enabled = true;
        }
        else
        {
            collider.enabled = false;
        }
    }

    //Using LineRenderer.BakeMesh()
    // private void SetLineCollider(bool isGateActive)
    // {
    //     if (isGateActive && !_isMeshBaked)
    //     {
    //         Mesh mesh = new Mesh();
    //         _lineRenderer.BakeMesh(mesh);
    //
    //         //Re-adjust mesh vertices according to parent position
    //         var vertices = new Vector3[mesh.vertices.Length];
    //
    //         for (var i = 0; i < vertices.Length; i++)
    //         {
    //             var vertex = mesh.vertices[i];
    //             vertex.x -= transform.position.x;
    //             vertex.y -= transform.position.y;
    //             vertex.z -= transform.position.z;
    //             vertices[i] = vertex;
    //         }
    //
    //         mesh.vertices = vertices;
    //         _collider.sharedMesh = mesh;
    //
    //         _isMeshBaked = true;
    //         _collider.enabled = true;
    //     }
    //     else if (!isGateActive)
    //     {
    //         _isMeshBaked = false;
    //         _collider.enabled = false;
    //     }
    // }
}