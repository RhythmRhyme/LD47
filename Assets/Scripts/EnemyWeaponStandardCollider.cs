using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponStandardCollider : MonoBehaviour
{
    private WeaponStandardAction actionScript;

    // Start is called before the first frame update
    void Start()
    {
        actionScript = transform.parent.gameObject.GetComponent<WeaponStandardAction>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lightsaber"))
        {
            if(actionScript != null)
                actionScript.setActionFreeze();
        }
        else if (other.CompareTag("Player"))
        {
            if (actionScript.isAttackPosture())
            {
                other.GetComponent<PlayerController>().Death();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        actionScript.isActionFreeze = false;
    }
}