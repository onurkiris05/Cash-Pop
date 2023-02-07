using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using _Main.Scripts.GamePlay.MoneySystem.MoneyTrail;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VP.Nest.UI;
using VP.Nest.UI.Currency;

namespace _Main.Scripts.Utilities
{
    public class RollerEarningManager : BaseManager<Roller>
    {
        [Header("To Be Added Money")] 
        [SerializeField] private Transform uiTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI toBeAddedMoneyText;
        private float currentToBeAddedMoney;
        private float nextToBeAddedMoney;
        private bool isPanelClose = true;
        
        private float pricePerBanknote = 1;
        private float levelEarningMultiplier = 0.2f;

        private CurrencyUI currencyUI;

        #region EncapsulationMethods

        public float EarningMoneyPerBanknote(MoneyTrail moneyTrail) => 1 + (moneyTrail.Level * levelEarningMultiplier) * pricePerBanknote;

        #endregion

        #region InitializaitionMethods

        public override void Initialize()
        {
            base.Initialize();
            pricePerBanknote = MoneyRollRemoteValues.BasePricePerBanknote;
            levelEarningMultiplier = MoneyRollRemoteValues.AdditionalMultiplierPerLevel;

            currencyUI = UIManager.Instance.CurrencyUI;
        }

        #endregion

        #region ToBeAddedPanelMethods

        private void OpenPanel()
        {
            if (!isPanelClose)
                return;

            uiTransform.DOKill(true);
            canvasGroup.DOComplete(true);
            uiTransform.DOScale(1, .5f).SetEase(Ease.InQuart).SetLink(uiTransform.gameObject);
            canvasGroup.DOAlpha(1, .5f).SetEase(Ease.InQuart).SetLink(canvasGroup.gameObject);
            isPanelClose = false;
        }

        private void ClosePanel()
        {
            if (isPanelClose)
                return;

            uiTransform.DOKill(true);
            canvasGroup.DOComplete(true);
            uiTransform.DOScale(0, .5f).SetEase(Ease.OutQuart).SetLink(uiTransform.gameObject);
            canvasGroup.DOAlpha(0, .5f).SetEase(Ease.OutQuart).SetLink(canvasGroup.gameObject);
            isPanelClose = true;
        }

        #endregion

        #region ToBeAddedMoneyMethods

        public void AddMoneyToBeAddedMoney(float amount)
        {
            OpenPanel();

            nextToBeAddedMoney += amount;

            toBeAddedMoneyText.transform.DOComplete();
            toBeAddedMoneyText.transform.DOPunchScale(Vector3.one * .05f, .2f, 2, .5f);

            DOTween.Complete("ToBeAddedMoneyAnimation");
            DOTween.To(() => currentToBeAddedMoney, x => currentToBeAddedMoney = x, nextToBeAddedMoney, .2f)
                .SetId("ToBeAddedMoneyAnimation").SetEase(Ease.OutCubic)
                .OnUpdate(() => toBeAddedMoneyText.SetText("+" + ((int)currentToBeAddedMoney).FormatMoney()));
        }
        
        public void AddToBeAddedMoneyToCurrency(float duration)
        {
            currencyUI.AddMoneyWithCountAnim(currentToBeAddedMoney, duration);

            StartCoroutine(Helper.InvokeAction(() =>
            {
                toBeAddedMoneyText.transform.DOComplete(true);
                toBeAddedMoneyText.transform.DOScale(1.2f, 0.125f).SetLoops(2, LoopType.Yoyo).SetLink(toBeAddedMoneyText.gameObject).OnComplete(
                    () =>
                    {
                        currentToBeAddedMoney = 0;
                        nextToBeAddedMoney = 0;
                        toBeAddedMoneyText.SetText("+" + ((int)currentToBeAddedMoney).FormatMoney());
                        ClosePanel(); 
                    });
            }, duration));

            // DOTween.Complete("ReducingToBeAddedMoneyAnimation");
            // DOTween.To(() => currentToBeAddedMoney, x => currentToBeAddedMoney = x, 0, duration)
            //     .SetId("ReducingToBeAddedMoneyAnimation").SetEase(Ease.OutCubic)
            //     .OnUpdate(() => toBeAddedMoneyText.SetText("+" + currentToBeAddedMoney.FormatMoney())).OnComplete(() =>
            //     {
            //         currentToBeAddedMoney = 0;
            //         nextToBeAddedMoney = 0;
            //         toBeAddedMoneyText.SetText("+" + ((int)currentToBeAddedMoney).FormatMoney());
            //         ClosePanel();
            //     }).SetLink(toBeAddedMoneyText.gameObject);
        }

        #endregion
    }
}