using UnityEngine;

[DisallowMultipleComponent]
public class MoveAgent : MonoBehaviour
{
    [Header("Optional physics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool useRigidbodyWhenPossible = true;

    public bool IsActive { get; private set; }
    public Vector3 Target { get; private set; }
    public float Speed { get; private set; }
    public float StopDistance { get; private set; }
    public MoveAlgorithm Algorithm { get; private set; }

    public System.Action<MoveAgent> OnReachedTarget;

    private Transform _tr;

    private void Awake()
    {
        _tr = transform;
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (rb != null) rb.velocity = Vector3.zero;
        IsActive = false;
    }

    public void Activate(Vector3 startPos, Vector3 target, float speed, float stopDistance, MoveAlgorithm algorithm)
    {
        _tr.position = startPos;
        Target = target;
        Speed = Mathf.Max(0.01f, speed);
        StopDistance = Mathf.Max(0.001f, stopDistance);
        Algorithm = algorithm;
        IsActive = true;

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public void Tick(float dt)
    {
        if (!IsActive) return;

        Vector3 pos = _tr.position;
        float dist = Vector3.Distance(pos, Target);
        if (dist <= StopDistance)
        {
            OnReachedTarget?.Invoke(this);
            return;
        }
        if (Algorithm == MoveAlgorithm.Lerp)
        {
            _tr.position = Algorithms.MoveLerp(pos, Target, Speed, dt);
        }
        else if (Algorithm == MoveAlgorithm.MoveTowards)
        {
            _tr.position = Algorithms.MoveTowards(pos, Target, Speed, dt);
        }
        else 
        {
            if (useRigidbodyWhenPossible && rb != null)
            {
                rb.velocity = Algorithms.VelocityToTarget(pos, Target, Speed);
            }
            else
            {
                _tr.position = Algorithms.MoveTowards(pos, Target, Speed, dt);
            }
        }
    }
}
