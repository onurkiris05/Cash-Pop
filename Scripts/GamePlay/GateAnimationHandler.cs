using DG.Tweening;
using UnityEngine;

public class GateAnimationHandler : MonoBehaviour
{
    public void HitAnimation()
    {
        transform.DOComplete();
        transform.DOScale(1.02f, 0.4f).From().SetEase(Ease.OutBack);
    }
}
