using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyTrail
{
    [CreateAssetMenu(fileName = "New Money Trail Info", menuName = "MoneyTrail/Info")]
    public class MoneyTrailInfo : ScriptableObject
    {
        public Color trailColor;
    }
}
