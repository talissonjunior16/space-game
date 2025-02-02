using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // Singleton instance

    [SerializeField] private SoundClipData soundAssets;

    private void Awake()
    {
        // Check if another instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign the singleton instance
        Instance = this;

        // Optional: Persist this object across scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (soundAssets == null)
        {
            Debug.LogError("SoundManager: No SoundAssets reference assigned!");
            return;
        }
    }

    public void PlayMoveBuildingSound(Vector3 position)
    {
        PlaySound(soundAssets.OnMoveBuilding, position);
    }

    public void PlayOnPlaceBuildingSound(Vector3 position)
    {
        PlaySound(soundAssets.OnPlaceBuilding, position);
    }
    
    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Attempted to play a sound, but the clip is null!");
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position);
    }
}
