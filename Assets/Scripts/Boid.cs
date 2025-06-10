using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{

    public GameObject targetObject;
    public Rigidbody RB;

    public float speedMax = 1;
    public float accelMax = 1;

    // public Vector3 whiskerStartPosition;
    public Vector3 whiskerLeftEndPosition;
    public Vector3 whiskerRightEndPosition;
    public GameObject leftWhisker;
    public GameObject rightWhisker;
    
    [SerializeField]
    float foodGrabRadius = 0.1f;

    private void Start()
    {
        //target
        targetObject = GameObject.Find("Target");
        RB = GetComponent<Rigidbody>();
        RB.linearVelocity = Random.insideUnitSphere;
        
        var rotation = transform.rotation;
        
        transform.rotation = Quaternion.identity;
        leftWhisker.transform.position = transform.position + whiskerLeftEndPosition;
        rightWhisker.transform.position = transform.position + whiskerRightEndPosition;
        transform.rotation = rotation;
        
        
    }
    
    
    
    private void FixedUpdate()
    {
      
        Vector3 toTarget = targetObject.transform.position - transform.position;

        //checking if fish are close to obstacle
        //if it has hit the left whisker
        if (CheckForObstacles(leftWhisker.transform.position))
        {
            toTarget = transform.right;
        }
        //if it has hit the right whisker
        else if (CheckForObstacles(rightWhisker.transform.position))
        {
            toTarget = transform.right * -1f ;
        }
        
        Vector3 toTargetNormalized = toTarget.normalized;

        Vector3 acceleration = toTargetNormalized * accelMax;

        RB.linearVelocity += acceleration * Time.fixedDeltaTime;

        RB.linearVelocity = Vector3.ClampMagnitude(RB.linearVelocity, speedMax);

        transform.forward = RB.linearVelocity;
         
        //limiting speed
        float speed = RB.linearVelocity.magnitude;
        if(speed > speedMax)
        {
            //enforce a top speed
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
        //orient toward velocity
        transform.forward = Vector3.RotateTowards(transform.forward, RB.linearVelocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime,100);
    }
    
    public void Arrive()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, foodGrabRadius);
        
            
        //for each collider in this radius
        foreach (Collider collider in colliders)
        {
            //check if the food has component
            Food food = collider.GetComponent<Food>();
                 
            //if it has this component, it will not be null, if not null, we can seek the food
            if (food != null)
            {
                Debug.LogWarning($"Boid {name} ate food {food.name}");
                Destroy(food.gameObject);
            }
        }
    }
    
    public Vector3 Pursue(Vector3 target, float acceleration, float desiredSpeed)
    {
        //get the displacement vector to target
        Vector3 toTarget = target - transform.position;
        
        
        
        //normalize to get the direction to the target
        Vector3 toTargetNormalized = toTarget.normalized;
        
        //determine desired velocity towards target
        Vector3 desiredVelocity = toTargetNormalized * desiredSpeed;
        
        //determine required change in velocity between current velocity and desired
        Vector3 deltaVel = desiredVelocity - RB.linearVelocity;
        
        //acceleration in the direction of the required change
        Vector3 accel = deltaVel.normalized * acceleration;
        
        return accel;
    }
    
    
    public Vector3 Seek(Vector3 target, float acceleration)
    {
        Vector3 toTarget = target - transform.position;

        //direction to target
        Vector3 toTargetNormalized = toTarget.normalized;
        
        //determine acceleration with a given magnitude
        Vector3 accel = toTargetNormalized * acceleration;
        
        return accel;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position , leftWhisker.transform.position);
        Gizmos.DrawLine(transform.position , rightWhisker.transform.position);
    }

    private bool CheckForObstacles(Vector3 whiskerEndPosition)
    {
        //get direction of whiskers(raycast)
        var direction = whiskerEndPosition - transform.position;
        float lenght = direction.magnitude;
        direction.Normalize();
        var hits = Physics.RaycastAll(transform.position, direction, lenght);
        foreach (var hit in hits)
        {
            if (hit.collider.GetComponent<Obstacle>() != null)
            {
                print(name + " found obstacle " + hit.collider.name);
                return true;
            }
        }
        return false;
    }
}