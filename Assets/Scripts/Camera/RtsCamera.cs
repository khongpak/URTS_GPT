using UnityEngine;

public class RtsCamera : MonoBehaviour
{
    [Header("Pan")]
    public float panSpeed = 20f;
    public float edge = 10f;
    public bool edgePan = true;

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float minY = 10f;
    public float maxY = 60f;
    public float zoomSmoothTime = 0.08f;
    public bool invertScroll = false;

    [Header("Bounds")]
    public bool useBounds = false;
    public Vector2 minXZ = new(-50, -50);
    public Vector2 maxXZ = new(50, 50);

    [Header("Smoothing")]
    public float moveSmoothTime = 0.08f;

    [Header("Reset")]
    public bool resetRotation = true; //เลือกว่าจะ รีเซ็ตมุมกล้องด้วยหรือไม่

    Vector3 moveVel;   // เวคเตอร์ความเร็วชั่วคราวสำหรับ SmoothDamp (pan)
    float zoomVel;     // ความเร็วชั่วคราวสำหรับ SmoothDamp (zoom)

    private Vector3 startCameraPos;
    private Quaternion startCameraRota;

    void Start() 
    {
        startCameraPos = transform.position;
        startCameraRota = transform.rotation;

        Debug.Log($"X : {startCameraPos.x}, Y : {startCameraPos.y}, Z : {startCameraPos.z}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (resetRotation)
            {
                transform.SetPositionAndRotation(startCameraPos, startCameraRota);
            }
            else
            {
                transform.position = startCameraPos;
            }

            moveVel = Vector3.zero;
            zoomVel = 0f;
            return;
        }

        Vector3 pos = transform.position;

        // ทิศทางตามกล้องแต่แบนบนระนาบพื้น (XZ)
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        // Keyboard pan
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 desiredMove = (right * h + fwd * v) * panSpeed;

        // Edge pan
        if (edgePan)
        {
            Vector3 m = Input.mousePosition;
            if (m.x <= edge) desiredMove += -right * panSpeed;
            if (m.x >= Screen.width - edge) desiredMove += right * panSpeed;
            if (m.y <= edge) desiredMove += -fwd * panSpeed;
            if (m.y >= Screen.height - edge) desiredMove += fwd * panSpeed;
        }

        // Smooth pan
        Vector3 targetPos = pos + desiredMove * Time.deltaTime;
        pos = Vector3.SmoothDamp(pos, targetPos, ref moveVel, moveSmoothTime);

        // Zoom + smooth
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (invertScroll) scroll = -scroll;
        float targetY = Mathf.Clamp(pos.y - scroll * zoomSpeed * Time.deltaTime, minY, maxY);
        pos.y = Mathf.SmoothDamp(pos.y, targetY, ref zoomVel, zoomSmoothTime);

        // Clamp พื้นที่เล่น
        if (useBounds)
        {
            pos.x = Mathf.Clamp(pos.x, minXZ.x, maxXZ.x);
            pos.z = Mathf.Clamp(pos.z, minXZ.y, maxXZ.y);
        }

        transform.position = pos;
    }

#if UNITY_EDITOR
    // วาดกรอบ Bounds ให้เห็นใน Scene View
    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        Gizmos.color = Color.yellow;
        var center = new Vector3((minXZ.x + maxXZ.x) * 0.5f, transform.position.y, (minXZ.y + maxXZ.y) * 0.5f);
        var size = new Vector3(Mathf.Abs(maxXZ.x - minXZ.x), 0.1f, Mathf.Abs(maxXZ.y - minXZ.y));
        Gizmos.DrawWireCube(center, size);
    }
#endif
}