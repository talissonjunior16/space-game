using UnityEngine;

public class GridItem : MonoBehaviour
{
    public Vector2Int itemSize = new Vector2Int(2, 2); // Size of the item on the grid (e.g., 2x2 tiles)
    private bool isDragging = false;
    private bool isEditMode = false; // Track whether the item is in edit mode
    private bool isFirstClick = false;
    private Vector3 offset;
    private GridItemVisual gridItemVisual;
    private Vector3 currentGridPosition;

    private void Start()
    {
        // Check if the visual component already exists, if not, create it
        gridItemVisual = GetComponent<GridItemVisual>();
        if (gridItemVisual == null)
        {
            gridItemVisual = gameObject.AddComponent<GridItemVisual>();
        }

        gridItemVisual.SetupLineRenderer();
    }

    private void OnMouseDown()
    {
        if (!isDragging)
        {
            ToggleEditMode(); // Toggle edit mode when clicked
        }
    }

    private void OnMouseDrag()
    {
        if (isEditMode) // Only allow dragging in edit mode
        {
            isDragging = true;
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            Vector3 snappedPosition = GetSnappedPosition(newPosition);

            if (snappedPosition != currentGridPosition) // Only update if position changes
            {
                currentGridPosition = snappedPosition;
                transform.position = snappedPosition;
                gridItemVisual.UpdateGridHighlight(snappedPosition, itemSize, GridManager.Instance.tileSize);
            }
        }
    }

    private void OnMouseUp()
    {
        if (isEditMode && isFirstClick)
        {
            isFirstClick = false;
            return;
        }

        isDragging = false;
        isEditMode = false;
        isFirstClick = false;
        gridItemVisual.ShowVisual(false); // Hide the visual when dragging ends
    }

    private void ToggleEditMode()
    {
        if (isEditMode && !isDragging)
        {
            return;
        }

        isEditMode = !isEditMode; // Toggle edit mode
        if (isEditMode)
        {
            isFirstClick = true;
            isDragging = true;
            gridItemVisual.ShowVisual(true); // Show the visual when edit mode is activated
        }
        else
        {
            isDragging = false;
            gridItemVisual.ShowVisual(false); // Hide the visual when edit mode is deactivated
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y - transform.position.y;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private Vector3 GetSnappedPosition(Vector3 position)
    {
        // Calculate the snapped position based on the item size
        float tileSize = GridManager.Instance.tileSize;
        float offsetX = (itemSize.x * tileSize) / 2f;
        float offsetZ = (itemSize.y * tileSize) / 2f;

        // Snap to the nearest grid position that aligns with the item size
        int x = Mathf.RoundToInt((position.x + offsetX) / (itemSize.x * tileSize)) * (itemSize.x);
        int z = Mathf.RoundToInt((position.z + offsetZ) / (itemSize.y * tileSize)) * (itemSize.y);

        // Adjust for the grid's origin
        float gridOffset = (GridManager.Instance.gridSize * tileSize) / 2f - tileSize / 2f;
        return new Vector3(x * tileSize - gridOffset, 0, z * tileSize - gridOffset);
    }
}