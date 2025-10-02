using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private DropperMover _dropperMover;
    [SerializeField] private MergeObjectPool _pool;
    [SerializeField] private Dropper _dropper;
    [SerializeField] private ScoreCounter _scoreCounter;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private Deadline _deadline;
    [SerializeField] private EndWindow _newRecord;
    [SerializeField] private EndWindow _result;

    public event UnityAction Restarting;

    private void OnEnable()
    {
        _deadline.Defeated += ShowResult;
        _spawner.Win += ShowResult;
    }

    private void OnDisable()
    {
        _deadline.Defeated -= ShowResult;
        _spawner.Win -= ShowResult;
    }

    public void ShowResult()
    {
        if (_scoreCounter.TrySetRecord(out int recordValue))
            SetResult(_newRecord, $"<color=#FFB938>{recordValue}</color>");
        else
            SetResult(_result, _scoreCounter.Current.ToString());
    }

    public void Restart(Button button)
    {
        Time.timeScale = 1f;
        button.interactable = false;

        DOTween.Sequence().SetUpdate(true)
            .AppendInterval(1)
            .AppendCallback(() => button.interactable = true);

        Restarting?.Invoke();
        SDK.Instance.ShowFullScreenAd();
        SoundsPlayer.Instance.StopSound();
        _scoreCounter.ResetCurrent();
        _spawner.Clear();
        _dropper.Next();
        _dropperMover.enabled = true;
    }

    private void SetResult(EndWindow resultWindow,string score)
    {
        _dropperMover.enabled = false;
        resultWindow.SetScore(score);
        resultWindow.Open();
        Time.timeScale = 0f;
    }
}
