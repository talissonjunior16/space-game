using System.Collections.Generic;
using UnityEngine;

public class GridBuildingPlacementData
{
    public List<Vector3Int> OccupiedPositions { get; set; }
    public int Id { get; set; }
    public int PlacedBuildingIndex { get; set; }

    public GridBuildingPlacementData(List<Vector3Int> occupiedPositions, int id, int placedBuildingIndex)
    {
        OccupiedPositions = occupiedPositions;
        Id = id;
        PlacedBuildingIndex = placedBuildingIndex;
    }
}
