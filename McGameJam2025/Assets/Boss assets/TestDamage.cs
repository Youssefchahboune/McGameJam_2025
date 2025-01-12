using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damage = 1;
    public GameObject BossContainer;
    static int BossHealth = 100;
    public GameObject BossSpriteGameObject;

    public Material flashMaterial;
    private Material originalMaterial;
    private bool isHit = false;
    public float flashingTime = 0.2f;
    private Color originalcolor;

    public CinemachineVirtualCamera CinemachineVirtualCamera;
    public float shakeIntencity = 1f;
    public float shakeTime = 0.2f;
    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    public float HitstopTime = 1f;

    public Animator doorAnimator;
    private bool bossDefeated = false;
    public GameObject deathParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = BossSpriteGameObject.GetComponent<SpriteRenderer>().material;
        originalcolor = BossSpriteGameObject.GetComponent<SpriteRenderer>().color;

        if (CinemachineVirtualCamera.isActiveAndEnabled)
        {
            StopShake();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CinemachineVirtualCamera.isActiveAndEnabled)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    StopShake();
                }
            }
        }
    }

    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntencity;

        timer = shakeTime;
    }

    public void ShakeCamera(float intencity,float time)
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = intencity;

        timer = time;
    }

    public void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;

        timer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (!bossDefeated)
            {
                playerHealth.TakeDamage(damage);
            }
            
        } else if (collision.gameObject.tag == "slash")
        {
            if (!bossDefeated)
            {
                TakeDamage(5);
            }
            
        }
    }

    private void TakeDamage(int damage)
    {
        isHit = true;
        BossHealth -= damage;
        Debug.Log(BossHealth);

        if(BossHealth < 0)
        {
            
            bossDefeated = true;
            deathParticleSystem.SetActive(true);
            deathParticleSystem.GetComponent<ParticleSystem>().Play();
            ShakeCamera(2.5f, 7f);

            StartCoroutine(bossIsDefeated());
            return;
        }
        
        

        if (CinemachineVirtualCamera.isActiveAndEnabled)
        {
            ShakeCamera();
        }

        BossSpriteGameObject.GetComponent<SpriteRenderer>().material = flashMaterial;

        StartCoroutine(setEnemyOriginalMaterial());

        // hit stop
        TriggerHitStop(HitstopTime);
    }

    private IEnumerator setEnemyOriginalMaterial()
    {
        yield return new WaitForSeconds(flashingTime);
        BossSpriteGameObject.GetComponent<SpriteRenderer>().color = originalcolor;
        BossSpriteGameObject.GetComponent<SpriteRenderer>().material = originalMaterial;
        isHit = false;
    }

    public void TriggerHitStop(float duration)
    {
        StartCoroutine(DoHitStop(duration));
    }

    private IEnumerator DoHitStop(float duration)
    {
        // Save the original time scale and fixed delta time
        float originalTimeScale = Time.timeScale;
        float originalFixedDeltaTime = Time.fixedDeltaTime;

        // Set time scale to 0 (freeze time)
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixedDeltaTime to match

        // Wait for the specified duration in real time
        yield return new WaitForSecondsRealtime(duration);

        // Restore original time scale and fixed delta time
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    private IEnumerator bossIsDefeated()
    {
        yield return new WaitForSecondsRealtime(7f);
        // stop animation
        deathParticleSystem.GetComponent<ParticleSystem>().Stop();
        // destroy the boss
        Destroy(BossSpriteGameObject);
        gameObject.GetComponent<Collider2D>().enabled = false;
        
        yield return new WaitForSecondsRealtime(3.5f);
        //open the door
        doorAnimator.SetBool("bossDefeated", true);

    }
}
