using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _root;
    private readonly Queue<T> _queue;
    private readonly Action<T> _onGet;
    private readonly Action<T> _onRelease;
    private readonly bool _allowGrowth;

    public int CountInactive => _queue.Count;

    public ObjectPool(
        T prefab,
        int preload,
        Transform root = null,
        Action<T> onGet = null,
        Action<T> onRelease = null,
        bool allowGrowth = true)
    {
        _prefab = prefab;
        _root = root;
        _onGet = onGet;
        _onRelease = onRelease;
        _allowGrowth = allowGrowth;
        _queue = new Queue<T>(Mathf.Max(1, preload));

        Preload(preload);
    }

    private void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var inst = UnityEngine.Object.Instantiate(_prefab, _root);
            inst.gameObject.SetActive(false);
            _queue.Enqueue(inst);
        }
    }

    public bool TryGet(out T item)
    {
        if (_queue.Count == 0)
        {
            item = null;
            return false;
        }

        item = _queue.Dequeue();
        item.gameObject.SetActive(true);
        _onGet?.Invoke(item);
        return true;
    }

    public T Get()
    {
        if (_queue.Count > 0)
        {
            var item = _queue.Dequeue();
            item.gameObject.SetActive(true);
            _onGet?.Invoke(item);
            return item;
        }

        if (!_allowGrowth)
        {
            return null;
        }

        var created = UnityEngine.Object.Instantiate(_prefab, _root);
        created.gameObject.SetActive(true);
        _onGet?.Invoke(created);
        return created;
    }

    public void Release(T item)
    {
        if (item == null) return;

        _onRelease?.Invoke(item);
        item.gameObject.SetActive(false);
        _queue.Enqueue(item);
    }
}