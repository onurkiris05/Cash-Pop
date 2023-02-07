using System;
using ButtonFever.Utilities.Pools;
using UnityEngine;

namespace ButtonFever.Managers
{
    public class FloatingTextManager : Singleton<FloatingTextManager>
    {
        [SerializeField] private AnimationCurve moveCurve;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private float floatHeight;
        [SerializeField] private float duration;

        private FloatingTextPooler _floatingTextPooler;

        #region EncapsulatinMethods
        
        public float Duration => duration;

        public float FloatHeight => floatHeight;

        public AnimationCurve MoveCurve => moveCurve;

        public AnimationCurve ScaleCurve => scaleCurve;

        #endregion
        
        #region UnityEventFunctions

        private void Awake()
        {
            _floatingTextPooler = FloatingTextPooler.Instance;
        }

        private void Update()
        {
            MoveAllTexts();
        }

        #endregion

        #region FloatingTextManager

        private void MoveAllTexts()
        {
            var activeFloatingTextCount = _floatingTextPooler.ActiveTexts.Count;
            
            for (int i = 0; i < activeFloatingTextCount; i++)
            {
                _floatingTextPooler.ActiveTexts[i].Move();
            }
            
            for (int i = activeFloatingTextCount-1; i >= 0; i--)
            {
                _floatingTextPooler.ActiveTexts[i].CheckRelease();
            }
        }

        #endregion
    }
}