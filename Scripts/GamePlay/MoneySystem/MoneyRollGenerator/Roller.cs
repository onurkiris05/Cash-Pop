using System;
using System.Collections.Generic;
using _Main.Scripts.UI.TapUI;
using _Main.Scripts.Utilities;
using UnityEngine;
using VP.Nest.UI;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator
{
    [RequireComponent(typeof(RollerMeshManager))]
    [RequireComponent(typeof(RollerEarningManager))]
    [RequireComponent(typeof(RollerMoneyManager))]
    public class Roller : Singleton<Roller>
    {
        [SerializeField] private RollerMeshManager rollerMeshManager;
        [SerializeField] private RollerMoneyManager rollerMoneyManager;
        [SerializeField] private RollerEarningManager rollerEarningManager;

        #region EncapsulationMethods

        public RollerMeshManager RollMeshManager => rollerMeshManager;

        public RollerMoneyManager MoneyManager => rollerMoneyManager;

        public RollerEarningManager EarningManager => rollerEarningManager;

        #endregion

        #region UnityEventFunctions

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Terminate();
        }

        #endregion

        #region InitializaitionMethods

        private void Initialize()
        {
            RollMeshManager.Initialize();
            MoneyManager.Initialize();
            EarningManager.Initialize();
        }

        private void Terminate()
        {
            RollMeshManager.Terminate();
            MoneyManager.Terminate();
            EarningManager.Terminate();
        }

        #endregion
    }
}