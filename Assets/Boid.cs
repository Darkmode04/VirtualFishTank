using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    public Rigidbody rigidbody;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.linearVelocity = Random.insideUnitSphere;
    }


    private void Update()
    {

        AlignToVelocity();

    }

    public void AlignToVelocity()
    {
        Vector3 velocity = rigidbody.linearVelocity;
        Vector3 forward = transform.forward;
        float angle = Vector3.SignedAngle(forward, velocity, Vector3.up);
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);


    }
}
