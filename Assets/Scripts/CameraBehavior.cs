using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 farOffset = new Vector3(0f, 1.5f, -5f);
    public Vector3 nearOffset = new Vector3(-1.75f, 1.4f, -3f);
    public float smoothSpeed = 6f;

    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 120f;
    public float minY = -35f;
    public float maxY = 60f;

    private Transform target;
    public bool isLeftShoulder = false;

    private Vector3 desiredOffset;
    private Vector3 currentOffset;

    private float yaw;
    private float pitch = 10f;

    public GameObject Crosshair;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        currentOffset = farOffset;
        yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        if (Input.GetMouseButton(1))
        {
            desiredOffset = nearOffset;
            Crosshair.SetActive(true);
        }
        else
        {
            desiredOffset = farOffset;
            desiredOffset.x = 0f;
            Crosshair.SetActive(false);
        }

        RectTransform rt = Crosshair.GetComponent<RectTransform>();
        Vector2 pos = rt.anchoredPosition;

        if (Input.GetKeyDown(KeyCode.Q) && Input.GetMouseButton(1))
        {
            if (isLeftShoulder)
            {
                isLeftShoulder = false;
                pos.x = -15f;
            }
            else
            {
                isLeftShoulder = true;
                pos.x = 15f;
            }
            rt.anchoredPosition = pos;
        }

        if (Input.GetMouseButton(1))
        {
            if (isLeftShoulder)
                desiredOffset.x = -Mathf.Abs(desiredOffset.x);
            else
                desiredOffset.x = Mathf.Abs(desiredOffset.x);
        }

        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, smoothSpeed * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = target.position + rot * currentOffset;

        Vector3 lookPoint = target.position + rot * Vector3.forward * 15f + Vector3.up * 1f;
        transform.LookAt(lookPoint);
    }
}