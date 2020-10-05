using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    private GameObject player;
    private PlayerController playerController;
    public GameObject WeaponSet;
    private WeaponStandardAction weaponStandardAction;
    private Rigidbody enemyRb;
    public bool isDead { get; set; }
    private float changeCooldown = 2;
    private bool changeCooldownOver;
    private float attackCooldown = 4;
    private bool attackCooldownOver;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        enemyRb = GetComponent<Rigidbody>();
        weaponStandardAction = Instantiate(WeaponSet, transform.position, WeaponSet.transform.rotation).GetComponent<WeaponStandardAction>();
        weaponStandardAction.Master = this.gameObject;
        isDead = false;
        changeCooldownOver = true;
        attackCooldownOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || player == null || playerController.isDead || !playerController.CanMove())
        {
            return;
        }

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
                float distance = (player.transform.position - transform.position).magnitude;
                if (weaponStandardAction.isPreAttackPosture() && distance < 4)
                {
                    //攻击
                    weaponStandardAction.Attack();
                    StartCoroutine(AttackActionCooldown());
                }
                else if (attackCooldownOver && distance < 5)
                {
                    //靠近玩家准备攻击
                    weaponStandardAction.PreAttack();
                }
                else if (changeCooldownOver && distance < 8)
                {
                    //切换姿态
                    ChangeAction();
                    StartCoroutine(ChangeActionCooldown());
                }
                
                transform.LookAt(player.transform);
                if (playerController.CanMove() && !playerController.isOutFirstCircle)
                {
                    //靠近玩家
                    if (distance > 2.5)
                    {
                        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                    }
                    else if (distance < 1.5)
                    {
                        transform.Translate(Vector3.back * moveSpeed / 2 * Time.deltaTime);
                    }
                }
            }
        }
    }

    IEnumerator AttackActionCooldown()
    {
        attackCooldownOver = false;
        yield return new WaitForSeconds(attackCooldown);
        attackCooldownOver = true;
    }

    IEnumerator ChangeActionCooldown()
    {
        changeCooldownOver = false;
        yield return new WaitForSeconds(changeCooldown);
        changeCooldownOver = true;
    }

    /**
     * 武器姿态动作
     */
    private void ChangeAction()
    {
        switch (weaponStandardAction.posture - Random.Range(1, 2))
        {
            case 0:
                weaponStandardAction.leftPosition();
                break;
            case 1:
                weaponStandardAction.rightPosition();
                break;
            case 2:
                weaponStandardAction.upPosition();
                break;
            default:
                weaponStandardAction.upPosition();
                break;
        }
    }

    public void Death()
    {
        if (isDead)
            return;
        Debug.Log("Enemy Death");
        weaponStandardAction.audio.PlayOneShot(weaponStandardAction.audioHitSomebody);
        playerController.getGameManager().AddEnemyNumber(1);
        isDead = true;
        //光剑收起
        weaponStandardAction.light.SetActive(false);
        enemyRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        enemyRb.AddForce(Random.Range(0, 1) == 0 ? Vector3.back * 2 : Vector3.forward * 2, ForceMode.Impulse);
        StartCoroutine(FreezeBody());
    }

    IEnumerator FreezeBody()
    {
        yield return new WaitForSeconds(5);
        enemyRb.constraints = RigidbodyConstraints.FreezeAll;
    }
}