using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using DG.Tweening;
using UnityEngine;
using VPNest.UI.Scripts.IncrementalUI;

public class GateManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private Transform spawnPos, showUpPos;

    [Space] [Header("Wall Settings")]
    [SerializeField] private WallIncrementalCard wallIncrementalCard;
    [SerializeField] private float wallLengthIncrement;
    [SerializeField] private Material[] wallMaterials;

    [Space] [Header("Merge Settings")]
    [SerializeField] private int mergeSize = 3;
    [SerializeField] private MergeIncrementalCard mergeIncrementalCard;
    [SerializeField] private GameObject mergeParent;
    [SerializeField] private Transform[] mergePoints;

    public List<Gate> Gates;
    public GameObject GatePrefab => gatePrefab;
    public Material[] WallMaterials => wallMaterials;
    public float WallLengthIncrement => wallLengthIncrement;

    private int maxGateLevel = 1;
    private int currentLevel = 1;
    private bool isSlotEmpty = true;

    private void Start()
    {
        IncrementalManager.Instance.GetWallUpgradeCard(UpgradeType.Wall).OnCurrencyPurchase += NewGate;
        IncrementalManager.Instance.GetMergeUpgradeCard(UpgradeType.Merge).OnCurrencyPurchase += MergeGates;

        mergeIncrementalCard.CheckIfAvailable();
    }

    public void NewGate()
    {
        GameObject newGate = Instantiate(gatePrefab, spawnPos.position, Quaternion.identity, transform);

        Gate gate = newGate.GetComponent<Gate>();
        gate.SetNodes(false);
        Gates.Add(gate);
        isSlotEmpty = false;
        wallIncrementalCard.SetSlotFullCard(false);

        newGate.transform.DOMove(showUpPos.position, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            gate.SetNodes(true);
            mergeIncrementalCard.CheckIfAvailable();
        });
    }



    public bool CheckMerge()
    {
        if (GetGates() == null) return false;

        return true;
    }

    public void CheckWall()
    {
        wallIncrementalCard.CheckIfAvailable();
    }

    public void SetSlot(bool state)
    {
        wallIncrementalCard.SetSlotFullCard(state);
    }

    public void MergeGates()
    {
        List<Gate> mergeGates = GetGates();

        if (mergeGates == null) return;

        StartCoroutine(ProcessMergeGates(mergeGates));
    }

    IEnumerator ProcessMergeGates(List<Gate> gates)
    {
        //Set next level from first gate
        int upgradedLevel = gates[0].Level + 1;

        //Get position for upgraded gate to be placed on board
        WallPosition nodePos = gates[0].GetNodePosition();

        //List for nodes to destroy later
        List<Node> nodesToDestroy = new List<Node>();

        int index = 0;

        //Get nodes inside of merge gates, parent it to merge animation parent and disable merge gates
        foreach (Gate gate in gates)
        {
            gate.SetNodes(false);
            gate.transform.DOKill();

            for (int i = 0; i < gate.Nodes.Length; i++)
            {
                nodesToDestroy.Add(gate.Nodes[i]);
                gate.Nodes[i].transform.parent = mergePoints[index];
                gate.Nodes[i].transform.DOMove(mergePoints[index].position, 0.5f);
                index++;
            }

            Gates.Remove(gate);
            Destroy(gate.gameObject);
        }

        //Set animations for merge animation parent
        mergeParent.transform.DORotate(new Vector3(0, 0, -180), 0.3f).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        mergeParent.transform.DOScale(new Vector3(0, 0, 0), 1.5f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                mergeParent.transform.DOKill();

                foreach (Node node in nodesToDestroy)
                {
                    Destroy(node.gameObject);
                }

                nodesToDestroy.Clear();

                //Reset merge parent game object
                mergeParent.transform.localScale = Vector3.one;
                mergeParent.transform.rotation = Quaternion.Euler(Vector3.zero);
            });

        //Check if there is possible merge option with new merged item
        mergeIncrementalCard.CheckIfAvailable();

        yield return new WaitForSeconds(1.5f);

        SpawnUpgradedGate(upgradedLevel, nodePos);
    }

    private void SpawnUpgradedGate(int upgradedLevel, WallPosition boardPos)
    {
        Vector3 spawnPos = new Vector3(0, 1, 0);

        //Instantiate new merged item and set scale zero
        GameObject upgradedGate = Instantiate(gatePrefab, spawnPos, Quaternion.identity, transform);
        upgradedGate.transform.localScale = new Vector3(0, 0, 0);

        //Set settings for item and nodes
        Gate newGate = upgradedGate.GetComponent<Gate>();
        newGate.SetSettings(upgradedLevel, wallLengthIncrement);
        newGate.SetNodes(false);
        newGate.SetUpgradedMaterial(wallMaterials[upgradedLevel - 1]);
        Gates.Add(newGate);

        //Set new max gate level
        if (upgradedLevel > maxGateLevel) maxGateLevel = upgradedLevel;

        //Place new gate into board
        for (int i = 0; i < newGate.Nodes.Length; i++)
        {
            newGate.Nodes[i].transform.DOMove(boardPos.NodePositions[i], 1f);
        }

        upgradedGate.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                newGate.SetNodes(true);
                mergeIncrementalCard.CheckIfAvailable();
            });
    }

    private List<Gate> GetGates()
    {
        List<Gate> mergeGates = new List<Gate>();

        currentLevel = 1;

        //Iterate all gates along maxGateLevel
        for (int i = 0; i < maxGateLevel; i++)
        {
            //Find gates that has same level
            mergeGates = Gates.FindAll(x => x.Level.Equals(currentLevel));

            //Check founded gate size if lower then mergeSize, If its, increase current level check again
            if (mergeGates.Count < mergeSize)
            {
                currentLevel++;
            }
            //Check founded gate size if higher then mergeSize, If its, subtract excess ones
            else if (mergeGates.Count > mergeSize)
            {
                int subtractNum = mergeGates.Count - mergeSize;

                for (int j = 0; j < subtractNum; j++)
                {
                    mergeGates.Remove(mergeGates[j]);
                }

                break;
            }
            else break;
        }

        //If there are not enough same walls at all, return null
        if (mergeGates.Count < mergeSize) return null;

        return mergeGates;
    }
}
