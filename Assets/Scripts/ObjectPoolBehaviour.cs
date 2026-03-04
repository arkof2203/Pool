using UnityEngine;

public class ObjectPoolBehaviour : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private MoveAgent prefab;
    [SerializeField] private int poolSize = 40;

    public ObjectPool<MoveAgent> Pool { get; private set; }

    private void Awake()
    {
        Pool = new ObjectPool<MoveAgent>(
            prefab,
            poolSize,
            root: transform,
            onGet: a => a.OnPooledGet(),
            onRelease: a => a.OnPooledRelease(),
            allowGrowth: false
        );
    }
}
