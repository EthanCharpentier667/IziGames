using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Offset from the target character's position")]
    public Vector3 offset = new Vector3(0, 2, -4);
    public float smoothSpeed = 5f;
    public float sensX = 120f;
    public float sensY = 120f;
    public float minY = -40f;
    public float maxY = 80f;

    [Header("Player Rotation")]
    public bool rotatePlayerWithCamera = true;
    public float playerRotationSpeed = 10f;

    private Transform target;
    private float yaw = 0f;
    private float pitch = 10f;

    private InputAction lookAction;

    [Header("Camera Inversion")]
    public bool invertX = false;
    public bool invertY = false;

    void Start()
    {
        foreach (var character in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            if (character.isPlayer)
            {
                target = character.transform;
                break;
            }
        }

        var inputActionsAsset = Resources.Load<InputActionAsset>("CharacterActions");
        lookAction = inputActionsAsset.FindAction("Look", true);
        lookAction.Enable();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 look = lookAction.ReadValue<Vector2>();
        float lookX = invertX ? -look.x : look.x;
        float lookY = invertY ? -look.y : look.y;

        yaw += lookX * sensX * Time.deltaTime;
        pitch -= lookY * sensY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);

        if (rotatePlayerWithCamera)
        {
            Quaternion targetRotation = Quaternion.Euler(0, yaw, 0);
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);
        }
    }
}