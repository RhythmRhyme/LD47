using UnityEngine;
using System.Collections;


public class DrawCircle : MonoBehaviour
{
    public GameObject circleModel;
    //旋转改变的角度
    public int changeAngle = 0;
    //旋转一周需要的预制物体个数
    private int count;

    private float angle = 0;
    public float r = 5;
    public float yPosition = 0;

    // Use this for initialization
    void Start()
    {
        count = 360 / changeAngle;
        for (int i = 0; i < count; i++)
        {
            yPosition -= 0.01f;
            Vector3 center = new Vector3(0, yPosition, 0);
            GameObject cube = Instantiate(circleModel);
            float hudu = (angle / 180.0f) * Mathf.PI;
            float xx = center.x + r * Mathf.Cos(hudu);
            float zz = center.z + r * Mathf.Sin(hudu);
            cube.transform.position = new Vector3(xx, yPosition, zz);
            cube.transform.LookAt(center);
            cube.transform.parent = this.transform;
            angle += changeAngle;
        }
    }
}