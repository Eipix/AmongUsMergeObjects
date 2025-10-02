using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeReference, ReadOnly] private List<MergeObject> _spawnedObjects = new List<MergeObject> ();
    [SerializeField] private FlyingScore _flyingScore;
    [SerializeField] private MergeObjectPool _pool;
    [SerializeField] private Dropper _dropper;
    [SerializeField] private Image _nextView;
    [SerializeField] private AudioClip _merge;

    public event UnityAction<int> OnMerged;
    public event UnityAction Win;

    private int _next = 0;

    public readonly string SaveKey = "ListMergeObjects";
    public readonly int MinRandomSpawnLevel = 1;
    public readonly int MaxRandomSpawnLevel = 5;

    public void LoadSpawned()
    {
        var serializedObject = SaveSerial.Instance.Load(SaveKey);

        if (serializedObject == null)
            return;

        IEnumerable<string> serializedData = ((IEnumerable)serializedObject).Cast<object>()
                                                                            .Select(data => data.ToString());

        foreach (var mergeData in serializedData)
        {
            if (mergeData == null || mergeData == string.Empty)
                continue;

            string[] data = mergeData.Split(";", StringSplitOptions.RemoveEmptyEntries);
            string[] serializedPosition = data[1].Split(":", StringSplitOptions.RemoveEmptyEntries);
            int level = int.Parse(data[0]);
            Vector2 position = new Vector2(serializedPosition[0].ParseFloat(), serializedPosition[1].ParseFloat());
            Add(mergeObject => mergeObject.Level == level, transform, position);
        }
    }

    public MergeObject SpawnRandom()
    {
        int randomLevel = GetRandomLevel();

        MergeObject current = _next == 0
            ? Add(mergeObj => mergeObj.Level == randomLevel, _dropper.SpawnPoint, _dropper.SpawnPoint.position)
            : Add(mergeObj => mergeObj.Level == _next, _dropper.SpawnPoint, _dropper.SpawnPoint.position);

        _next = GetRandomLevel();
        _nextView.sprite = _pool.Prefabs.Where(mergeObj => mergeObj.Level == _next).Select(mergeObj => mergeObj.Icon).First();
        current.PlayShowing();
        return current;
    }

    public void SpawnLevelUped(MergeObject thisMerge, MergeObject neighbour, Vector2 spawnPosition)
    {
        if (_pool.Contains(thisMerge) || _pool.Contains(neighbour))
            return;

        Remove(thisMerge);
        Remove(neighbour);

        if (CheckWin(thisMerge))
            return;

        Func<MergeObject, bool> condition = mergeObject => mergeObject.Level == thisMerge.Level + 1;
        var instance = Add(condition, transform, spawnPosition);
        instance.OnLevelUp();

        string text = $"+{instance.Score}";
        var position = new Vector2(instance.transform.position.x + instance.WorldRadius, instance.transform.position.y + instance.WorldRadius);
        _flyingScore.FlyText(text, position);

        SoundsPlayer.Instance.PlayOneShotSound(_merge);
        OnMerged?.Invoke(instance.Score);

        SaveSpawned();
    }

    public void Clear()
    {
        if (_dropper.Current != null && _spawnedObjects.Contains(_dropper.Current) == false)
            throw new InvalidOperationException($"{_dropper.Current.name} id {_dropper.Current.GetInstanceID()} dont contains in the spawned!");

        foreach (var spawned in _spawnedObjects)
        {
            if (_pool.Contains(spawned))
                throw new InvalidOperationException($"pool contains {spawned.name} id {spawned.GetInstanceID()}");
        }

        _pool.Put(_spawnedObjects.ToArray());
        _spawnedObjects.Clear();
        SaveSpawned();
    }

    [Button]
    private void InvokeWin() => Win?.Invoke();

    private bool CheckWin(MergeObject mergeObject)
    {
        if (mergeObject.Level >= _pool.MaxLevel)
        {
            Win?.Invoke();
            return true;
        }
        return false;
    }

    private void SaveSpawned()
    {
        var filtered = new List<MergeObject>(_spawnedObjects);
        filtered.Remove(_dropper.Current);
        var data = filtered.Select(mergeObject => mergeObject.Data);
        SaveSerial.Instance.Save(SaveKey, data);
        SDK.Instance.Save();
    }

    private int GetRandomLevel() => UnityEngine.Random.Range(MinRandomSpawnLevel, MaxRandomSpawnLevel + 1);

    private void Remove(MergeObject mergeObject)
    {
        _spawnedObjects.Remove(mergeObject);
        _pool.Put(mergeObject);
    }

    private MergeObject Add(Func<MergeObject, bool> predicate, Transform parent, Vector2 position = default)
    {
        var instance = _pool.TakeOrAdd(predicate, parent, position);

        if (_spawnedObjects.Contains(instance))
            throw new InvalidOperationException($"object {instance.name} id {instance.GetInstanceID()} already in the spawner!");

        _spawnedObjects.Add(instance);
        return instance;
    }
}
