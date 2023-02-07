using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollScaleHandler : BaseComponent<MoneyRoll>
    {
        private Vector3 originalScale;
        [SerializeField]private float currentLength;
        private bool hasStarted;

        #region EncapsulationMethods

        internal bool IsFuelOver => currentLength <= 0;

        public Vector3 OriginalScale => originalScale;

        internal float Length
        {
            get
            {
                Debug.Log("Yes" + IncrementalManager.Instance.GetUpgradeData(UpgradeType.Length).CurrentIndex);
                Debug.Log("Yessss" + IncrementalManager.Instance.GetUpgradeData(UpgradeType.Length).CurrentAdditionalValue);
                return MoneyRollRemoteValues.BaseLenght +
                       IncrementalManager.Instance.GetUpgradeData(UpgradeType.Length).CurrentAdditionalValue;
            }
        }

        #endregion

        #region InitializationMethods

        internal override void Initialize()
        {
            base.Initialize();
            currentLength = Length;
            UpdateOriginalScale();
        }

        internal override void ResetComponent()
        {
            base.ResetComponent();
            currentLength = Length;
        }

        #endregion

        #region ScaleMethods

        internal void RearrangeScale()
        {
            if (!BaseComp.Movement.IsMoving) 
                return;

            if (BaseComp.AnimationController.OnScaleAnimation)
                return;

            currentLength -= Time.deltaTime;
            SetScale(currentLength, Length);
        }

        internal void UpdateOriginalScale()
        {
            if (!BaseComp.Movement.IsMoving)
                currentLength = Length;
            
            var size = Mathf.Lerp(1, 1.5f, BaseComp.ScaleHandler.Length / 50f);
            originalScale =  new Vector3(size, 1, size);
        }

        private void SetScale(float remainingRollLength, float originalRollLength)
        {
            float scaleVal = UsefulFunctions.Map(remainingRollLength, 0, originalRollLength, 0, originalScale.x);
            BaseComp.ModelController.CurrentModel.transform.localScale = new Vector3(scaleVal, originalScale.y, scaleVal);
        }

        #endregion
    }
}
