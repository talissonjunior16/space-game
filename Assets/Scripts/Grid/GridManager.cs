using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int gridSize = 44;
    public float tileSize = 1.0f;
    public List<GameObject> tilePrefabs; // List of tile prefabs
    public Transform tilesParent;
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

}