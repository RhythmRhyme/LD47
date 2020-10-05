using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 firstPosition;
    private Quaternion firstRotation;
    private Vector3 thirdPosition;
    private Quaternion thirdRotation;

    void Start()
    {
        firstPosition = new Vector3(0, 0.5f, 0f);
        firstRotation = Quaternion.Euler(0f,0f,0f);
        thirdPosition = new Vector3(0, 3f, -3f);
        thirdRotation = Quaternion.Euler(20f,0f,0f);
        
        //默认第一人称
        transform.localPosition = firstPosition;
        transform.localRotation = firstRotation;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            //放大
            ZoomIn();
        }
        else if (scroll < 0)
        {
            //缩小
            ZoomOut();
        }
    }

    public void ZoomOutPlus()
    {
        transform.localPosition = new Vector3(0, 10f, -10f);
        transform.localRotation = Quaternion.Euler(40f,0f,0f);
    }

    public void ZoomOut()
    {
        transform.localPosition = thirdPosition;
        transform.localRotation = thirdRotation;
    }

    public void ZoomIn()
    {
        transform.localPosition = firstPosition;
        transform.localRotation = firstRotation;
    }
}
