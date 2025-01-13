using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutDoor1 : MonoBehaviour
{
    public PlayerController playerController;
    private Collider2D colider;
    public Animator doorAnimator;
    public AudioSource bossMusic;
    public AudioSource AmbianceSounds;
    public CinemachineVirtualCamera followCamera; // The player's camera
    public CinemachineVirtualCamera bossRoomCamera; // The boss room camera
    void Start()
    {
        colider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            playerController.FreezePlayer();
            AmbianceSounds.Stop();
            bossMusic.Play();
            followCamera.Priority = 10;
            bossRoomCamera.Priority = 20;
            //deactivate trigger colider
            colider.enabled = false;
            //shut door animation
            doorAnimator.SetBool("shutDoor", true);
            StartCoroutine(unfreezePlayer());
        }
        
    }

    private IEnumerator unfreezePlayer()
    {
        yield return new WaitForSeconds(2f);
        playerController.unFreezePlayer();
    }
}
