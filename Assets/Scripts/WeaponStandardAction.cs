using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStandardAction : MonoBehaviour
{
    public GameObject hilt1;
    public GameObject light;

    public AudioClip audioHitSomebody;
    public AudioClip audioHitLightsaber;
    public AudioClip audioAttack;
    public AudioSource audio { get; set; }

    public GameObject Master { get; set; }
    private bool isPlayer;

    //动作是否完成
    public bool isActionOver { get; set; }

    //动作是否暂停
    public bool isActionFreeze { get; set; }
    private float freezeActionTime = 0.3f;
    private Vector3 freezePosition;
    private Quaternion freezeRotation;

    //姿态
    public int posture;

    private float changeSpeed = 10;
    private float attSpeed = 20;

    //目标动作位置、角度
    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation = Quaternion.identity;

    private float attCooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (Master != null)
        {
            transform.parent = Master.transform;
        }
        audio = GetComponent<AudioSource>();
        posture = 1;
        isPlayer = Master.CompareTag("Player");
        if (isPlayer)
        {
            changeSpeed = 30;
            attSpeed = 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private bool compareQuaternion(Quaternion a, Quaternion b)
    {
        return Mathf.Abs(a.x) - Mathf.Abs(b.x) < 0.05
               && Mathf.Abs(a.y) - Mathf.Abs(b.y) < 0.05
               && Mathf.Abs(a.z) - Mathf.Abs(b.z) < 0.05;
    }

    public void upPosition()
    {
        posture = 2;
        targetPosition = new Vector3(0, 0.4f, 0.6f);
        targetRotation = Quaternion.Euler(-75f + 90, 0, 0);
        ActionAndCheckIsOver(targetPosition, targetRotation, attSpeed);
    }

    public void upPositionPreAttack()
    {
        posture = 12;
        targetPosition = new Vector3(0, 1.1f, 0.6f);
        targetRotation = Quaternion.Euler(-130f + 90, 0, 0);
        if (ActionAndCheckIsOver(targetPosition, targetRotation, attSpeed) && isPlayer)
        {
            posture = 22;
        }
    }

    public void upPositionAttack()
    {
        posture = 22;
        targetPosition = new Vector3(0, 0.2f, 0.6f);
        targetRotation = Quaternion.Euler(15 + 90, 0, 0);
        ActionAndCheckIsOver(targetPosition, targetRotation, attSpeed, attCooldown);
        StartCoroutine(CountAndSetPosture());
    }

    public void leftPosition()
    {
        posture = 0;
        targetPosition = new Vector3(-0.2f, 1.3f, 0.4f);
        targetRotation = Quaternion.Euler(new Vector3(60 + 90, -45f, 0));
        ActionAndCheckIsOver(targetPosition, targetRotation, changeSpeed);
    }

    public void leftPositionPreAttack()
    {
        posture = 10;
        //高举
        // targetPosition = new Vector3(-0.5f, 0.6f, 0.6f);
        // targetRotation = Quaternion.Euler(new Vector3(-30f, -130f, 0));
        //横放
        targetPosition = new Vector3(-0.3f, 0.65f, 0.5f);
        targetRotation = Quaternion.Euler(new Vector3(6f + 90, -60f, 0));
        if (ActionAndCheckIsOver(targetPosition, targetRotation, changeSpeed)  && isPlayer)
        {
            posture = 20;
        }
    }

    public void leftPositionAttack()
    {
        posture = 20;
        // targetPosition = new Vector3(0.5f, 0.52f, 0.72f);
        // targetRotation = Quaternion.Euler(new Vector3(65, 50, 0));
        targetPosition = new Vector3(0.5f, 0f, 0.5f);
        targetRotation = Quaternion.Euler(new Vector3(90, 215, 140));
        ActionAndCheckIsOver(targetPosition, targetRotation, attSpeed, attCooldown);
        StartCoroutine(CountAndSetPosture());
    }

    public void rightPosition()
    {
        posture = 1;
        targetPosition = new Vector3(0.4f, 0f, 0.65f);
        targetRotation = Quaternion.Euler(new Vector3(250 + 90, -120f, 0));
        ActionAndCheckIsOver(targetPosition, targetRotation, changeSpeed);
    }

    public void rightPositionPreAttack()
    {
        posture = 11;
        targetPosition = new Vector3(0.5f, 0f, 0f);
        targetRotation = Quaternion.Euler(new Vector3(-180f + 90, -190f, 0));
        if (ActionAndCheckIsOver(targetPosition, targetRotation, changeSpeed) && isPlayer)
        {
            posture = 21;
        }
    }

    public void rightPositionAttack()
    {
        posture = 21;
        targetPosition = new Vector3(0.125f, 0.25f, 1f);
        targetRotation = Quaternion.Euler(new Vector3(180f + 90, 180f, 0));
        ActionAndCheckIsOver(targetPosition, targetRotation, attSpeed, attCooldown);
        StartCoroutine(CountAndSetPosture());
    }

    /**
     * 切换动作并检查是否执行完毕
     */
    private bool ActionAndCheckIsOver(Vector3 targetPosition, Quaternion targetRotation, float speed, float cooldown)
    {
        //碰撞检测，画出两次移动间的N条射线，如果相交，则当前帧武器移动到该位置
        checkCollision(targetPosition, targetRotation, speed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * speed);
        bool isOver = Vector3Util.compareVector3(transform.localPosition, targetPosition, 0.05f) && compareQuaternion(transform.localRotation, targetRotation);
        if (isOver)
        {
            SetActionOver(cooldown);
        }
        else
        {
            SetActionProcess();
        }

        return isOver;
    }

    private bool checkCollision(Vector3 targetPosition, Quaternion targetRotation, float speed)
    {
        //TODO 射线检测武器碰撞
        // Debug.Log("hilt1Position=" + hilt1Position + " \t " + lightPosition);
        // Ray[] rays = new Ray[5];
        // rays[0] = new Ray(transform.position - new Vector3(0, 0,0 ), transform.TransformDirection(Vector3.up));
        // rays[1] = new Ray(transform.position - new Vector3(0.2f, 0,0 ), transform.TransformDirection(Vector3.up));
        // rays[2] = new Ray(transform.position - new Vector3(-0.2f, 0,0 ), transform.TransformDirection(Vector3.up));
        // rays[3] = new Ray(transform.position - new Vector3(0, 0,0.2f ), transform.TransformDirection(Vector3.up));
        // rays[4] = new Ray(transform.position - new Vector3(0, 0,-0.2f ), transform.TransformDirection(Vector3.up));
        // foreach (Ray ray in rays)
        // {
        //     // Debug.Log(ray.direction);
        //     Debug.DrawRay(ray.origin ,ray.direction, Color.red);
        //     RaycastHit hit;
        //     float raylength = light.GetComponent<CapsuleCollider>().height;
        //     if(Physics.Raycast (ray, out hit, raylength))
        //     {
        //         if(hit.collider.CompareTag("Enemy") || (hit.collider.CompareTag("Lightsaber") && !hit.collider.gameObject.Equals(this.light)))
        //         {
        //             Debug.Log("检测到物体 " + hit.collider.tag);
        //             return true;
        //         }
        //     }
        // }
        return false;
    }

    private bool ActionAndCheckIsOver(Vector3 targetPosition, Quaternion targetRotation, float speed)
    {
        return ActionAndCheckIsOver(targetPosition, targetRotation, speed, 0);
    }

    IEnumerator SetCooldownActionOver(float time)
    {
        yield return new WaitForSeconds(time);
        SetActionOver(0);
    }

    /**
     * 设置动作结束
     */
    private void SetActionOver(float cooldown)
    {
        if (cooldown > 0)
        {
            StartCoroutine(SetCooldownActionOver(0));
        }
        else
        {
            isActionOver = true;
        }
    }

    /**
     * 设置动作开始
     */
    private void SetActionProcess()
    {
        isActionOver = false;
    }

    /**
     * 攻击蓄力动作
     */
    public void PreAttack()
    {
        if (isActionFreeze)
        {
            return;
        }

        switch (posture)
        {
            case 0:
                leftPositionPreAttack();
                break;
            case 1:
                rightPositionPreAttack();
                break;
            case 2:
                upPositionPreAttack();
                break;
        }
    }

    /**
     * 攻击
     */
    public void Attack()
    {
        if (isActionFreeze)
        {
            return;
        }

        switch (posture)
        {
            case 0:
            case 10:
                leftPositionAttack();
                break;
            case 1:
            case 11:
                rightPositionAttack();
                break;
            case 2:
            case 12:
                upPositionAttack();
                break;
        }
        audio.PlayOneShot(audioAttack);
    }


    public void continueAction()
    {
        if (isActionFreeze)
        {
            return;
        }

        switch (posture)
        {
            case 0:
                leftPosition();
                break;
            case 1:
                rightPosition();
                break;
            case 2:
                upPosition();
                break;
            case 10: //蓄力
                leftPositionPreAttack();
                break;
            case 20: //攻击
                leftPositionAttack();
                break;
            case 11: //蓄力
                rightPositionPreAttack();
                break;
            case 21: //攻击
                rightPositionAttack();
                break;
            case 12: //蓄力
                upPositionPreAttack();
                break;
            case 22: //攻击
                upPositionAttack();
                break;
        }
    }

    IEnumerator CountAndSetPosture()
    {
        yield return new WaitForSeconds(freezeActionTime);
        isActionFreeze = false;
        //回到前一个动作
        switch (posture)
        {
            case 10:
            case 20:
                leftPosition();
                break;
            case 11:
            case 21:
                rightPosition();
                break;
            case 12:
            case 22:
                upPosition();
                break;
        }
    }

    public void setActionFreeze()
    {
        isActionFreeze = true;
        freezePosition = transform.position;
        freezeRotation = transform.rotation;
        StartCoroutine(CountAndSetPosture());
        audio.PlayOneShot(audioHitLightsaber);
    }

    public void FreezeAction()
    {
        transform.position = freezePosition;
        transform.rotation = freezeRotation;
    }
    
    public bool isPreAttackPosture()
    {
        switch (posture)
        {
            case 10:
            case 11:
            case 12:
                return true;
            default:
                return false;
        }
    }
    
    public bool isAttackPosture()
    {
        switch (posture)
        {
            case 20:
            case 21:
            case 22:
                return true;
            default:
                return false;
        }
    }
}