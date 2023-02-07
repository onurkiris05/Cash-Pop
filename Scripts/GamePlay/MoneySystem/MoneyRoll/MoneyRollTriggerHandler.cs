using System;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyRoll
{
    public class MoneyRollTriggerHandler : BaseComponent<MoneyRoll>
    {
        #region InitializationMethods

        internal override void Initialize()
        {
            base.Initialize();
        }

        #endregion

        #region UnityEventFunctions

        private void OnCollisionEnter(Collision collision)
        {
            if (!BaseComp.Movement.IsMoving)
                return;

            if (collision.collider.attachedRigidbody &&
                collision.collider.attachedRigidbody.TryGetComponent(out AreaBounds areaBounds))
            {
                BaseComp.MoneyRollHitBounds(collision.GetContact(0).normal);
            }
            else if (collision.collider.attachedRigidbody &&
                     collision.collider.attachedRigidbody.TryGetComponent(out Gate gate))
            {
                BaseComp.MoneyRollHitGate(gate, collision.GetContact(0).normal);
            }
            else if (collision.collider.TryGetComponent(out Node node))
            {
                BaseComp.MoneyRollHitNode(node);
            }
        }

        #endregion
    }
}