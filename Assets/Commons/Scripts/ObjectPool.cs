using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeReference] private List<T> _prefabs = new List<T>();

    public List<T> Prefabs
    {
        get => _prefabs;
        protected set => _prefabs = value;
    }

    private List<T> _pool = new List<T>();

    protected IEnumerable<T> Pool => _pool;

    public virtual void Init()
    {
        foreach (var poolObject in _prefabs)
        {
            var instance = Instantiate(poolObject, transform);
            OnInstantiate(instance);
            instance.gameObject.SetActive(false);
            _pool.Add(instance);
        }
    }

    protected virtual void OnInstantiate(T poolObject) { }

    public bool Contains(T poolObject) => _pool.Contains(poolObject);

    public T Take(Transform parent, Vector2 position)
    {
        if (TryGetFirstOrDefault(out T poolObject))
        {
            Set(poolObject, parent, position); 
            _pool.Remove(poolObject);
            return poolObject;
        }
        else
        {
            return OnEmpty();
        }
    }

    public T Take(Func<T, bool> predicate, Transform parent, Vector2 position = default, Quaternion rotation = default)
    {
        if (TryGet(predicate, out T poolObject))
        {
            Set(poolObject, parent, position, rotation);
            _pool.Remove(poolObject);
            return poolObject;
        }
        else
        {
            return OnEmpty();
        }
    }

    public T TakeOrAdd(Func<T, bool> predicate, Transform parent, Vector2 position = default, Quaternion rotation = default)
    {
        if (TryGet(predicate, out T poolObject))
        {
            Set(poolObject, parent, position, rotation);
            _pool.Remove(poolObject);
            return poolObject;
        }
        else
        {
            Add(predicate);
            return TakeOrAdd(predicate, parent, position, rotation);
        }
    }

    public T TakeOrAdd(Transform parent = default, Vector2 position = default, Quaternion rotation = default)
    {
        if (TryGetFirstOrDefault(out T poolObject))
        {
            Set(poolObject, parent, position, rotation);
            _pool.Remove(poolObject);
            return poolObject;
        }
        else
        {
            Add();
            return TakeOrAdd(parent, position, rotation);
        }
    }

    public virtual void Put(params T[] poolObjects)
    {
        foreach (var poolObject in poolObjects)
        {
            if (_pool.Contains(poolObject))
                throw new InvalidOperationException($"Object {poolObject.name} with id {poolObject.GetInstanceID()} has already contains in the pool!");

            poolObject.transform.SetParent(transform);
            poolObject.gameObject.SetActive(false);
            poolObject.transform.position = transform.position;
            poolObject.transform.rotation = Quaternion.identity;
            _pool.Add(poolObject);
        }
    }

    protected virtual T OnEmpty() => throw new InvalidOperationException($"Not enough object of type: {_pool.FirstOrDefault().GetType()} in pool.");

    protected virtual void Set(T poolObject, Transform parent = null, Vector2 position = default, Quaternion rotation = default)
    {
        poolObject.transform.position = position;
        poolObject.transform.rotation = rotation;

        if (parent != null)
            poolObject.transform.SetParent(parent);

        poolObject.gameObject.SetActive(true);
    }

    private T Add(Func<T, bool> predicate)
    {
        var instance = Instantiate(_prefabs.First(predicate), transform);
        instance.gameObject.SetActive(false);
        _pool.Add(instance);
        OnInstantiate(instance);
        return instance;
    }

    private T Add()
    {
        var instance = Instantiate(_prefabs.First(), transform);
        instance.gameObject.SetActive(false);
        _pool.Add(instance);
        OnInstantiate(instance);
        return instance;
    }

    private bool TryGetFirstOrDefault(out T poolObject)
    {
        poolObject = _pool.FirstOrDefault();
        return poolObject != null;
    }

    private bool TryGet(Func<T, bool> condition, out T poolObject)
    {
        poolObject = _pool.Where(condition).FirstOrDefault();
        return poolObject != null;
    }
}
