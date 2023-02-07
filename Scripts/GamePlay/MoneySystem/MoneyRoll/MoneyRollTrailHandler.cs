using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using _Main.Scripts.Utilities;
using ButtonFever.Utilities.Pools;
using UnityEngine;
using VP.Nest.UI;
using VP.Nest.UI.Currency;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollTrailHandler : BaseComponent<MoneyRoll>
    {
        [SerializeField] private MoneyTrail.MoneyTrail moneyTrailPrefab;
        
        private Roller roller;
        private FloatingTextPooler floatingTextPooler;
        
        private int currentCollectedMoneyCount = 0;
        private MoneyTrail.MoneyTrail lateMoneyTrail;
        private MoneyTrail.MoneyTrail currentMoneyTrail;
        private readonly List<MoneyTrail.MoneyTrail> spawnedMoneyTrails = new List<MoneyTrail.MoneyTrail>();

        private Coroutine collectingMoneyCoroutine;
        private bool isTrailSet = false;
        private bool isClearingMoney = false;

        #region EncapsulationMethods

        

        #endregion

        #region InitializaitionMethods

        internal override void Initialize()
        {
            base.Initialize();
            roller = Roller.Instance;
            floatingTextPooler = FloatingTextPooler.Instance;
        }

        internal override void ResetComponent()
        {
            base.ResetComponent();
            
            StartCollectAllTrailedMoney();
        }

        #endregion

        #region TrailMethods

        private MoneyTrail.MoneyTrail SpawnNewTrail()
        {
            var modelHolderTransform = BaseComp.ModelController.ModelHolder.transform; 
            var newMoneyTrail = Instantiate(moneyTrailPrefab, modelHolderTransform.position + (Vector3.forward * 0.25f), modelHolderTransform.rotation, modelHolderTransform);
            newMoneyTrail.Initialize(BaseComp.Level, roller.RollMeshManager.GetMoneyTrailInfo(BaseComp.Level), BaseComp);
            spawnedMoneyTrails.Add(newMoneyTrail);
            currentCollectedMoneyCount = 0;
            return newMoneyTrail;
        }

        internal void SetupTrail()
        {
            StartCoroutine(Helper.InvokeAction(() =>
            {
                currentMoneyTrail = SpawnNewTrail();
                isTrailSet = true;
            }, .05f));
        }

        internal void UpgradeTrail()
        {
            lateMoneyTrail = currentMoneyTrail;
            currentMoneyTrail = SpawnNewTrail();
            isTrailSet = false;
            
            StartCoroutine(Helper.InvokeAction(() =>
            {
                lateMoneyTrail.Release();
                isTrailSet = true;
            }, .05f));
        }

        private void StartCollectAllTrailedMoney()
        {
            if (isClearingMoney)
                StopCollectAllTrailedMoney();
            
            collectingMoneyCoroutine = StartCoroutine(ClearAllTrailedMoney());
        }

        private void StopCollectAllTrailedMoney()
        {
            if (!isClearingMoney)
                return;
            
            StopCoroutine(collectingMoneyCoroutine);
        }

        internal void CollectTrailMoney()
        {
            if (!BaseComp.Movement.IsMoving)
                return;

            if (!isTrailSet)
            {
                Debug.Log("Beecause of thiss");
                return;
            }

            var alignedMoneyCount = (int)(currentMoneyTrail.Lenght / 0.75f);
            if (currentCollectedMoneyCount >= alignedMoneyCount) return;
            
            var difference = (alignedMoneyCount - currentCollectedMoneyCount);
            currentCollectedMoneyCount += difference;

            for (int i = 0; i < difference; i++)
            {
                var earningMoney = roller.EarningManager.EarningMoneyPerBanknote(currentMoneyTrail);
                Debug.Log(earningMoney);
                floatingTextPooler.GetFloatingText(transform.position, earningMoney);
                roller.EarningManager.AddMoneyToBeAddedMoney(earningMoney);
            }
        }

        private IEnumerator ClearAllTrailedMoney()
        {
            isClearingMoney = true;
            currentMoneyTrail.Release();
            if (lateMoneyTrail != null) lateMoneyTrail.Release();
            var expectedWaitDuration = 0f;

            foreach (var spawnedMoney in spawnedMoneyTrails)
            {
                var additional = ((int)spawnedMoney.Lenght / 5);
                expectedWaitDuration += MoneyRollRemoteValues.CollectionSpeed + (additional * 0.05f);
            }

            expectedWaitDuration += 0.35f;
            roller.EarningManager.AddToBeAddedMoneyToCurrency(expectedWaitDuration);
            
            foreach (var spawnedMoney in spawnedMoneyTrails)
            {
                yield return spawnedMoney.ResetTrail();
            }
            
            Debug.Log("Finish All Trails");
            spawnedMoneyTrails.Clear();
            isTrailSet = false;
            currentMoneyTrail = null;
            isClearingMoney = false;
            BaseComp.OnCollectAllMoney?.Invoke();
            yield return null;
        }

        #endregion
    }
}