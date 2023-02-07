using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollMovement : BaseComponent<MoneyRoll>
    {
        [SerializeField] private float speed;
        private Vector3 direction;

        private bool isMoving = false;

        #region EncapsulationMethods

        public float Speed => speed;

        public Vector3 Direction => direction;

        public bool IsMoving => isMoving;

        #endregion

        #region InitializaitionMethods

        internal override void Initialize()
        {
            base.Initialize();
            direction = new Vector3(-1, 0.5f, 0);
            direction = direction.normalized;
        }

        internal override void ResetComponent()
        {
            base.ResetComponent();
            direction = new Vector3(-1, 0.5f, 0);
            direction = direction.normalized;
        }

        #endregion

        #region MovementMethods

        internal void StartMove()
        {
            isMoving = true;
        }

        internal void StopMove()
        {
            isMoving = false;
        }

        internal void Move()
        {
            if (!isMoving) return;

            transform.Translate(speed * Time.deltaTime * direction);
        }

        #endregion

        #region ReflectMethods

        internal void ReflectFromAreaBounds(Vector3 dir)
        {
            direction = Vector3.Reflect(direction, dir).normalized;
        }

        internal void ReflectFromGate(Gate gate,Vector3 dir)
        {
           
            direction = Vector3.Reflect(direction, dir).normalized;

            gate.GateAnimationHandler.HitAnimation();
        }

        internal void ReflectFromNode(Node node)
        {
            direction *= -1;
        }

        #endregion
    }
}