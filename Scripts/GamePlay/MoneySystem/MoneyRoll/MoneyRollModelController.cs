using _Main.Scripts.GamePlay.MoneySystem.MoneyRoll.ModelInfo;
using _Main.Scripts.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollModelController : BaseComponent<MoneyRoll>
    {
        [SerializeField] private GameObject modelHolder;
        [SerializeField] private GameObject currentModel;
        [SerializeField] private Renderer currentModelRenderer;
        [SerializeField] private Renderer currentModelSideRenderer;

        private MoneyRollModelInfo moneyRollModelInfo;
        
        private RollerMeshManager rollerMeshManager;

        #region EncapsulationMethods

        public GameObject ModelHolder => modelHolder;

        public GameObject CurrentModel => currentModel;

        public Renderer CurrentModelRenderer => currentModelRenderer;

        #endregion

        #region InitializaitionMethods

        internal override void Initialize()
        {
            base.Initialize();
            rollerMeshManager = MoneyRollGenerator.Roller.Instance.RollMeshManager;
            UpdateModelInfo();
            UpdateModelSize(true);
            AdjustRotation();
        }

        internal override void ResetComponent()
        {
            base.ResetComponent();
            UpdateModel();
            AdjustRotation();
        }

        #endregion

        #region ModelMethods

        internal void UpdateModel()
        {
            UpdateModelInfo();
            UpdateModelSize();
        }
        
        private void UpdateModelInfo()
        {
            moneyRollModelInfo = rollerMeshManager.GetMoneyRollModelInfo(BaseComp.Level);
            //currentModelRenderer.material.SetColor("_BaseColor", moneyRollModelInfo.modelColor);
            //currentModelSideRenderer.material.SetColor("_BaseColor", moneyRollModelInfo.modelColor);
        }

        private void UpdateModelSize(bool isInit = false)
        {
            var size = Mathf.Lerp(1, 1.5f, BaseComp.ScaleHandler.Length / 50f);
            currentModel.transform.localScale = new Vector3(size, 1, size);
        }

        #endregion

        #region ModelHandlerMethods

        internal void AdjustRotation()
        {
            BaseComp.ModelController.ModelHolder.transform.DOComplete();
            float angle = (BaseComp.HitSurfaceCount % 2 == 0 ? 0 : 180) - Vector2.SignedAngle(BaseComp.Movement.Direction, Vector2.up);
            BaseComp.ModelController.ModelHolder.transform.DOLocalRotate(new Vector3(0, 0, angle), 0.2f, RotateMode.Fast);
        }

        #endregion
    }
}