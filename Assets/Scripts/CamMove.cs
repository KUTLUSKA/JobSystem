using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fastSpeed = 15f;
    public float mouseSensitivity = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // İmleci gizle ve ortala
    }

    void Update()
    {
        // Mouse hareketi ile kamera döndürme
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // Aşırı yukarı/aşağı bakmayı engelle

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Klavye ile hareket
        float moveX = Input.GetAxis("Horizontal"); // A-D
        float moveZ = Input.GetAxis("Vertical");   // W-S

        // Hız artırma (Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * currentSpeed * Time.deltaTime;
    }
}