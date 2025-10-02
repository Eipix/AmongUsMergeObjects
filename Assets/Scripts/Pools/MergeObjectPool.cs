using Sirenix.Utilities;
using System.Linq;
using UnityEngine;

public class MergeObjectPool : ObjectPool<MergeObject>
{
    [SerializeField] private Spawner _spawner;

    public int MaxLevel => Prefabs.Count;

    public override void Init()
    {
        base.Init();
        Prefabs = Prefabs.OrderBy(obj => obj.Level).ToList();
    }

    public override void Put(params MergeObject[] poolObjects)
    {
        base.Put(poolObjects);
        foreach (var poolObject in poolObjects)
            poolObject.Activate();
    }

    protected override void OnInstantiate(MergeObject poolObject)
    {
        base.OnInstantiate(poolObject);
        poolObject.Init(_spawner);
    }
}
