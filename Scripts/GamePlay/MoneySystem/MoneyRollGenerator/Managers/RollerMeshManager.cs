using System.Collections.Generic;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRoll.ModelInfo;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using _Main.Scripts.GamePlay.MoneySystem.MoneyTrail;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public class RollerMeshManager : BaseManager<Roller>
    {
        [SerializeField] private List<MoneyRollModelInfo> moneyRollModelInfos;
        [SerializeField] private List<MoneyTrailInfo> moneyTrailInfos;

        #region EncapsulationMethods

        public MoneyTrailInfo GetMoneyTrailInfo(int level) => moneyTrailInfos[level % moneyTrailInfos.Count];
        
        public MoneyRollModelInfo GetMoneyRollModelInfo (int level) => moneyRollModelInfos[level % moneyRollModelInfos.Count];

        #endregion

        #region InitializationMethods

        public override void Initialize()
        {
            base.Initialize();
        }

        #endregion
    }
}