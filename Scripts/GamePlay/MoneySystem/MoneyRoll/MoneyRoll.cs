using System;
using _Main.Scripts.Utilities;
using UnityEngine;
using VP.Nest.Haptic;
using VP.Nest.UI;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    [RequireComponent(typeof(MoneyRollMovement))]
    [RequireComponent(typeof(MoneyRollTrailHandler))]
    [RequireComponent(typeof(MoneyRollScaleHandler))]
    [RequireComponent(typeof(MoneyRollTriggerHandler))]
    [RequireComponent(typeof(MoneyRollModelController))]
    [RequireComponent(typeof(MoneyRollAnimationController))]
    public class MoneyRoll : MonoBehaviour
    {
        [SerializeField] private MoneyRollMovement moneyRollMovement;
        [SerializeField] private MoneyRollTrailHandler moneyRollTrailHandler;
        [SerializeField] private MoneyRollScaleHandler moneyRollScaleHandler;
        [SerializeField] private MoneyRollTriggerHandler moneyRollTriggerHandler;
        [SerializeField] private MoneyRollModelController moneyRollModelController;
        [SerializeField] private MoneyRollAnimationController moneyRollAnimationController;

        private int level;
        private int hitSurfaceCount;
        private bool hasInit = false;

        public Action OnHitBounds;
        public Action OnHitGate;
        public Action OnHitNode;
        public Action OnFuelOver;
        public Action OnCollectAllMoney;

        #region EncapsulationMethods

        public MoneyRollMovement Movement => moneyRollMovement;

        public MoneyRollTrailHandler TrailHandler => moneyRollTrailHandler;

        public MoneyRollScaleHandler ScaleHandler => moneyRollScaleHandler;

        public MoneyRollTriggerHandler TriggerHandler => moneyRollTriggerHandler;

        public MoneyRollModelController ModelController => moneyRollModelController;

        public MoneyRollAnimationController AnimationController => moneyRollAnimationController;

        public int Level => level;

        internal int HitSurfaceCount => hitSurfaceCount;

        #endregion

        #region UnityEventFunctions

        private void Update()
        {
            if (!hasInit)
                return;
            
            Movement.Move();
            ScaleHandler.RearrangeScale();
            AnimationController.MovingAnimation();
            TrailHandler.CollectTrailMoney();
            CheckForFuel();
        }

        #endregion

        #region InitializationMethods

        public void Initialize()
        {
            level = IncrementalManager.Instance.GetUpgradeData(UpgradeType.Value).CurrentIndex;

            Movement.Initialize();
            TrailHandler.Initialize();
            ScaleHandler.Initialize();
            TriggerHandler.Initialize();
            ModelController.Initialize();
            AnimationController.Initialize();
            
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Length).OnCurrencyPurchase += UpdateMoneyRollOnUpgrade;
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Value).OnCurrencyPurchase += UpdateMoneyRollOnUpgrade;

            hasInit = true;
        }

        internal void ResetMoneyRoll()
        {
            level = IncrementalManager.Instance.GetUpgradeData(UpgradeType.Value).CurrentIndex;
            
            Movement.ResetComponent();
            TrailHandler.ResetComponent();
            ScaleHandler.ResetComponent();
            TriggerHandler.ResetComponent();
            ModelController.ResetComponent();
            AnimationController.ResetComponent();
        }

        #endregion

        #region MoneyRollEventFunctions

        internal void MoneyRollHitBounds(Vector3 dir)
        {
            Movement.ReflectFromAreaBounds(dir);
            ModelController.AdjustRotation();
            hitSurfaceCount++;
            OnHitBounds?.Invoke();

            HapticManager.Haptic(HapticType.MediumImpact);
            AudioManager.Instance.PlayHitBoundSFX();
        }

        internal void MoneyRollHitGate(Gate gate,Vector3 dir)
        {
            level += MoneyRollRemoteValues.GetUpgradeLevel(gate);
            Movement.ReflectFromGate(gate,dir);
            ModelController.UpdateModel();
            ModelController.AdjustRotation();
            TrailHandler.UpgradeTrail();
            hitSurfaceCount++;
            OnHitGate?.Invoke();

            HapticManager.Haptic(HapticType.HeavyImpact);
            AudioManager.Instance.PlayHitWallSFX();
        }

        internal void MoneyRollHitNode(Node node)
        {
            level += MoneyRollRemoteValues.GetUpgradeLevel(node.Gate);
            Movement.ReflectFromNode(node);
            ModelController.UpdateModel();
            ModelController.AdjustRotation();
            TrailHandler.UpgradeTrail();
            hitSurfaceCount++;
            OnHitNode?.Invoke();
        }

        #endregion

        #region MoneyRollMethods

        public void Throw()
        {
            TrailHandler.SetupTrail();
            Movement.StartMove();
            AnimationController.ThrowAnimation();
        }

        #endregion

        #region UpdateMethods

        private void UpdateMoneyRollOnUpgrade()
        {
            if (!Movement.IsMoving)
                level = IncrementalManager.Instance.GetUpgradeData(UpgradeType.Value).CurrentIndex;
            ScaleHandler.UpdateOriginalScale();
            ModelController.UpdateModel();
        }

        #endregion

        #region CheckingMethods

        private void CheckForFuel()
        {
            if (ScaleHandler.IsFuelOver)
            {
                Movement.StopMove();
                OnFuelOver?.Invoke();
            }
        }

        #endregion
    }
}
