using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManipulation : MonoBehaviour
{
    public Camera cam;
    public Transform selected;     //The object to be manipulated
    public Vector3 objOffset;

    public float rotationSpeed;
    public float translationSpeed;
    public bool leftClick;
    public bool rightClick;

    public float yaw;
    public float pitch;
    public float xMovement;
    public float zMovement;

    public bool newDeltaObtained;
    public Quaternion rotateBy;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        leftClick = Input.GetMouseButton(0);
        rightClick = Input.GetMouseButton(1);

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            ObjGrab();
        }
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && selected.tag == "moveable")
        {
            ObjRelease();
        }

        if (Input.GetMouseButton(1) && selected.tag == "moveable")
        {
            ObjRotate();
        }

        if (Input.GetMouseButton(0) && selected.tag == "moveable")
        {
            ObjDrag();
        }
        
        if (Input.GetMouseButtonUp(1))
            selected.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (Input.GetMouseButton(0) && selected.tag == "moveable")
        {
            //ObjDrag();

            xMovement = Input.GetAxis("Mouse X") * -translationSpeed;
            zMovement = Input.GetAxis("Mouse Y") * -translationSpeed;

            selected.transform.Translate(new Vector3(xMovement, 0, zMovement),  Space.World);
        }
    }

    public void ObjGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(hit.transform.gameObject.name);

            if (hit.transform.gameObject.tag == "moveable")
            {
                selected = hit.transform;
                objOffset = this.transform.position - selected.position;
            }
        }
    }

    public void ObjRelease()
    {
        selected = GameObject.Find("Placeholder Empty").transform;
    }

    public void ObjRotate()
    {
        ///My personal code for rotating with a nonmoving camera
        ///
        /*if (Input.GetMouseButtonDown(1))
           // selected.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed * -1;
        pitch += Input.GetAxis("Mouse Y") * rotationSpeed;

        selected.transform.eulerAngles = new Vector3(pitch, yaw, 0);*/

        ///Code adapted from Raigex on the Unity forums
        ///https://answers.unity.com/questions/299126/how-to-rotate-relative-to-camera-angleposition.html
        ///
        //Gets the world vector space for cameras up vector 
        Vector3 relativeUp = this.transform.TransformDirection(Vector3.up);
        //Gets world vector for space cameras right vector
        Vector3 relativeRight = this.transform.TransformDirection(Vector3.right);

        //Turns relativeUp vector from world to objects local space
        Vector3 objectRelativeUp = selected.transform.InverseTransformDirection(relativeUp);
        //Turns relativeRight vector from world to object local space
        Vector3 objectRelativeRight = selected.transform.InverseTransformDirection(relativeRight);

        rotateBy = Quaternion.AngleAxis(Input.GetAxis("Mouse X") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeUp)
            * Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeRight);

        selected.localRotation *= rotateBy;
    }

    public void ObjDrag()
    {
        Vector3 newOffset = this.transform.position - selected.position;

        if (newOffset != objOffset)
        {
            selected.position = this.transform.position - objOffset;
        }
    }
}
