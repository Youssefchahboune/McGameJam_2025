using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinGear : MonoBehaviour
{
    private Rigidbody2D gearRb;
    public float rotationSpeed = 100f;
    void Start()
    {
        gearRb = GetComponent<Rigidbody2D>();
        gearRb.angularVelocity = rotationSpeed;
    }
}
