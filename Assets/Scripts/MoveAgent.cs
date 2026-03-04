using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    public event Action<MoveAgent> Reached;

    private bool hasDestination;

    private Vector3 _zoneCenter;
    private Vector3 _zoneSize;
    private bool _zoneAssigned;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void OnPooledGet()
    {
        if (agent != null) agent.enabled = true;
        hasDestination = false;
    }

    public void OnPooledRelease()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        hasDestination = false;
    }

    public void SetDespawnZone(Vector3 center, Vector3 size)
    {
        _zoneCenter = center;
        _zoneSize = size;
        _zoneAssigned = true;
    }

    public void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
        if (agent != null && agent.enabled)
        {
            agent.Warp(pos);
            agent.ResetPath();
            agent.isStopped = true;
        }
    }

    public void MoveTo(Vector3 worldPoint)
    {
        if (agent == null || !agent.enabled) return;
        agent.isStopped = false;
        agent.SetDestination(worldPoint);
        hasDestination = true;
    }

    private void Update()
    {
        if (!hasDestination || agent == null || !agent.enabled) return;
        if (!_zoneAssigned) return;
        if (IsInsideZoneXZ(transform.position, _zoneCenter, _zoneSize))
        {
            hasDestination = false;
            Reached?.Invoke(this);
        }
    }

    private static bool IsInsideZoneXZ(Vector3 p, Vector3 c, Vector3 size)
    {
        float hx = size.x * 0.5f;
        float hz = size.z * 0.5f;

        return p.x >= c.x - hx && p.x <= c.x + hx &&
               p.z >= c.z - hz && p.z <= c.z + hz;
    }
}
