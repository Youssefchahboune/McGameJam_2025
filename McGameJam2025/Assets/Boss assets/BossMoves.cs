using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossMoves : MonoBehaviour
{
    public Animator bossAnimator;
    public float TimeTransition = 1f;
    public float beakAttackToIdleTime = 1f;
    public bool bossIsAttacking = false;
    public static bool bossDead = false;
    public Transform playerTransform;
    public 

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("beakAttackCall", 1f, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Alpha1)) {
            //idle
            setBossToIdle();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            //beak attack
            setBossToIdle();
            //StartCoroutine(BeakAttack());
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            // slashes
            setBossToIdle();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            // dash
            setBossToIdle();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            // dash down
            setBossToIdle();

        }*/


        if (!bossIsAttacking && !bossDead)
        {
            bossIsAttacking = true;
            float randomTime = Random.Range(1f, 2f);
            beakAttackCall(randomTime);
        }
        

        
    }

    public void setBossToIdle()
    {
        bossAnimator.SetBool("beakAttack", false);
        bossAnimator.SetBool("dashDown", false);
        bossAnimator.SetBool("slashes", false);
        bossAnimator.SetBool("dashTowardsPlayer", false);
    }

    public void beakAttackCall(float speedAnimation)
    {
        StartCoroutine(BeakAttack(speedAnimation));
    }
    
    private IEnumerator BeakAttack(float speedAnimation)
    {
        yield return new WaitForSeconds(speedAnimation);
        if (!bossDead)
        {
            bossAnimator.SetBool("beakAttack", true);
        }
        
        yield return new WaitForSeconds(beakAttackToIdleTime);
        
        if (!bossDead)
        {
            setBossToIdle();
            bossIsAttacking = false;
        }
    }

    public static void BossIsDefeated()
    {
        bossDead = true;
    }
}
