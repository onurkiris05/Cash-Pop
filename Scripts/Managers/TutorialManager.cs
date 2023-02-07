using System;
using System.Collections;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using DG.Tweening;
using UnityEngine;
using VP.Nest.UI;
using VP.Nest.UI.InGame;
using VPNest.UI.Scripts.IncrementalUI;

namespace _Main.Scripts.Managers
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        [Header("Scene References")] 
        [SerializeField] private Pin toBePinFirstPin;
        [SerializeField] private Pin toBePinSecondPin;
        
        [Header("Tutorial Needs")] 
        [SerializeField] private GameObject addRope;

        [SerializeField] private GameObject firstDragToMove, secondDragToMove;
        [SerializeField] private GameObject addBall;
        [SerializeField] private PathFtue pathFtue1, pathFtue2;
        [SerializeField] private GameObject p1, p2, p1Target, p2Target;

        private GateManager _gateManager;
        
        private Gate _wall;
        private Node _pin1, _pin2;

        private Coroutine _cor1, _cor2;
        private bool _isRopeAdded;
        private bool _isFirstNodeMoved, _isSecondNodeMoved;
        private bool _isBallAdded;

        private bool isFirstDragPlayed = false;
        private bool isSecondDragPlayed = false;
        private bool hasInit = false;

        private int TutorialStep
        {
            get => PlayerPrefs.GetInt("TutorialStep", 0);
            set => PlayerPrefs.SetInt("TutorialStep", value);
        }

        public bool TutorialCompleted
        {
            get => Convert.ToBoolean(PlayerPrefs.GetInt("TutorialCompleted"));
            set => PlayerPrefs.SetInt("TutorialCompleted", Convert.ToInt32(value));
        }


        private void OnEnable()
        {
            UIManager.Instance.InGameUI.OnLevelStart += StartTutorial;
        }

        private void OnDisable()
        {
            UIManager.Instance.InGameUI.OnLevelStart -= StartTutorial;
        }

        private void Awake()
        {
            if (TutorialCompleted)
            {
                Destroy(gameObject);
            }
        }

        private void StartTutorial()
        {
            _gateManager = FindObjectOfType<GateManager>();
            StartCoroutine(TutorialRoutine());
        }

        private IEnumerator TutorialRoutine()
        {
            while (!TutorialCompleted)
            {
                if (!hasInit && TutorialStep > 0)
                {
                    _wall = _gateManager.Gates[0];
                    _pin1 = _wall.Nodes[0];
                    _pin2 = _wall.Nodes[1];
                    hasInit = true;
                }
                yield return TutorialSteps();
                yield return null;
                TutorialStep++;
                if (TutorialStep is 1) yield return new WaitForSeconds(1.25f);
            }
        }

        private WaitUntil TutorialSteps()
        {
            switch (TutorialStep)
            {
                case 0:
                    OpenAddRope();
                    return new WaitUntil((() => _isRopeAdded));
                case 1:
                    CloseAddRope();
                    OpenFirstDragToMove();
                    return new WaitUntil((() => _isFirstNodeMoved));
                case 2:
                    CloseFirstDragToMove();
                    OpenSecondDragToMove();
                    return new WaitUntil((() => _isSecondNodeMoved));
                case 3:
                    CLoseSecondDragToMove();
                    TutorialCompleted = true;
                    Roller.Instance.MoneyManager.ThrowFirstMoneyRoll();
                    Destroy(gameObject);
                    return null;
                default:
                    return null;
            }
        }

        private void SetRopeAddTrue()
        {
            _isRopeAdded = true;
        }

        private void SetBallAddTrue()
        {
            _isBallAdded = true;
        }

        private void SetFirstNodeMove()
        {
            _isFirstNodeMoved = true;
        }

        private void SetSecondNodeMove()
        {
            _isSecondNodeMoved = true;
        }

        private void CheckPinFirst(Node node, Pin pin)
        {
            SetFirstNodeMove();
        }

        private void CheckPinSecond(Node node, Pin pin)
        {
            SetSecondNodeMove();
        }

        #region Open-Close Funcs

        private void OpenAddRope()
        {
            UIManager.Instance.CurrencyUI.AddMoneyDirect
                (IncrementalManager.Instance.GetWallUpgradeCard(UpgradeType.Wall).incrementalData.CurrentPrice);
            IncrementalManager.Instance.GetWallUpgradeCard(UpgradeType.Wall).OnCurrencyPurchase += SetRopeAddTrue;
            
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Value).gameObject.SetActive(false);
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Length).gameObject.SetActive(false);
            IncrementalManager.Instance.GetMergeUpgradeCard(UpgradeType.Merge).gameObject.SetActive(false);
            
            //addBallButton.DisableManuel();
            addRope.SetActive(true);
        }


        private void CloseAddRope()
        {
            IncrementalManager.Instance.GetWallUpgradeCard(UpgradeType.Wall).OnCurrencyPurchase -= SetRopeAddTrue;
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Value).gameObject.SetActive(true);
            IncrementalManager.Instance.GetUpgradeCard(UpgradeType.Length).gameObject.SetActive(true);
            IncrementalManager.Instance.GetMergeUpgradeCard(UpgradeType.Merge).gameObject.SetActive(true);
            addRope.SetActive(false);
        }


        private void OpenFirstDragToMove()
        {
            SetFirstNodePos();
            _pin1.OnPinToNewSlot += SetFirstNodePos;
            _pin2.Interactable = false;
            firstDragToMove.SetActive(true);
            toBePinFirstPin.OnNodePin += CheckPinFirst;
            isFirstDragPlayed = true;
        }

        private void CloseFirstDragToMove()
        {
            if (!isFirstDragPlayed)
                return;
            
            _pin1.OnPinToNewSlot -= SetFirstNodePos;
            _pin1.Interactable = false;
            _pin2.Interactable = true;
            toBePinFirstPin.OnNodePin -= CheckPinFirst;
            firstDragToMove.SetActive(false);
        }


        private void OpenSecondDragToMove()
        {
            _pin1.Interactable = false;
            _pin2.Interactable = true;
            SetSecondNodePos();
            toBePinSecondPin.OnNodePin += CheckPinSecond;
            _pin2.OnPinToNewSlot += SetSecondNodePos;
            secondDragToMove.SetActive(true);
            isSecondDragPlayed = true;
        }

        private void CLoseSecondDragToMove()
        {
            if (!isSecondDragPlayed)
                return;
            
            _pin1.Interactable = true;
            _pin2.Interactable = true;
            _pin2.OnPinToNewSlot -= SetSecondNodePos;
            toBePinSecondPin.OnNodePin -= CheckPinSecond;
            secondDragToMove.SetActive(false);
        }

        private void OpenAddBall()
        {
            // AddBallButton.OnButtonClick += SetBallAddTrue;
            // addBallButton.EnableManuel();
            addBall.SetActive(true);
        }

        private void CloseAddBall()
        {
            // AddBallButton.OnButtonClick -= SetBallAddTrue;
            addBall.SetActive(false);
        }

        #endregion


        private void SetFirstNodePos()
        {
            p1.transform.position = Camera.main.WorldToScreenPoint(_pin1.transform.position);
            p1Target.transform.position = Camera.main.WorldToScreenPoint(toBePinFirstPin.transform.position);
            DOTween.Kill("FingerMove");
            pathFtue1.InitAgain();
            if (_cor1 != null)
            {
                StopCoroutine(_cor1);
            }

            _cor1 = StartCoroutine(pathFtue1.DoHandMove());
        }

        private void SetSecondNodePos()
        {
            // node 
            p2.transform.position = Camera.main.WorldToScreenPoint(_pin2.transform.position);
            p2Target.transform.position = Camera.main.WorldToScreenPoint(toBePinSecondPin.transform.position);
            DOTween.Kill("FingerMove");
            pathFtue2.InitAgain();
            if (_cor2 != null)
            {
                StopCoroutine(_cor2);
            }

            _cor2 = StartCoroutine(pathFtue2.DoHandMove());
        }
    }
}