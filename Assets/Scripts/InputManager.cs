using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    [SerializeField]
    private LayerMask placementLayermask;

    private Vector3 lastPosition;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PlacementSystem.Instance.StartPlacement(1);

        if(Input.GetKeyDown(KeyCode.Alpha2))
            PlacementSystem.Instance.StartPlacement(2);

        if(Input.GetKeyDown(KeyCode.Alpha3))
            PlacementSystem.Instance.StartPlacement(3);

        if(Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}