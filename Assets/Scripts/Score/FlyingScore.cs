using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FlyingScore : MonoBehaviour
{
    [SerializeField] private FlyingScorePool _scorePool;
    [SerializeField] private Vector2 _targetOffset;
    [SerializeField] private float _duration;

    public void FlyText(string text, Vector2 startPosition)
    {
        var score = _scorePool.TakeOrAdd();
        score.text = text;
        score.color = Color.white;
        score.transform.position = startPosition;

        Sequence sequence = FlyAway(score, () => _scorePool.Put(score));
    }

    private Sequence FlyAway<T>(T moveObject, TweenCallback onComplete = default) where T : MaskableGraphic
    {
        return DOTween.Sequence()
           .Append(moveObject.transform.DOMove(moveObject.transform.position + (Vector3)_targetOffset, _duration))
           .Join(moveObject.DOFade(0, _duration))
           .OnComplete(onComplete);
    }
}
