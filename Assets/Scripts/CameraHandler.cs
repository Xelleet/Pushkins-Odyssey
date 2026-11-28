using System;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Цель")]
    [Tooltip("Объект, вокруг которого вращается камера")]
    [SerializeField] private Transform target;

    [Tooltip("Смещение точки вращения относительно центра цели (например, по высоте)")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1f, 0f);

    [Header("Орбита")]
    [Tooltip("Расстояние от камеры до цели")]
    [SerializeField] private float distance = 5f;

    [Tooltip("Чувствительность вращения мышью")]
    [SerializeField] private float sensitivity = 3f;

    [Tooltip("Сглаживание движения (0 — без сглаживания)")]
    [SerializeField] private float smoothing = 10f;

    [Header("Ограничения по вертикали (pitch)")]
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Зум колёсиком")]
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    [Header("Блокировка курсора")]
    [SerializeField] private bool lockCursor = true;

    [SerializeField] private InputHandler inputHandler;

    // Текущие углы орбиты
    private float _yaw;
    private float _pitch = 20f;

    // Целевые (для сглаживания) значения
    private float _targetYaw;
    private float _targetPitch;
    private float _targetDistance;


    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("OrbitCamera: не задан target!");
            enabled = false;
            return;
        }

        _targetDistance = distance;

        // Инициализируем углы из текущего положения камеры,
        // чтобы не было "прыжка" при старте
        Vector3 dir = transform.position - GetPivot();
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(-dir.normalized);
            Vector3 euler = look.eulerAngles;
            _targetYaw = euler.y;
            _targetPitch = NormalizeAngle(euler.x);
        }

        _yaw = _targetYaw;
        _pitch = _targetPitch;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleCursor();

        // --- Ввод мыши ---
        float mouseX = inputHandler.CameraRotation.x * sensitivity;
        float mouseY = inputHandler.CameraRotation.y * sensitivity;

        _targetYaw += mouseX;
        _targetPitch -= mouseY;
        _targetPitch = Mathf.Clamp(_targetPitch, minPitch, maxPitch);

        // --- Зум ---
        //if (enableZoom)
        //{
        //    float scroll = Input.GetAxis("Mouse ScrollWheel");
        //    if (Mathf.Abs(scroll) > 0.0001f)
        //    {
        //        _targetDistance -= scroll * zoomSpeed;
        //        _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);
        //    }
        //}

        // --- Сглаживание ---
        float t = smoothing > 0f ? 1f - Mathf.Exp(-smoothing * Time.deltaTime) : 1f;
        _yaw = Mathf.LerpAngle(_yaw, _targetYaw, t);
        _pitch = Mathf.LerpAngle(_pitch, _targetPitch, t);
        distance = Mathf.Lerp(distance, _targetDistance, t);

        // --- Позиционирование камеры ---
        Vector3 pivot = GetPivot();
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);

        transform.position = pivot + offset;

        // Камера всегда смотрит на цель (учитываем смещение точки вращения)
        transform.LookAt(pivot);
    }

    /// <summary>Точка, вокруг которой вращается камера.</summary>
    private Vector3 GetPivot() => target.position + targetOffset;

    public Quaternion CameraRotation()
    {
        return transform.rotation;
    }

    private void HandleCursor()
    {
        //if (!lockCursor) return;
//
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
        //else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    // Удобно видеть орбиту в редакторе
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetPivot(), 0.15f);
        Gizmos.DrawLine(GetPivot(), transform.position);
    }
}