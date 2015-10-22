using UnityEngine;
using System.Collections;

public class PlayerAiming : MonoBehaviour
{

    private Vector3 prevPos = new Vector3();
    private Vector3 prevMousePos = new Vector3();

    // Use this for initialization
    void Start()
    {
        prevPos = transform.position;
        prevMousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (prevPos != transform.position || prevPos != Input.mousePosition) {

            Vector3 toMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            toMouse.Normalize();

            toMouse.x = Mathf.Abs(toMouse.x) * toMouse.x;
            toMouse.y = Mathf.Abs(toMouse.y) * toMouse.y;

            float rotationZ = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            prevPos = transform.position;
            prevPos = Input.mousePosition;
        }
    }
}