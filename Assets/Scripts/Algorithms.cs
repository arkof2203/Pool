using UnityEngine;

public enum MoveAlgorithm
{
    Lerp,
    MoveTowards,
    RigidbodyVelocity
}

public static class Algorithms
{
    public static Vector3 MoveLerp(Vector3 from, Vector3 to, float speed, float dt)
    {
        float t = 1f - Mathf.Exp(-speed * dt);
        return Vector3.LerpUnclamped(from, to, t);
    }

    public static Vector3 MoveTowards(Vector3 from, Vector3 to, float speed, float dt)
    {
        return Vector3.MoveTowards(from, to, speed * dt);
    }

    public static Vector3 VelocityToTarget(Vector3 from, Vector3 to, float speed)
    {
        Vector3 dir = (to - from);
        float mag = dir.magnitude;
        if (mag < 0.0001f) return Vector3.zero;
        return dir / mag * speed;
    }
}