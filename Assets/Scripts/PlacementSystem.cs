using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField]
    private GameObject mouseIndicator;

    [SerializeField]
    private InputManager inputManager;
    
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private BuildingsDatabase buildingsDatabase;
    private int selectedBuildingIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private GridBuildingData buildingData;

    private List<GameObject> placedBuildings = new List<GameObject>();

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        StopPlacement();
        buildingData = new GridBuildingData();
    }
    
    private void Update()
    {
        if(selectedBuildingIndex < 0) {
            return;
        }
        
        var (gridPosition, mousePosition) = GetCurrentGridWorldToCellPosition();

        mouseIndicator.transform.position = mousePosition;

        if(lastDetectedPosition != gridPosition) {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedBuildingIndex);

            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacement(int id) 
    {
        selectedBuildingIndex = buildingsDatabase.Buildings.FindIndex(data => data.ID == id);

        if(selectedBuildingIndex < 0) 
        {
            Debug.Log($"no id found {id}");
            return;
        }

        gridVisualization.SetActive(true);
        preview.StartShowing(buildingsDatabase.Buildings[selectedBuildingIndex]);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()) {
            return;
        }

        var (gridPosition, mousePosition) = GetCurrentGridWorldToCellPosition();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedBuildingIndex);
        if(placementValidity == false) {
            return;
        }

        GameObject newBuilding = Instantiate(buildingsDatabase.Buildings[selectedBuildingIndex].Prefab);
        newBuilding.transform.position = grid.CellToWorld(gridPosition);

        placedBuildings.Add(newBuilding);

        buildingData.AddBuildingAt(gridPosition, buildingsDatabase.Buildings[selectedBuildingIndex], placedBuildings.Count - 1);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedBuildingIndex)
    {
        return buildingData.CanPlaceBuildingAt(gridPosition, buildingsDatabase.Buildings[selectedBuildingIndex].Size);
    }

    private void StopPlacement()
    {
        selectedBuildingIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowing();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private (Vector3Int gridPosition, Vector3 worldPosition) GetCurrentGridWorldToCellPosition() 
    {
        Vector3 worldPosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        gridPosition.y = 0;

        return (gridPosition, worldPosition);
    }

}