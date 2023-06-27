using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // From https://gamedevacademy.org/unity-rts-camera-tutorial/

    public float moveSpeed;
    public float zoomSpeed;

    public float minZoomDist;
    public float maxZoomDist;

    private Camera myCamera;

    void Awake(){
        myCamera = Camera.main;
    }

    private void Move(){
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * zInput + transform.right * xInput;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void Zoom(){
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        float dist = Vector3.Distance(transform.position, myCamera.transform.position);

        if(dist < minZoomDist && scrollInput > 0.0f){
            return;
        }
        if(dist > maxZoomDist && scrollInput < 0.0f){
            return;
        }

        myCamera.transform.position += myCamera.transform.forward * scrollInput * zoomSpeed;
    }

    public void FocusOnPoint(Vector3 point){
        transform.position = point;
    }

    void Update(){
        Move();
        Zoom();
    }
}
