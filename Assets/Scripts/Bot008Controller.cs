using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot008Controller : MonoBehaviour
{
    public GameObject head;
    public GameObject ring;

    public PlayerController player { set; get; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    /**
     * 返回true则表示完成
     */
    public bool Do(int action)
    {
        if (action == 100)
        {
            //隐藏光环
            ring.SetActive(false);
            return true;
        }
        else if (action == 101)
        {
            //显示光环
            ring.SetActive(true);
            return true;
        }
        else if (action == 1)
        {
            //从地底出现
            IncorporealOn();
            float speed = 1;
            float z = player.transform.rotation.z > 0 ? -5f : 5f;
            float x = player.transform.rotation.z > 0 ? -1.5f : 1.5f;
            Vector3 targetPosition = player.transform.position + new Vector3(x, 0f, z);
            Debug.Log("transform.position=" + transform.position + "targetPosition=" + targetPosition);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            if (Vector3Util.compareVector3(transform.position, targetPosition, 0.05f))
            {
                return true;
            }
        }
        else if (action == 3)
        {
            //钻入地底消失
            IncorporealOn();
            float speed = 1;
            float z = player.transform.rotation.z > 0 ? -5f : 5f;
            Vector3 targetPosition = player.transform.position + new Vector3(0, -1.5f, z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            if (Vector3Util.compareVector3(transform.position, targetPosition, 0.05f))
            {
                return true;
            }
        }
        else if (action == 10)
        {
            //前往某地点
            IncorporealOff();
            float speed = 2;
            Vector3 targetPosition = getClosetMemoryBug().transform.position;
            transform.Translate((targetPosition - transform.position).normalized * speed * Time.deltaTime); 
            if (Vector3Util.compareVector3(transform.position, targetPosition, 8f))
            {
                return true;
            }
            
        }

        return false;
    }

    public GameObject getClosetMemoryBug()
    {
        GameObject[] findWithTag = GameObject.FindGameObjectsWithTag("Bug");
        GameObject closet = null;
        float minDistance = 0;
        foreach (GameObject obj in findWithTag)
        {
            float distance = (transform.position - obj.transform.position).magnitude;
            if (minDistance == 0)
            {
                minDistance = distance;
                closet = obj;
            } else if (distance < minDistance)
            {
                minDistance = distance;
                closet = obj;
            }
        }
        return closet;
    }
    
    /**
     * 取消碰撞及重力
     */
    public void IncorporealOn()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    /**
     * 取消碰撞及重力
     */
    public void IncorporealOff()
    {
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX
                                                | RigidbodyConstraints.FreezePositionZ
                                                | RigidbodyConstraints.FreezeRotationX
                                                | RigidbodyConstraints.FreezeRotationY
                                                | RigidbodyConstraints.FreezeRotationZ;
    }

    public void talkAction()
    {
        head.transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }
}