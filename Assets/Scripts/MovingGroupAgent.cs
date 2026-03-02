using UnityEngine;

public class MovingGroupAgent
{
    private readonly MovingGroupManager _mgr;

    public MovingGroupAgent(MovingGroupManager mgr)
    {
        _mgr = mgr;
    }

    public void SetupAgent(MoveAgent agent)
    {
        agent.OnReachedTarget = OnReached;
    }

    public void ActivateAgent(MoveAgent agent)
    {
        Vector3 start = _mgr.RandomPoint();
        Vector3 target = _mgr.RandomPoint();
        agent.Activate(start, target, _mgr.Speed, _mgr.StopDistance, _mgr.Algorithm);
    }

    private void OnReached(MoveAgent agent)
    {
        if (_mgr.LoopTargets)
        {
            agent.Activate(agent.transform.position, _mgr.RandomPoint(), _mgr.Speed, _mgr.StopDistance, _mgr.Algorithm);
            return;
        }

        _mgr.Release(agent);
    }
}
