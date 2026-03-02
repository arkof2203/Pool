using System.Collections.Generic;
using UnityEngine;

public class ObjectsMover : MonoBehaviour
{
    private readonly List<MoveAgent> _agents = new List<MoveAgent>(256);

    public void Register(MoveAgent agent)
    {
        if (agent == null) return;
        if (!_agents.Contains(agent)) _agents.Add(agent);
    }

    public void Unregister(MoveAgent agent)
    {
        if (agent == null) return;
        _agents.Remove(agent);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < _agents.Count; i++)
        {
            var a = _agents[i];
            if (a != null && a.IsActive)
                a.Tick(dt);
        }
    }
}
