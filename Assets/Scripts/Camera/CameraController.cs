using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float zoomRate = 0.05f;

    private float Xinput, Yinput;
    private float zoom;//1 zoom out, -1 zoom in, 0 no zoom

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Xinput = Input.GetAxisRaw("Horizontal");
        Yinput = Input.GetAxisRaw("Vertical");
        zoom = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;

        transform.position += new Vector3(Xinput,Yinput, 0) * speed * cam.orthographicSize * Time.deltaTime;
        cam.orthographicSize += zoomRate * zoom;
        if (cam.orthographicSize < 0) cam.orthographicSize = 0;
    }
}
