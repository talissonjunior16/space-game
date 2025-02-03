using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    public float minZoom = 3f;
    public float maxZoom = 14f;
    public float zoomSpeed = 5f;
    public float panSpeed = 2f;
    public float mobileZoomSpeed = 2f;
    public float mobilePanSpeed = 0.3f;

    [SerializeField]
    private bool isMobile = true;

    private float targetZoom;
    private Vector3 lastMousePosition;
    private Vector3 initialPosition;
    private bool isZooming;
    private bool canPan = true;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        targetZoom = cinemachineCamera.Lens.OrthographicSize;
        initialPosition = transform.position;
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float zoomChange;

        if (isMobile)
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                float prevDist = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
                float currentDist = (touch0.position - touch1.position).magnitude;

                zoomChange = prevDist - currentDist;
                targetZoom = Mathf.Clamp(targetZoom + zoomChange * 0.01f, minZoom, maxZoom);

                isZooming = true;
            }
            else
            {
                isZooming = false;
            }
        }
        else
        {
            zoomChange = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom + zoomChange, minZoom, maxZoom);
        }

        if (isZooming)
        {
            if(isMobile) {
                cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(
                    cinemachineCamera.Lens.OrthographicSize, 
                    targetZoom, 
                    Time.deltaTime * mobileZoomSpeed * 10f
                );
            }
            else {
                cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(
                    cinemachineCamera.Lens.OrthographicSize, 
                    targetZoom, 
                    Time.deltaTime * zoomSpeed
                );
            }
        }
        else
        {
            cinemachineCamera.Lens.OrthographicSize = targetZoom;
        }

        // If zoom is at max, reset position and disable panning
        if (cinemachineCamera.Lens.OrthographicSize >= maxZoom - 0.01f)
        {
            transform.position = initialPosition;
            canPan = false;
        }
        else
        {
            canPan = true; // Allow panning at min zoom now
        }
    }

    void HandlePan()
    {
        if (!canPan) return; // Block only if at max zoom

        if (isMobile)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 move = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0) * mobilePanSpeed * Time.deltaTime;
                    transform.position -= move;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                lastMousePosition = Input.mousePosition;

            if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;
                transform.position += move;
                lastMousePosition = Input.mousePosition;
            }
        }
    }
}
