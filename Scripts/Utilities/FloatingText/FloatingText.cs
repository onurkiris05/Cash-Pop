using System;
using ButtonFever.Managers;
using ButtonFever.Utilities.Pools;
using TMPro;
using UnityEngine;

namespace ButtonFever.Utilities.FloatingText
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        private FloatingTextManager _floatingTextManager;
        
        private float _elapsedLifeTime = 0;
        private Vector3 _startPos;
        private Vector3 _endPos;

        #region UnityEventFunctions

        private void Awake()
        {
            _floatingTextManager = FloatingTextManager.Instance;
        }

        private void OnDisable()
        {
            Reset();
        }

        #endregion
        
        #region FloatingTextMethods

        public void Move()
        {
            _elapsedLifeTime += Time.deltaTime;
            var newPos = Vector3.Lerp(_startPos, _endPos, _floatingTextManager.MoveCurve
                .Evaluate(_elapsedLifeTime / _floatingTextManager.Duration));
            var newScale = Mathf.Lerp(0, 1,
                _floatingTextManager.ScaleCurve.Evaluate(_elapsedLifeTime / _floatingTextManager.Duration));
            transform.position = newPos;
            transform.localScale = Vector3.one * newScale;
        }

        public void CheckRelease()
        {
            if (!(_elapsedLifeTime >= _floatingTextManager.Duration)) return;
            FloatingTextPooler.Instance.ReleaseFloatingText(this);
        }

        #endregion

        #region ImplementationMethods

        public void Init(float amount)
        {
            text.text = $"${amount:0.0}";
            _startPos = transform.position;
            _endPos = _startPos + (Vector3.up * _floatingTextManager.FloatHeight);
        }
        
        private void Reset()
        {
            _elapsedLifeTime = 0;
        }

        #endregion
    }
}