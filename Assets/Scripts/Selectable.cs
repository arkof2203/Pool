using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionMarker;

    public bool IsSelected { get; private set; }
    public MoveAgent Agent { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<MoveAgent>();
        SetSelected(false);
    }

    public void SetSelected(bool value)
    {
        IsSelected = value;
        if (selectionMarker != null)
            selectionMarker.SetActive(value);
    }
}