using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 5;
    private KnockBack knockback;

    public SpriteRenderer playerSr;
    //public playerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        knockback = GetComponent<KnockBack>();
    }


    public void TakeDamage(int pAmount)
    public void TakeDamage(int pAmount, Vector2 hitDirection)
    {
        health -= pAmount;


        // death check
        if(health <= 0)
        {
            playerSr.enabled = false;
            ResetCurrentScene();
        }

        // knockback 
        knockback.CallKnockback(hitDirection, Vector2.up);
    }

    public void ResetCurrentScene()
    {
        // Reloads the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
