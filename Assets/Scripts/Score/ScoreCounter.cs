using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;
    [SerializeField] private Score _current;
    [SerializeField] private Score _record;

    public int Current => _current.Value;

    private void OnEnable() => _spawner.OnMerged += Add;

    private void OnDisable() => _spawner.OnMerged -= Add;

    public void LoadScores()
    {
        _current.Value = 0;
        _record.Value = 0;

        var record = SaveSerial.Instance.Load(_record.Key, 0);
        var current = SaveSerial.Instance.Load(_current.Key, 0);

        if (current is long longCurrent)
            _current.Value = (int)longCurrent;

        if (record is long longRecord)
            _record.Value = (int)longRecord;
    }

    private void Add(int score)
    {
        if (score < 0)
            return;

        _current.Value += score;
        SaveSerial.Instance.Save(_current.Key, _current.Value);
        TrySetRecord(out int recordValue);
    }

    public void ResetCurrent() => _current.Value = 0;

    public bool TrySetRecord(out int recordValue)
    {
        recordValue = 0;

        if(_current.Value < _record.Value)
            return false;

        _record.Value = _current.Value;
        recordValue = _record.Value;
        SaveSerial.Instance.Save(_record.Key, _record.Value);
        SDK.Instance.SetToLeaderBoard(_record.Value);
        return true;
    }
}
