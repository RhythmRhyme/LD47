using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject WeaponSet;
    private WeaponStandardAction weaponStandardAction;
    public GameObject mainCamera;
    private Rigidbody playerRb;
    [SerializeField] private float moveSpeed = 10;
    public float mouseSpeed = 2f;
    public bool isDead { set; get; }
    public float jumpForce = 5;
    private int blackPanelAlpha;
    public bool isOnGround { get; set; }

    public bool isOutFirstCircle { get; set; }

    public void Death()
    {
        Debug.Log("Player Death");
        weaponStandardAction.audio.PlayOneShot(weaponStandardAction.audioHitSomebody);
        isDead = true;
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        playerRb.AddForce(Random.Range(0, 1) == 0 ? Vector3.back * 4 : Vector3.forward * 4, ForceMode.Impulse);
        mainCamera.GetComponent<CameraManager>().ZoomOut();
        //光剑收起
        weaponStandardAction.light.SetActive(false);
        gameManager.playerDeathCount++;
    }

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        mainCamera = GameObject.Find("Main Camera");
        playerRb = GetComponent<Rigidbody>();
        weaponStandardAction = Instantiate(WeaponSet, transform.position, WeaponSet.transform.rotation).GetComponent<WeaponStandardAction>();
        weaponStandardAction.Master = this.gameObject;
        init();
    }

    void init()
    {
        isOnGround = true;
        isDead = false;
    }

    void reset()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.identity;
        weaponStandardAction.light.SetActive(true);
        weaponStandardAction.light.gameObject.transform.localPosition = new Vector3(0, 1, 0);
        weaponStandardAction.light.gameObject.transform.localRotation = WeaponSet.transform.localRotation;
    }

    public void pressR(bool clearBody)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            init();
            reset();
            gameManager.ResetWave(clearBody);
        }
    }

    void Update()
    {
        if (!CanMove())
        {
            return;
        }

        if (gameManager.pressR2Refresh)
        {
            pressR(false);
        }

        if (isDead)
        {
            pressR(true);
            return;
        }

        Move();
        Rotate();

        //武器位置冻结
        if (weaponStandardAction.isActionFreeze)
        {
            weaponStandardAction.FreezeAction();
        }
        else
        {
            //不打断正在进行的动作
            weaponStandardAction.continueAction();
            if (weaponStandardAction.isActionOver)
            {
                WeaponAction();
            }
        }
    }

    public bool CanMove()
    {
        return !gameManager.isStoryGoing();
    }

    /*
     * 玩家移动
     */
    private void Move()
    {
        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        float vSpeed = vInput * moveSpeed * Time.deltaTime;
        float hSpeed = hInput * moveSpeed * Time.deltaTime;
        playerRb.transform.Translate(new Vector3(hSpeed, 0, vSpeed));

        bool space = Input.GetKeyDown(KeyCode.Space);
        if (space && isOnGround)
        {
            isOnGround = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(setOnGroud());
        }
    }

    IEnumerator setOnGroud()
    {
        yield return new WaitForSeconds(1.2f);
        isOnGround = true;
    }

    /*
     * 玩家转动视角
     */
    private void Rotate()
    {
        //左右
        float mousex = Input.GetAxis("Mouse X") * mouseSpeed;
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, mousex, 0);
    }

    /**
     * 武器动作
     */
    private void WeaponAction()
    {
        bool leftMouse = Input.GetButton("Fire1");
        bool leftMouseUp = Input.GetButtonUp("Fire1");
        // if (leftMouse)
        // {
        //     weaponStandardAction.PreAttack();
        // } else if (leftMouseUp)
        // {
        //     weaponStandardAction.Attack();
        // }
        if (leftMouseUp)
        {
            weaponStandardAction.Attack();
        }
        else
        {
            //上下
            float mousey = Input.GetAxis("Mouse Y");
            //左右
            float mousex = Input.GetAxis("Mouse X") * mouseSpeed;
            if (mousey > 0.5)
            {
                weaponStandardAction.upPosition();
            }
            else if (mousex > 0.5)
            {
                weaponStandardAction.rightPosition();
            }
            else if (mousex < -0.5)
            {
                weaponStandardAction.leftPosition();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FirstCircle"))
        {
            Debug.Log("FirstCircle OnCollisionExit=" + other.gameObject.name + " tag=" + other.gameObject.tag);
            //玩家从圈中出去
            isOutFirstCircle = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FirstCircle"))
        {
            Debug.Log("FirstCircle OnTriggerEnter=" + other.gameObject.name + " tag=" + other.gameObject.tag);
            isOutFirstCircle = false;
        }
    }
}