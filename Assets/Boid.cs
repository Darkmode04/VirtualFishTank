using UnityEngine;

public class Boid : MonoBehaviour
{

    public GameObject targetObject;
    public Rigidbody RB;

    public float speedMax = 1;
    public float accelMax = 1;

    private void Start()
    {
        targetObject = GameObject.Find("Target");
        RB = GetComponent<Rigidbody>();
        RB.linearVelocity = Random.insideUnitSphere;
    }

    private void FixedUpdate()
    {

        Vector3 toTarget = targetObject.transform.position - transform.position;

        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 acceleration = toTargetNormalized * accelMax;

        RB.linearVelocity += acceleration * Time.fixedDeltaTime;

        RB.linearVelocity = Vector3.ClampMagnitude(RB.linearVelocity, speedMax);

        transform.forward = RB.linearVelocity;

        float speed = RB.linearVelocity.magnitude;
        if (speed > speedMax)
        {
            RB.linearVelocity = RB.linearVelocity * speedMax / speed;
        }
    }




    private void Update()
    {
        Debug.DrawRay(transform.position, RB.linearVelocity, Color.red);
        AlignToVelocity();
    }


    public void AlignToVelocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, RB.linearVelocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime, 100);
    }

    public void Arrive()
    {
        float foodSeekRadius = 0.1f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, foodSeekRadius);

        foreach (Collider collider in colliders)
        {
            Food food = collider.GetComponent<Food>();
            if (food != null)
            {
                Debug.LogWarning($"Boid {name} ate food {food.name}");
                Destroy(food.gameObject);
            }
        }
    }

    public Vector3 Pursue(Vector3 target, float acceleration, float desiredSpeed)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 desiredVelocity = toTargetNormalized * desiredSpeed;
        Vector3 deltaVel = desiredVelocity - RB.linearVelocity;
        Vector3 accel = deltaVel.normalized * accelMax;

        return accel;
    }


    public Vector3 Seek(Vector3 target, float acceleration)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 accel = toTargetNormalized * acceleration;

        return accel;
    }

}