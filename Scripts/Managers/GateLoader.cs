using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Unity.Collections;
using UnityEngine;

public class GateLoader : MonoBehaviour
{
    private GateManager _gateManager;
    private List<WallPosition> wallPositions = new List<WallPosition>();
    private List<int> wallLevels = new List<int>();

    private void Awake()
    {
        _gateManager = GetComponent<GateManager>();

        if (PlayerPrefs.GetInt("NewGame") > 0)
        {
            LoadGates(LoadPositions());
        }
        else
        {
            PlayerPrefs.SetInt("NewGame", 1);
        }
    }

    private void Start()
    {
        StartCoroutine(StartSaving());
    }

    IEnumerator StartSaving()
    {
        while (true)
        {
            wallPositions.Clear();
            wallLevels.Clear();

            for (int i = 0; i < _gateManager.Gates.Count; i++)
            {
                WallPosition pos = _gateManager.Gates[i].GetNodePosition();
                wallPositions.Add(pos);

                int level = _gateManager.Gates[i].Level;
                wallLevels.Add(level);
            }

            SavePositionsAndLevels(wallPositions, wallLevels);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void LoadGates(WallPosition[] loadWalls)
    {
        for (int i = 0; i < loadWalls.Length; i++)
        {
            GameObject newGate = Instantiate(_gateManager.GatePrefab, transform.position, Quaternion.identity, transform);
            Gate gate = newGate.GetComponent<Gate>();

            int level = PlayerPrefs.GetInt($"{i}_wall_level");
            gate.SetSettings(level, _gateManager.WallLengthIncrement);
            gate.SetUpgradedMaterial(_gateManager.WallMaterials[level - 1]);

            for (int j = 0; j < gate.Nodes.Length; j++)
            {
                gate.Nodes[j].transform.position = loadWalls[i].NodePositions[j];
            }

            gate.SetNodes(true);
            _gateManager.Gates.Add(gate);
        }
    }

    private void SavePositionsAndLevels(List<WallPosition> wallPos, List<int> levels)
    {
        PlayerPrefs.SetInt("total_wall_count", wallPos.Count);

        for (int i = 0; i < wallPos.Count; i++)
        {
            PlayerPrefs.SetInt($"{i}_wall_level", levels[i]);

            for (int j = 0; j < wallPos[i].NodePositions.Length; j++)
            {
                PlayerPrefs.SetFloat($"{i}_wall_{j}_node_x", wallPos[i].NodePositions[j].x);
                PlayerPrefs.SetFloat($"{i}_wall_{j}_node_y", wallPos[i].NodePositions[j].y);
            }
        }
    }

    private WallPosition[] LoadPositions()
    {
        int wallCount = PlayerPrefs.GetInt("total_wall_count");

        WallPosition[] loadPositions = new WallPosition[wallCount];

        for (int i = 0; i < wallCount; i++)
        {
            loadPositions[i] = new WallPosition();
            loadPositions[i].NodePositions = new Vector3[2];

            for (int j = 0; j < loadPositions[i].NodePositions.Length; j++)
            {
                loadPositions[i].NodePositions[j].x = PlayerPrefs.GetFloat($"{i}_wall_{j}_node_x");
                loadPositions[i].NodePositions[j].y = PlayerPrefs.GetFloat($"{i}_wall_{j}_node_y");
            }
        }

        return loadPositions;
    }
}