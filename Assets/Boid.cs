using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{

    public GameObject targetObjest;
    private Rigidbody Rigidbody;

    public float speedMax = 2;
    public float accelMax = 3;

    private void Start()
    {
        targetObjest = GameObject.Find("Target");
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
      
       Vector3 toTarget = targetObjest.transform.position - transform.position;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 acceleration = toTargetNormalized * accelMax;

        Rigidbody.linearVelocity += acceleration * Time.fixedDeltaTime;

        Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, speedMax);

        transform.forward = Rigidbody.linearVelocity;
    }


    public new Rigidbody rigidbody;
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
      
        transform.forward = Vector3.RotateTowards(transform.forward, rigidbody.linearVelocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime,100);

    }
}
