using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public int damage = 1;
    public GameObject BossContainer;
    public int setBossHealth = 0;
    public static int BossHealth = 0;
    //public GameObject BossSpriteGameObject;

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
    public static bool bossDefeated = false;
    public GameObject deathParticleSystem;

    public AudioSource bossRoomMusic;
    public AudioSource Ambiance;
    public Animator enterDoor;

    public PlayerController playerController;

    public CinemachineVirtualCamera followCamera; // The player's camera
    public CinemachineVirtualCamera bossRoomCamera; // The boss room camera

    // Start is called before the first frame update
    void Start()
    {

        bossDefeated = false;
        
        BossHealth = setBossHealth;
        originalMaterial = GetComponent<SpriteRenderer>().material;
        originalcolor = GetComponent<SpriteRenderer>().color;

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
        if (collision.gameObject.tag == "slash")
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
        //Debug.Log(BossHealth);

        if(BossHealth < 0)
        {
            Collider2D[] bossColiders = GetComponents<Collider2D>();
            foreach (Collider2D col in bossColiders)
            {
                col.enabled = false;
            }
            bossDefeated = true;
            BossMoves.BossIsDefeated();
            bossRoomMusic.Stop();
            TriggerHitStop(HitstopTime);
            playerController.FreezePlayer();
            deathParticleSystem.SetActive(true);
            deathParticleSystem.GetComponent<ParticleSystem>().Play();
            ShakeCamera(3.5f, 7f);
            StartCoroutine(bossIsDefeated());
            return;
        }
        
        

        if (CinemachineVirtualCamera.isActiveAndEnabled)
        {
            ShakeCamera();
        }

        GetComponent<SpriteRenderer>().material = flashMaterial;

        StartCoroutine(setEnemyOriginalMaterial());

        // hit stop
        // TriggerHitStop(HitstopTime);
    }

    private IEnumerator setEnemyOriginalMaterial()
    {
        yield return new WaitForSeconds(flashingTime);
        GetComponent<SpriteRenderer>().color = originalcolor;
        GetComponent<SpriteRenderer>().material = originalMaterial;
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
        // stop animations + shake + music
        deathParticleSystem.GetComponent<ParticleSystem>().Stop();
        StopShake();
        GetComponent<SpriteRenderer>().enabled = false;
        bossRoomMusic.Stop();
        yield return new WaitForSecondsRealtime(1.5f);
        //open the door
        doorAnimator.SetBool("bossDefeated", true);
        enterDoor.SetBool("shutDoor", false);
        yield return new WaitForSecondsRealtime(1f);
        followCamera.Priority = 20;
        bossRoomCamera.Priority = 10;
        Ambiance.Play();
        playerController.unFreezePlayer();
        Destroy(BossContainer);

    }
}
