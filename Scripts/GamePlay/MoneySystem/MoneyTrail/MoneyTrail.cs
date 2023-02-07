using System;
using System.Collections;
using _Main.Scripts.GamePlay.MoneySystem.MoneyRollGenerator;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyTrail
{
    public class MoneyTrail : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private ParticleSystem collectingParticle;
        [SerializeField] private Material[] trailMaterials;

        private int level;
        private int materialIndex;
        private MoneyTrailInfo moneyTrailInfo;
        private MoneyRoll.MoneyRoll _moneyRoll;

        private bool isStartMoving = false;

        #region EncapsulationMethods

        public int Level => level;

        public float Lenght => trailRenderer.GetLength();

        public int TrailPositionCount => trailRenderer.positionCount;

        #endregion

        #region InitializaitionMethods

        public void Initialize(int level, MoneyTrailInfo moneyTrailInfo, MoneyRoll.MoneyRoll moneyRoll)
        {
            this.level = level;
            materialIndex = this.level;
            if (materialIndex > trailMaterials.Length - 1) materialIndex = trailMaterials.Length - 1;
            this.moneyTrailInfo = moneyTrailInfo;
            _moneyRoll = moneyRoll;
            trailRenderer.sortingOrder = level;
            //trailRenderer.material.SetColor("_BaseColor", moneyTrailInfo.trailColor);
            trailRenderer.material = trailMaterials[materialIndex];
            isStartMoving = true;
        }

        public void Release()
        {
            transform.parent = null;
            isStartMoving = false;
        }

        public IEnumerator ResetTrail()
        {
            isStartMoving = false;
            //ChangeParticleColor(collectingParticle, moneyTrailInfo.trailColor);
            collectingParticle.Play();

            var lenght = trailRenderer.GetLength();

            var points = new Vector3[trailRenderer.positionCount];
            var count = trailRenderer.GetPositions(points);
            var additional = ((int)lenght / 5);
            var duration = MoneyRollRemoteValues.CollectionSpeed + (additional * 0.05f);
            var durationPerIteration = duration / count;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    trailRenderer.SetPosition(j, points[i]);
                    var particleTransform = collectingParticle.transform;
                    particleTransform.position = points[j];

                    if (j + 1 <= i)
                        particleTransform.LookAt(points[j + 1]);
                }

                yield return new WaitForSeconds(durationPerIteration);
            }

            collectingParticle.Stop();
            // Roller.Instance.EarningManager.CollectTrailEarning(this, lenght);
            StartCoroutine(Helper.InvokeAction(() => Destroy(gameObject)));
        }

        private void ChangeParticleColor(ParticleSystem particle, Color color)
        {
            ParticleSystem.MainModule mainSettings = particle.main;
            mainSettings.startColor = new ParticleSystem.MinMaxGradient(color);

            ParticleSystem[] particles = particle.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particles.Length; i++)
            {
                mainSettings = particles[i].main;
                mainSettings.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }

        #endregion

        #region UnityEventFunctions

        private void Update()
        {
            if (!isStartMoving)
                return;

            SetTiling();
        }

        #endregion

        private void SetTiling()
        {
            if (!_moneyRoll.Movement.IsMoving) return;

            trailRenderer.material.mainTextureOffset += Vector2.left * 7f * Time.deltaTime;
        }
    }
}