using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int gridSize = 44;
    public float tileSize = 1.0f;
    public List<GameObject> tilePrefabs; // List of tile prefabs
    public Transform tilesParent;
    public List<GameObject> rightSidePrefabs; // List of prefabs for the right side
    public float prefabsEdgesOffset = 0.5f; // New offset for edges
    private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    private Transform tilesContainer; // New parent for all tiles
    private Transform rightSidePrefabsContainer; // New parent for all right side prefabs

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
        /*if (tiles.Count == 0)
        {
            GenerateTiles();
        }*/
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

        // Place right side prefabs inside their container
        PlaceSidePrefabs();
    }

    public void PlaceSidePrefabs()
    {
        // Create containers for all side prefabs if they don't already exist
        CreatePrefabContainer("TopSidePrefabs");
        CreatePrefabContainer("BottomSidePrefabs");
        CreatePrefabContainer("LeftSidePrefabs");
        CreatePrefabContainer("RightSidePrefabs");

        // Place prefabs on the right side (already covered)
        /*PlaceRightSidePrefabs();*/

        // Place prefabs on the left side
        //PlaceLeftSidePrefabs();

        // Place prefabs on the top side
        PlaceTopSidePrefabs();

        // Place prefabs on the bottom side
        /*PlaceBottomSidePrefabs();*/
    }

    private void CreatePrefabContainer(string name)
    {
        Transform container = tilesParent.Find(name);
        if (container != null)
        {
            if (Application.isPlaying)
            {
                Destroy(container.gameObject);
            }
            else
            {
                DestroyImmediate(container.gameObject);
            }
        }
        container = new GameObject(name).transform;
        container.SetParent(tilesParent);
    }

    private void PlaceRightSidePrefabs()
    {
        Transform rightSidePrefabsContainer = tilesParent.Find("RightSidePrefabs");
        int rightSideX = gridSize - 1; // Rightmost column (X = gridSize - 1)
        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;

        // Iterate through each row (Y axis)
        for (int y = 0; y < gridSize; y++)
        {
            int numCells = Random.Range(1, 3); // Randomly choose 1 or 2 cells

            // Ensure we don't exceed the bottom boundary of the right side
            if (numCells == 2 && rightSideX + numCells >= gridSize)
            {
                numCells = 1;
            }

            // Choose a random prefab from the rightSidePrefabs list
            int prefabIndex = Random.Range(0, rightSidePrefabs.Count);
            GameObject prefab = rightSidePrefabs[prefabIndex];

            // Place the prefab on the right side and apply rotation
            for (int i = 0; i < numCells; i++)
            {
                Vector3 position = new Vector3((rightSideX * tileSize - offset) + prefabsEdgesOffset, 0, (y * tileSize - offset) + prefabsEdgesOffset);
                GameObject placedPrefab = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0), rightSidePrefabsContainer);
                placedPrefab.name = $"RightSidePrefab_{rightSideX}_{y}";
            }
        }
    }

    private void PlaceLeftSidePrefabs()
    {
        Transform leftSidePrefabsContainer = tilesParent.Find("LeftSidePrefabs");
        int leftSideX = 0;
        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;

        for (int y = 0; y < gridSize; y++)
        {
            int numCells = Random.Range(1, 3); // 1 or 2 cells

            // Ensure we don't exceed the right boundary of the left side
            if (numCells == 2 && leftSideX + numCells >= gridSize)
            {
                numCells = 1;
            }

            // Choose a random prefab from the leftSidePrefabs list
            int prefabIndex = Random.Range(0, rightSidePrefabs.Count);
            GameObject prefab = rightSidePrefabs[prefabIndex];

            // Place the prefab on the left side and rotate it
            for (int i = 0; i < numCells; i++)
            {
                Vector3 position = new Vector3((leftSideX * tileSize - offset) - prefabsEdgesOffset, 0, (y * tileSize - offset) + prefabsEdgesOffset);
                GameObject placedPrefab = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0), leftSidePrefabsContainer);
                placedPrefab.name = $"LeftSidePrefab_{leftSideX}_{y}";
            }
        }
    }

    private void PlaceTopSidePrefabs()
    {
        Transform topSidePrefabsContainer = tilesParent.Find("TopSidePrefabs");
        int topSideY = gridSize - 1;
        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;

        for (int x = 0; x < gridSize; x++)
        {
            int numCells = Random.Range(1, 3); // 1 or 2 cells

            // Ensure we don't exceed the bottom boundary of the top side
            if (numCells == 2 && topSideY - numCells < 0)
            {
                numCells = 1;
            }

            // Choose a random prefab from the topSidePrefabs list
            int prefabIndex = Random.Range(0, rightSidePrefabs.Count);
            GameObject prefab = rightSidePrefabs[prefabIndex];

            // Place the prefab on the top side and rotate it
            for (int i = 0; i < numCells; i++)
            {
                Vector3 position = new Vector3((x * tileSize - offset) + prefabsEdgesOffset, 0, (topSideY * tileSize - offset) - prefabsEdgesOffset);

                if(i > 0) {
                    position.z -= 0.8f * i;
                }

                GameObject placedPrefab = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0), topSidePrefabsContainer);
                placedPrefab.name = $"TopSidePrefab_{x}_{topSideY}";
            }
        }
    }

    private void PlaceBottomSidePrefabs()
    {
        Transform bottomSidePrefabsContainer = tilesParent.Find("BottomSidePrefabs");
        int bottomSideY = 0;
        float offset = (gridSize * tileSize) / 2.0f - tileSize / 2.0f;

        for (int x = 0; x < gridSize; x++)
        {
            int numCells = Random.Range(1, 3); // 1 or 2 cells

            // Ensure we don't exceed the top boundary of the bottom side
            if (numCells == 2 && bottomSideY + numCells >= gridSize)
            {
                numCells = 1;
            }

            // Choose a random prefab from the bottomSidePrefabs list
            int prefabIndex = Random.Range(0, rightSidePrefabs.Count);
            GameObject prefab = rightSidePrefabs[prefabIndex];

            // Place the prefab on the bottom side and rotate it
            for (int i = 0; i < numCells; i++)
            {
                Vector3 position = new Vector3((x * tileSize - offset) + prefabsEdgesOffset, 0, (bottomSideY * tileSize - offset) + prefabsEdgesOffset);
                GameObject placedPrefab = Instantiate(prefab, position, Quaternion.Euler(0, Random.Range(0f, 360f), 0), bottomSidePrefabsContainer);
                placedPrefab.name = $"BottomSidePrefab_{x}_{bottomSideY}";
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
