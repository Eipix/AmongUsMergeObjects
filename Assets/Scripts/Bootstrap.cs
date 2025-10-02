using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private ScoreCounter _scoreCounter;
    [SerializeField] private MergeObjectPool _pool;
    [SerializeField] private Spawner _spawner;
    [SerializeField] private Dropper _dropper;

    private void Start()
    {
        SDK.Instance.Init(() =>
        {
            SoundsPlayer.Instance.Init();
            _scoreCounter.LoadScores();
            _pool.Init();
            _spawner.LoadSpawned();
            _dropper.Next();
        });
    }
}
