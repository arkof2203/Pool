using System.Collections.Generic;
using UnityEngine;

public class MovingGroupManager : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private MoveAgent prefab;
    [SerializeField] private int preloadCount = 50;

    [Header("Spawn Test")]
    [SerializeField] private int spawnCount = 50;
    [SerializeField] private Vector3 areaCenter = Vector3.zero;
    [SerializeField] private Vector3 areaSize = new Vector3(20f, 0f, 20f);

    [Header("Move Settings")]
    [SerializeField] private MoveAlgorithm algorithm = MoveAlgorithm.MoveTowards;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private bool loopTargets = true;

    private ObjectsMover mover;

    private readonly Stack<MoveAgent> _pool = new Stack<MoveAgent>(256);
    private readonly HashSet<MoveAgent> _inUse = new HashSet<MoveAgent>();

    private MovingGroupAgent _group;

    public MoveAlgorithm Algorithm => algorithm;
    public float Speed => speed;
    public float StopDistance => stopDistance;
    public bool LoopTargets => loopTargets;

    private void Awake()
    {
        if (mover == null)
        {
            mover = FindFirstObjectByType<ObjectsMover>();
            if (mover == null)
                mover = new GameObject("ObjectsMover").AddComponent<ObjectsMover>();
        }

        _group = new MovingGroupAgent(this);

        Preload(preloadCount);
    }

    private void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var a = Get();
            _group.SetupAgent(a);
            mover.Register(a);
            _group.ActivateAgent(a);
        }
    }

    private void Preload(int count)
    {
        if (prefab == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var a = Instantiate(prefab, transform);
            a.gameObject.SetActive(false);
            _pool.Push(a);
        }
    }

    public MoveAgent Get()
    {
        MoveAgent a = (_pool.Count > 0) ? _pool.Pop() : Instantiate(prefab, transform);
        _inUse.Add(a);
        return a;
    }

    public void Release(MoveAgent a)
    {
        if (a == null) return;

        if (_inUse.Remove(a))
        {
            mover.Unregister(a);
            a.Deactivate();
            _pool.Push(a);
        }
    }

    public Vector3 RandomPoint()
    {
        float x = areaCenter.x + Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float y = areaCenter.y;
        float z = areaCenter.z + Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f);
        return new Vector3(x, y, z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
#endif
}
