using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float maxFallSpeed = 5;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = maxFallSpeed;
    }
}
