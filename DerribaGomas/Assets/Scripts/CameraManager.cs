using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Transform target;
    [SerializeField] float distanceCam;
    [SerializeField] float camZ;
    [SerializeField] float camZMin;
    [SerializeField] float camZMax;
    [SerializeField] float scrollSpeed;
    float mouseWheel;
    Vector3 previousPos;
    Camera cam;

    public static CameraManager instance;

    void Awake()
    {

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCamera(int click)
    {
        if (Input.GetMouseButtonDown(click))
            previousPos = cam.ScreenToViewportPoint(Input.mousePosition);

        if (Input.GetMouseButton(click))
        {
            Vector3 direction = previousPos - cam.ScreenToViewportPoint(Input.mousePosition);

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * distanceCam);
            cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * distanceCam, Space.World);
            cam.transform.Translate(new Vector3(0, 0, camZ));

            previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    void Zoom()
    {
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel != 0)
        {
            camZ = Mathf.Clamp(mouseWheel * scrollSpeed, camZMin, camZMax);
            cam.transform.Translate(new Vector3(0, 0, camZ));
        }
    }

    public void ChangeTarget(Transform targetToChange)
    {
        target = targetToChange;

        //Posicion de la camata
        cam.transform.position = target.Find("camPos").position;

        //Mirar a la goma
        cam.transform.forward = -(cam.transform.position - target.position);
        MoveCamera(targetToChange.GetComponent<Rubber>().clickWatch);
    }


    public void StartAnim()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Start");
    }
}
