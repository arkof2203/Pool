using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingGroupManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectPoolBehaviour poolBehaviour;

    [Header("Counts")]
    [SerializeField] private int activeLimit = 20;

    [Header("Areas")]
    [SerializeField] private Vector3 spawnCenter = new Vector3(-10, 0, 0);
    [SerializeField] private Vector3 spawnSize = new Vector3(10, 0, 10);

    [SerializeField] private Vector3 despawnCenter = new Vector3(10, 0, 0);
    [SerializeField] private Vector3 despawnSize = new Vector3(10, 0, 10);

    public Vector3 DespawnCenter => despawnCenter;
    public Vector3 DespawnSize => despawnSize;

    private readonly List<MoveAgent> active = new();

    private void Awake()
    {
        if (poolBehaviour == null)
            poolBehaviour = FindFirstObjectByType<ObjectPoolBehaviour>();
    }

    private void Start()
    {
        for (int i = 0; i < activeLimit; i++)
            SpawnOneFromPool();
    }
    public void SpawnOneFromPool()
    {
        if (poolBehaviour == null || poolBehaviour.Pool == null) return;

        if (!poolBehaviour.Pool.TryGet(out var a) || a == null)
        {
            return;
        }

        a.Reached -= OnReached;
        a.Reached += OnReached;

        a.SetDespawnZone(despawnCenter, despawnSize);
        a.TeleportTo(RandomPointOnNavMesh(spawnCenter, spawnSize));

        active.Add(a);
    }

    private void OnReached(MoveAgent a)
    {
        Release(a);
        SpawnOneFromPool();

    }

    private void Release(MoveAgent a)
    {
        if (a == null) return;

        int idx = active.IndexOf(a);
        if (idx >= 0) active.RemoveAt(idx);

        a.Reached -= OnReached;
        poolBehaviour.Pool.Release(a);
    }

    private Vector3 RandomPointInArea(Vector3 c, Vector3 s)
    {
        float x = c.x + Random.Range(-s.x * 0.5f, s.x * 0.5f);
        float y = c.y;
        float z = c.z + Random.Range(-s.z * 0.5f, s.z * 0.5f);
        return new Vector3(x, y, z);
    }

    private Vector3 RandomPointOnNavMesh(Vector3 c, Vector3 s)
    {
        var p = RandomPointInArea(c, s);
        if (NavMesh.SamplePosition(p, out var hit, 5f, NavMesh.AllAreas))
            return hit.position;

        NavMesh.SamplePosition(c, out hit, 50f, NavMesh.AllAreas);
        return hit.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(despawnCenter, despawnSize);
    }
#endif
}