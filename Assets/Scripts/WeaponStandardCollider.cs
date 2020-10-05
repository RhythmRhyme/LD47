using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStandardCollider : MonoBehaviour
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
        else if (other.CompareTag("Enemy"))
        {
            if (actionScript.isAttackPosture())
            {
                other.gameObject.transform.parent.gameObject.GetComponent<EnemyController>().Death();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        actionScript.isActionFreeze = false;
    }
}