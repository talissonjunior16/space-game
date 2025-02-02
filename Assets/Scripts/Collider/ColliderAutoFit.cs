using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ColliderAutoFit : MonoBehaviour
{
    public enum ColliderType
    {
        BoxCollider,
        CapsuleCollider
    }

    public ColliderType collisionType; // Choose between BoxCollider or CapsuleCollider
    public List<GameObject> referenceObjects = new List<GameObject>(); // List of reference objects

    public void ApplyCollider()
    {
        if (referenceObjects == null || referenceObjects.Count == 0)
        {
            Debug.LogWarning("Reference objects list is empty!");
            return;
        }

        // Calculate the combined bounds of all reference objects
        Bounds combinedBounds = CalculateCombinedBounds();

        // Apply the selected collider
        switch (collisionType)
        {
            case ColliderType.BoxCollider:
                ApplyBoxCollider(combinedBounds);
                break;
            case ColliderType.CapsuleCollider:
                ApplyCapsuleCollider(combinedBounds);
                break;
            default:
                Debug.LogError("Invalid collider type selected!");
                break;
        }

#if UNITY_EDITOR
        // Mark the prefab or scene object as dirty to ensure changes are saved
        MarkPrefabDirty();
#endif
    }

    private Bounds CalculateCombinedBounds()
    {
        Bounds combinedBounds = new Bounds();
        bool hasValidRenderer = false;

        foreach (GameObject obj in referenceObjects)
        {
            if (obj == null) continue;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (!hasValidRenderer)
                {
                    combinedBounds = renderer.bounds;
                    hasValidRenderer = true;
                }
                else
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }
            }
        }

        if (!hasValidRenderer)
        {
            Debug.LogWarning("None of the reference objects have a valid Renderer component!");
        }

        return combinedBounds;
    }

    private void ApplyBoxCollider(Bounds bounds)
    {
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Set the size and center of the BoxCollider based on the combined bounds
        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center - transform.position;
    }

    private void ApplyCapsuleCollider(Bounds bounds)
    {
        CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        }

        // Calculate dimensions for the CapsuleCollider
        float radius = Mathf.Min(bounds.size.x, bounds.size.z) / 2;
        float height = bounds.size.y;

        capsuleCollider.radius = radius;
        capsuleCollider.height = height;
        capsuleCollider.center = bounds.center - transform.position;
    }

#if UNITY_EDITOR
    private void MarkPrefabDirty()
    {
        PrefabUtility.RecordPrefabInstancePropertyModifications(this); // Records changes
        EditorUtility.SetDirty(gameObject); // Marks the prefab instance as dirty
    }
#endif
}