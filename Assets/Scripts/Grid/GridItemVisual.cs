using UnityEngine;

public class GridItemVisual : MonoBehaviour
{
    public GameObject arrowPrefab;
    private LineRenderer lineRenderer;
    private GameObject topArrow, bottomArrow, leftArrow, rightArrow;

    public void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0, 1, 0, 0.5f); // Green with transparency
        lineRenderer.endColor = new Color(0, 1, 0, 0.5f);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 5; // Box outline (4 corners + closing point)
        lineRenderer.enabled = false; // Initially hidden

        // Create arrow objects using the prefab
        topArrow = CreateArrow("TopArrow");
        bottomArrow = CreateArrow("BottomArrow");
        leftArrow = CreateArrow("LeftArrow");
        rightArrow = CreateArrow("RightArrow");

        HideArrows(); // Initially hide the arrows
    }

    public void UpdateGridHighlight(Vector3 snappedPosition, Vector2Int itemSize, float tileSize)
    {
        // Calculate the bounds of the grid item
        Vector3 bottomLeft = snappedPosition - new Vector3(itemSize.x * tileSize / 2, 0, itemSize.y * tileSize / 2);
        Vector3 bottomRight = bottomLeft + new Vector3(itemSize.x * tileSize, 0, 0);
        Vector3 topRight = bottomRight + new Vector3(0, 0, itemSize.y * tileSize);
        Vector3 topLeft = bottomLeft + new Vector3(0, 0, itemSize.y * tileSize);

        // Update the box outline
        lineRenderer.SetPositions(new Vector3[] { bottomLeft, bottomRight, topRight, topLeft, bottomLeft });

        // Position and rotate the arrows around the grid item
        PositionArrows(snappedPosition, itemSize, tileSize);
    }

    public void ShowVisual(bool isVisible)
    {
        lineRenderer.enabled = isVisible;
        if (isVisible)
        {
            ShowArrows();
        }
        else
        {
            HideArrows();
        }
    }

    private GameObject CreateArrow(string name)
    {
        // Instantiate the arrow prefab
        GameObject arrow = Instantiate(arrowPrefab, transform);
        arrow.name = name;
        arrow.SetActive(false); // Initially hidden
        return arrow;
    }

    private void PositionArrows(Vector3 snappedPosition, Vector2Int itemSize, float tileSize)
    {
        // Get the collider of the GridItem
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogWarning("No collider found on the GridItem. Arrows will not be positioned correctly.");
            return;
        }

        // Get the bounds of the collider
        Bounds bounds = collider.bounds;

        // Calculate the size of the arrow's collider (assuming the arrow has a BoxCollider)
        BoxCollider arrowCollider = arrowPrefab.GetComponent<BoxCollider>();
        if (arrowCollider == null)
        {
            Debug.LogWarning("Arrow prefab does not have a BoxCollider. Arrows will not be positioned correctly.");
            return;
        }

        Vector3 arrowSize = arrowCollider.size;

        // Calculate the positions for the arrows based on the collider bounds
        Vector3 topPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.max.z + arrowSize.z / 2);
        Vector3 bottomPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.min.z - arrowSize.z / 2);
        Vector3 leftPosition = new Vector3(bounds.min.x - arrowSize.x / 2, bounds.min.y, bounds.center.z);
        Vector3 rightPosition = new Vector3(bounds.max.x + arrowSize.x / 2, bounds.min.y, bounds.center.z);

        // Position and rotate arrows on each side of the grid item
        PositionAndRotateArrow(topArrow, topPosition, Quaternion.Euler(0, 180, 0)); // Top (pointing outward)
        PositionAndRotateArrow(bottomArrow, bottomPosition, Quaternion.Euler(0, 0, 0)); // Bottom (pointing outward)
        PositionAndRotateArrow(leftArrow, leftPosition, Quaternion.Euler(0, 90, 0)); // Left (pointing outward)
        PositionAndRotateArrow(rightArrow, rightPosition, Quaternion.Euler(0, 270, 0)); // Right (pointing outward)
    }

    private void PositionAndRotateArrow(GameObject arrow, Vector3 position, Quaternion rotation)
    {
        arrow.transform.position = position;
        arrow.transform.rotation = rotation;
    }

    private void ShowArrows()
    {
        topArrow.SetActive(true);
        bottomArrow.SetActive(true);
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    private void HideArrows()
    {
        topArrow.SetActive(false);
        bottomArrow.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
    }
}