using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ButtonFever.Utilities.Pools
{
    public class FloatingTextPooler : Singleton<FloatingTextPooler>
    {
        [SerializeField] private FloatingText.FloatingText floatingTextPrefab;

        private ObjectPool<FloatingText.FloatingText> _floatingTextPool;

        private List<FloatingText.FloatingText> _activeTexts;
        
        #region EncapsulationMethods
        
        public List<FloatingText.FloatingText> ActiveTexts => _activeTexts;

        #endregion

        #region UnityEventFunctions

        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region PrivateMethods

        private void Initialize()
        {
            _activeTexts = new List<FloatingText.FloatingText>();
            _floatingTextPool = new ObjectPool<FloatingText.FloatingText>(CreateFloatingText,
                OnTakeFloatingTextFromPool, OnReturnFloatingTextFromPool);
        }

        private FloatingText.FloatingText CreateFloatingText()
        {
            var newFloatingText = Instantiate(floatingTextPrefab);
            return newFloatingText;
        }

        private void OnTakeFloatingTextFromPool(FloatingText.FloatingText floatingText)
        {
            floatingText.gameObject.SetActive(true);
        }

        private void OnReturnFloatingTextFromPool(FloatingText.FloatingText floatingText)
        {
            floatingText.gameObject.SetActive(false);
        }

        #endregion

        #region PublicMethods

        public FloatingText.FloatingText GetFloatingText(Vector3 position, float amount)
        {
            var floatingText = _floatingTextPool.Get();
            floatingText.transform.position = position;
            floatingText.Init(amount);
            
            _activeTexts.Add(floatingText);
            return floatingText;
        }

        public void ReleaseFloatingText(FloatingText.FloatingText floatingText)
        {
            _activeTexts.Remove(floatingText);
            _floatingTextPool.Release(floatingText);
        }

        #endregion
    }
}