using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 farOffset = new Vector3(-2.5f, 1.5f, -5f);
    public Vector3 nearOffset = new Vector3(-1.75f, 1.4f, -3f);

    public float smoothSpeed = 6f;

    private Transform target;

    private bool isZooming = false;
    public bool isLeftShoulder = false;

    private Vector3 desiredOffset;
    private Vector3 currentOffset;

    public GameObject Crosshair;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        currentOffset = farOffset;
    }

    void LateUpdate()
    {

        if (Input.GetMouseButton(1))
        {
            isZooming = true;
        }
        else
        {
            isZooming = false;
        }

        RectTransform rt = Crosshair.GetComponent<RectTransform>();
        Vector2 pos = rt.anchoredPosition;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isLeftShoulder == true)
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


        if (isZooming == true)
        {
            desiredOffset = nearOffset;
        }
        else
        {
            desiredOffset = farOffset;
        }


        if (isLeftShoulder == true)
        {
            desiredOffset.x = -Mathf.Abs(desiredOffset.x);
        }
        else
        {
            desiredOffset.x = Mathf.Abs(desiredOffset.x);
        }

        // Smooth camera movement
        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, smoothSpeed * Time.deltaTime);
        //currentOffset = desiredOffset;

        transform.position = target.TransformPoint(currentOffset);

        Vector3 lookPoint = target.position + target.forward * 15f + Vector3.up * 1f;
        transform.LookAt(lookPoint);
    }
}