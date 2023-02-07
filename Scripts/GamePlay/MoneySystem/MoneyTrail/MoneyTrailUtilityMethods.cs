using UnityEngine;

namespace _Main.Scripts.GamePlay.MoneySystem.MoneyTrail
{
    public static class MoneyTrailUtilityMethods
    {
        public static float GetLength(this TrailRenderer trailRenderer)
        {
            var points = new Vector3[trailRenderer.positionCount]; 
            var count = trailRenderer.GetPositions(points);
            
            if(count < 2) return 0f;
            
            var length = 0f;
            var start = points[0];
            
            for(var i = 1; i < count; i++)
            {
                var end = points[i];
                length += Vector3.Distance(start, end);
                start = end;
            }
            return length;
        }
    }
}