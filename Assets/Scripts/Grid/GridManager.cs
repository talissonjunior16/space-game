using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int gridSize = 44;
    public float tileSize = 1.0f;
    public List<GameObject> tilePrefabs; // List of tile prefabs
    public Transform tilesParent;
    public Color gridGizmoColor = new Color(0, 1, 0, 0.3f);

    private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    private Transform tilesContainer; // New parent for all tiles

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (tiles.Count == 0)
        {
            GenerateTiles();
        }
    }

    public void GenerateTiles()
    {
        tiles.Clear(); // Clear dictionary before regenerating

        tilesContainer = tilesParent.Find("Tiles");

        // Ensure tilesContainer exists inside tilesParent
        if (tilesContainer != null)
        {
            if (Application.isPlaying)
            {
                Destroy(tilesContainer.gameObject);
            }
            else
            {
                DestroyImmediate(tilesContainer.gameObject);
            }
        }

        tilesContainer = new GameObject("Tiles").transform;
        tilesContainer.SetParent(tilesParent);

        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x * tileSize - offset, 0, y * tileSize - offset);
                Vector2Int gridPos = new Vector2Int(x, y);

                // Calculate the index of the tile prefab to use
                int prefabIndex = (x + y) % tilePrefabs.Count;

                GameObject tile = Instantiate(tilePrefabs[prefabIndex], position, Quaternion.identity, tilesContainer);
                tile.name = $"Tile_{x}_{y}";
                tiles[gridPos] = tile;
            }
        }
    }

    public Vector3 GetNearestGridPosition(Vector3 position)
    {
        if (tiles.Count == 0)
        {
            GenerateTiles();
        }

        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;
        int x = Mathf.RoundToInt((position.x + offset) / tileSize);
        int y = Mathf.RoundToInt((position.z + offset) / tileSize);

        x = Mathf.Clamp(x, 0, gridSize - 1);
        y = Mathf.Clamp(y, 0, gridSize - 1);

        Vector2Int gridPos = new Vector2Int(x, y);
        if (tiles.TryGetValue(gridPos, out GameObject tile))
        {
            return tile.transform.position;
        }

        return new Vector3(x * tileSize - offset, 0, y * tileSize - offset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridGizmoColor;

        if (tiles.Count == 0)
        {
            // Try to repopulate tiles from the scene if they're missing
            RepopulateTilesFromScene();
        }

        foreach (var tile in tiles)
        {
            Vector3 tilePos = tile.Value.transform.position;
            Vector3 bottomLeft = tilePos - new Vector3(tileSize / 2f, 0, tileSize / 2f);
            Vector3 bottomRight = tilePos + new Vector3(tileSize / 2f, 0, -tileSize / 2f);
            Vector3 topLeft = tilePos + new Vector3(-tileSize / 2f, 0, tileSize / 2f);
            Vector3 topRight = tilePos + new Vector3(tileSize / 2f, 0, tileSize / 2f);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }

    private void RepopulateTilesFromScene()
    {
        // Find the Tiles parent object in the scene
        Transform tilesParentTransform = tilesParent.Find("Tiles");

        if (tilesParentTransform == null)
        {
            Debug.LogWarning("Tiles parent not found in the scene.");
            return;
        }

        // Loop through all children of the Tiles parent to populate the tiles dictionary
        foreach (Transform tileTransform in tilesParentTransform)
        {
            // Ensure the tile prefab's name follows the same naming convention
            Vector2Int gridPos = ParseTilePosition(tileTransform.name);

            if (!tiles.ContainsKey(gridPos))
            {
                tiles[gridPos] = tileTransform.gameObject;
            }
        }
    }

    private Vector2Int ParseTilePosition(string tileName)
    {
        // Parse the tile's name (Tile_X_Y)
        string[] parts = tileName.Split('_');
        if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
        {
            return new Vector2Int(x, y);
        }

        return Vector2Int.zero; // Default value if the name format is incorrect
    }
}