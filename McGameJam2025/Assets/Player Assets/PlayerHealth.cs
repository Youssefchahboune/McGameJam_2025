using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 5;

    public SpriteRenderer playerSr;
    //public playerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }


    public void TakeDamage(int pAmount)
    {
        health -= pAmount;
        if(health <= 0)
        {
            playerSr.enabled = false;
            ResetCurrentScene();
        }
    }

    public void ResetCurrentScene()
    {
        // Reloads the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
