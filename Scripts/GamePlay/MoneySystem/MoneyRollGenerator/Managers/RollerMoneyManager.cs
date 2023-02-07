using System;
using System.Collections.Generic;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRoll;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using DG.Tweening;
using UnityEngine;
using VP.Nest.UI;

namespace _Main.Scripts.Utilities
{
    public class RollerMoneyManager : BaseManager<Roller>
    {
        [Header("Spawn References")]
        [SerializeField] private int moneyCount;
        [SerializeField] private Transform pipeStartPivot;
        [SerializeField] private Transform pipeEndPivot;
        [SerializeField] private MoneyRoll moneyRollPrefab;
        
        private MoneyRoll currentMoneyRoll;
        private readonly Queue<MoneyRoll> moneyRolls = new Queue<MoneyRoll>();
        
        #region InitializaitionMethods

        public override void Initialize()
        {
            base.Initialize();
            SpawnMonies();
            SetToBeThrowingMoneyRoll();
            
            if (!UIManager.Instance.InGameUI.TutorialManager.TutorialCompleted)
                return;
            
            UIManager.Instance.InGameUI.OnLevelStart += ThrowFirstMoneyRoll;
        }

        #endregion

        #region UnityEventFunctions

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (currentMoneyRoll.Movement.IsMoving)
                {
                    currentMoneyRoll.Movement.StopMove();
                }
                else
                {
                    currentMoneyRoll.Movement.StartMove();
                }
            }
        }  
#endif

        #endregion

        #region MoneyMethods

        public void ThrowFirstMoneyRoll()
        {
            currentMoneyRoll.Throw();
            RearrangeMoneyPositions();
        }

        private void SpawnMonies()
        {
            var startPos = pipeStartPivot.position;
            var endPos = pipeEndPivot.position;
            
            Debug.Log("Money Count -> " + moneyCount);
            for (int i = 0; i < moneyCount; i++)
            {
                var spawnPos = Vector3.Lerp(startPos, endPos, (float)i / moneyCount);
                var spawnedMoney = Instantiate(moneyRollPrefab, spawnPos, Quaternion.identity, transform);
                spawnedMoney.Initialize();
                moneyRolls.Enqueue(spawnedMoney);
                Debug.Log("Spawned");
            }
        }
        
        private void SetToBeThrowingMoneyRoll()
        {
            currentMoneyRoll = moneyRolls.Dequeue();
            
            currentMoneyRoll.OnFuelOver += CollectCurrentMoney;
            currentMoneyRoll.OnCollectAllMoney += ThrowCurrentMoney;
        }

        private void ResetToBeThrowingMoneyRoll()
        {
            currentMoneyRoll.OnFuelOver -= CollectCurrentMoney;
            currentMoneyRoll.OnCollectAllMoney -= ThrowCurrentMoney;
            SetToBeThrowingMoneyRoll();   
        }
        
        private void CollectCurrentMoney()
        {
            currentMoneyRoll.ResetMoneyRoll();
            currentMoneyRoll.transform.position = pipeEndPivot.position;
            moneyRolls.Enqueue(currentMoneyRoll);
        }

        private void ThrowCurrentMoney()
        {
            ResetToBeThrowingMoneyRoll();
            
            currentMoneyRoll.Throw();
            RearrangeMoneyPositions();
        }
        
        private void RearrangeMoneyPositions()
        {
            var startPos = pipeStartPivot.position;
            var endPos = pipeEndPivot.position;
            
            var moneyRollsArray = this.moneyRolls.ToArray();
            var moneyRollsCount = moneyRollsArray.Length;
            for (int i = 0; i < moneyRollsCount; i++)
            {
                var nextPos = Vector3.Lerp(startPos, endPos, (float)i / moneyRollsCount);
                moneyRollsArray[i].transform.DOComplete(true);
                moneyRollsArray[i].transform.DOMove(nextPos, .25f).SetEase(Ease.InOutBack).SetLink(moneyRollsArray[i].gameObject)
                    .SetDelay(0.25f * i);
            }
        }

        #endregion
        
    }
}