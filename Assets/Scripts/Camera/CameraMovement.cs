using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private Vector3 _defaultCameraPos = new(4.5f, 4.5f, -10);
    private Vector3 _originPos = new();
    private Vector3 _difference = new();
    private bool _isDragging = false;

    private const float MIN_ZOOM = 2;
    private const float MAX_ZOOM = 6;

    public bool IsMouseCanDrug { get; private set; } = false;

    private void Start()
    {
        transform.position = _defaultCameraPos;
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        DraggingUpdate();
        ZoomUpdate();
    }

    private void ZoomUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        float wheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (_virtualCamera.m_Lens.OrthographicSize - wheelValue < MIN_ZOOM) 
            _virtualCamera.m_Lens.OrthographicSize = MIN_ZOOM;
        else if (_virtualCamera.m_Lens.OrthographicSize - wheelValue > MAX_ZOOM)
            _virtualCamera.m_Lens.OrthographicSize = MAX_ZOOM;
        else _virtualCamera.m_Lens.OrthographicSize -= Input.GetAxis("Mouse ScrollWheel");
    }

    private void DraggingUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) _isDragging = false;
        else if (IsMouseCanDrug && Input.GetMouseButton(0))
        {
            _difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position);
            if (!_isDragging)
            {
                _isDragging = true;
                _originPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else transform.position = _originPos - _difference;
        }
        else if (IsMouseCanDrug) _isDragging = false;
    }

    public void SwitchDragOption() => IsMouseCanDrug = !IsMouseCanDrug;

    public void AdjustBounds(PolygonCollider2D collider)
    {
        GetComponent<CinemachineConfiner>().InvalidatePathCache();
        GetComponent<CinemachineConfiner>().m_BoundingShape2D = collider;
    }
}
