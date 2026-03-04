using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelectionController : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask selectableMask; 
    [SerializeField] private LayerMask groundMask; 

    [Header("NavMesh")]
    [SerializeField] private float sampleRadius = 2f;

    [Header("Keys")]
    [SerializeField] private KeyCode selectAllKey = KeyCode.A;

    [Header("Refs")]
    [SerializeField] private MovingGroupManager groupManager;

    private readonly List<Selectable> selected = new();

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (groupManager == null) groupManager = FindFirstObjectByType<MovingGroupManager>();
    }

    private void Update()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            leftClick = true;

        if (Input.GetKeyDown(selectAllKey))
            SelectAll();

        if (leftClick) HandleSelect();
        if (rightClick) HandleMoveCommand();
    }

    private void HandleSelect()
    {
        bool add = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (RayToLayer(selectableMask, out var hit))
        {
            var sel = hit.collider.GetComponentInParent<Selectable>();
            if (sel == null) return;

            if (!add) ClearSelection();

            if (!selected.Contains(sel))
            {
                selected.Add(sel);
                sel.SetSelected(true);
            }
        }
        else
        {
            if (!add) ClearSelection();
        }
    }

    private void HandleMoveCommand()
    {
        if (selected.Count == 0) return;
        if (!RayToLayer(groundMask, out var hit)) return;

        Vector3 target = hit.point;

        if (NavMesh.SamplePosition(target, out var navHit, sampleRadius, NavMesh.AllAreas))
            target = navHit.position;
        if (groupManager != null)
        {
            var c = groupManager.DespawnCenter;
            var s = groupManager.DespawnSize;

            if (!IsInsideZoneXZ(target, c, s))
            {
                return;
            }
        }

        for (int i = 0; i < selected.Count; i++)
        {
            var sel = selected[i];
            if (sel != null && sel.Agent != null && sel.gameObject.activeInHierarchy)
                sel.Agent.MoveTo(target);
        }
    }

    private bool RayToLayer(LayerMask mask, out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, 500f, mask);
    }

    private void SelectAll()
    {
        ClearSelection();

        var all = FindObjectsByType<Selectable>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            selected.Add(all[i]);
            all[i].SetSelected(true);
        }
    }

    private void ClearSelection()
    {
        for (int i = 0; i < selected.Count; i++)
            if (selected[i] != null) selected[i].SetSelected(false);

        selected.Clear();
    }

    private static bool IsInsideZoneXZ(Vector3 p, Vector3 c, Vector3 size)
    {
        float hx = size.x * 0.5f;
        float hz = size.z * 0.5f;

        return p.x >= c.x - hx && p.x <= c.x + hx &&
               p.z >= c.z - hz && p.z <= c.z + hz;
    }
}
