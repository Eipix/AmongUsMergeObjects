using System;
using DG.Tweening;

public class Timer : IDisposable
{
    private TweenCallback _onComplete;
    private Sequence _sequence;

    private float _seconds;
    private bool _isIndependentUpdate;

    public bool IsDisposed { get; private set; }

    public Timer(float seconds, TweenCallback onComplete, bool isIndependentUpdate = false)
    {
        _seconds = seconds;
        _onComplete = onComplete;
        _isIndependentUpdate = isIndependentUpdate;
    }

    public void Start()
    {
        Validate();

        if (_sequence != null || _sequence.IsActive())
            return;

        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(_isIndependentUpdate);
        _sequence.AppendInterval(_seconds);
        _sequence.OnComplete(() =>
        {
            _onComplete.Invoke();
            Dispose();
        });
    }

    public void Resume()
    {
        Validate();
        _sequence.Play();
    }

    public void Pause()
    {
        Validate();
        _sequence.Pause();
    }

    public void Complete()
    {
        Validate();
        _sequence.Complete(true);
    }

    public void Dispose()
    {
        Validate();

        if (_sequence != null)
        {
            _sequence.Kill();
            _sequence = null;
        }

        _onComplete = null;
        IsDisposed = true;
    }

    private void Validate()
    {
        if(IsDisposed)
            throw new ObjectDisposedException("Timer is disposed!");
    }
}
