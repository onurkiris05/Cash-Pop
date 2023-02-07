using _Main.Scripts.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollAnimationController : BaseComponent<MoneyRoll>
    {
        private float movingSpeedPerFrame;
        private bool onScaleAnimation;

        #region EncapsulationMethods

        public bool OnScaleAnimation => onScaleAnimation;

        #endregion

        #region InitializationMethods

        internal override void Initialize()
        {
            base.Initialize();
            CalculateMovingSpeed();
        }

        #endregion

        #region HelperMethods

        private void CalculateMovingSpeed()
        {
            movingSpeedPerFrame = BaseComp.Movement.Speed;
        }

        #endregion

        #region AnimationMethods

        internal void MovingAnimation()
        {
            if (!BaseComp.Movement.IsMoving)
                return;

            BaseComp.ModelController.CurrentModelRenderer.material.mainTextureOffset +=
                (BaseComp.HitSurfaceCount % 2 == 0 ? Vector2.right : Vector2.left) * movingSpeedPerFrame * Time.deltaTime;
        }

        internal void ThrowAnimation()
        {
            return;
            onScaleAnimation = true;
            
            var modelTransform = BaseComp.ModelController.CurrentModel.transform;
            modelTransform.DOComplete(true);
            modelTransform.DOScale(1.2f, .75f)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(modelTransform.gameObject)
                .SetEase(Ease.OutCirc)
                .OnComplete(()=> onScaleAnimation = false);
        }

        #endregion
    }
}