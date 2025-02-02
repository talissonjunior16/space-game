using System;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingData
{
    Dictionary<Vector3Int, GridBuildingPlacementData> placedBuildings = new();

    public void AddBuildingAt(
        Vector3Int gridPosition,
        Building building,
        int buildingIndex
    ) {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, building.Size);
        GridBuildingPlacementData data = new GridBuildingPlacementData(positionsToOccupy, building.ID, buildingIndex);

        foreach (var pos in positionsToOccupy)
        {
            if(placedBuildings.ContainsKey(pos)) {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }

            placedBuildings[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int buildingSize)
    {
        List<Vector3Int> returnValues = new();

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                returnValues.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnValues;
    }

    public bool CanPlaceBuildingAt(Vector3Int gridPosition, Vector2Int buildingSize) 
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, buildingSize);

        foreach (var pos in positionsToOccupy)
        {
            if(placedBuildings.ContainsKey(pos)) {
                return false;
            }
        }

        return true;
    }
}
