using System;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject validCellIndicatorPrefab;
    [SerializeField]
    private GameObject invalidCellIndicatorPrefab;
    
    private GameObject previewPrefab;
    private Building previewBuilding;
    private List<GameObject> validCellIndicatorInstances = new();
    private List<GameObject> invalidCellIndicatorInstances = new();

    private void Start() {
        validCellIndicatorPrefab.SetActive(false);
        invalidCellIndicatorPrefab.SetActive(false);
    }

    public void StartShowing(Building building) {
        previewBuilding = building;
        previewPrefab = Instantiate(building.Prefab);
        GenerateCellIndicators(building.Size);
    }

    public void StopShowing() {
        Destroy(previewPrefab);
        ClearCellIndicators();
    }

    public void UpdatePosition(Vector3 position, bool validity) {
        MovePreview(position);
        MoveCursor(position, validity);
    }

    private void MoveCursor(Vector3 position, bool validity)
    {
        List<GameObject> activeList = validity ? validCellIndicatorInstances : invalidCellIndicatorInstances;
        List<GameObject> inactiveList = validity ? invalidCellIndicatorInstances : validCellIndicatorInstances;

        // Enable the correct set of indicators, disable the others
        foreach (var cell in activeList) cell.SetActive(true);
        foreach (var cell in inactiveList) cell.SetActive(false);

        // Positioning logic
        Vector3 basePosition = position;
        float tileSize = 1f; // Adjust according to your grid size
        int index = 0;

        for (int x = 0; x < previewBuilding.Size.x; x++)
        {
            for (int y = 0; y < previewBuilding.Size.y; y++)
            {
                if (index < activeList.Count)
                {
                    activeList[index].transform.position = basePosition + new Vector3(x * -tileSize, 0, y * -tileSize);
                    index++;
                }
            }
        }
    }

    private void MovePreview(Vector3 position)
    {
        previewPrefab.transform.position = position;
    }

    private void GenerateCellIndicators(Vector2Int size)
    {
        ClearCellIndicators();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                GameObject validCell = Instantiate(validCellIndicatorPrefab, transform);
                validCell.SetActive(true);
                validCellIndicatorInstances.Add(validCell);

                GameObject invalidCell = Instantiate(invalidCellIndicatorPrefab, transform);
                invalidCell.SetActive(true);
                invalidCellIndicatorInstances.Add(invalidCell);
            }
        }
    }

    private void ClearCellIndicators()
    {
        foreach (var cell in validCellIndicatorInstances)
            Destroy(cell);
        validCellIndicatorInstances.Clear();

        foreach (var cell in invalidCellIndicatorInstances)
            Destroy(cell);
        invalidCellIndicatorInstances.Clear();
    }
}
