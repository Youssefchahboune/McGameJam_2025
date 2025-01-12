using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public float knockbackTime = 0.3f;
    public float hitDirectionForce = 20f;
    public float constForce = 10f;

    public float knockbackCooldown = 1.0f;

    private Rigidbody2D playerRb;

    private Coroutine knockbackCoroutine;

    private bool canKnockback = true;

    public bool IsBeingKnockBack { get; private set;}

    public IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection)
    {
        IsBeingKnockBack = true;

        Vector2 _hitForce;
        Vector2 _constantForce;
        Vector2 _knockbackForce;

        _hitForce = hitDirection * hitDirectionForce;
        _constantForce = constantForceDirection * constForce;


        float elapsedTime = 0f;
        while( elapsedTime < knockbackTime)
        {
            elapsedTime += Time.fixedDeltaTime;

            _knockbackForce = _hitForce + _constantForce;

            playerRb.velocity =_knockbackForce;

            yield return new WaitForFixedUpdate();
        }


        IsBeingKnockBack = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void CallKnockback(Vector2 hitDirection, Vector2 constantForceDirection)
    // {
    //     knockbackCoroutine = StartCoroutine(KnockbackAction(hitDirection, constantForceDirection));
    // }

    public void CallKnockback(Vector2 hitDirection, Vector2 constantForceDirection)
    {
        // Only start knockback if we're not on cooldown
        if (!canKnockback) 
            return;

        // We can't knockback again until cooldown finishes
        canKnockback = false;

        // Start the knockback coroutine
        knockbackCoroutine = StartCoroutine(KnockbackAction(hitDirection, constantForceDirection));

        // Also start a separate cooldown coroutine
        StartCoroutine(KnockbackCooldownRoutine());
    }

    private IEnumerator KnockbackCooldownRoutine()
    {
        // Wait for the knockback time to finish first
        // (Optional - if you prefer cooldown only starts after the knockback ends)
        // yield return new WaitForSeconds(knockbackTime);

        // Or if you want cooldown to begin immediately, just skip the above

        // Wait the defined cooldown duration
        yield return new WaitForSeconds(knockbackCooldown);

        // Now we can get knocked back again
        canKnockback = true;
    }
}
