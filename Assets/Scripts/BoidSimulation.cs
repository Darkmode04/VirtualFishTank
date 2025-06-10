using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject boidPrefab = null;
    public GameObject foodPrefab = null;
    public GameObject stonePrefab = null;
    public int boidToSpawn = 10;
    public float FoodSeekRadius = 2f;

    public List<Boid> boids = new List<Boid>();

    
    public enum ControlMode
    {
        Seek,
        Pursue,
        Food,
        Obstacle

    }
    public ControlMode controlMode = ControlMode.Seek;


    // Start is called before the first frame update
    private void Start()
    {
        //following the mouse
        targetObject = GameObject.Find("Target");

        // Spawn the boids
        for (int i = 0; i < boidToSpawn; i++)
        {
            //the spot of spawning
            Vector3 position = new Vector3(Random.Range(0.7f,-1), Random.Range(0, 1.3f), Random.Range(-0.4f, 0));
            Quaternion rotation = Random.rotation;

            //instantiate a copy of the prefab
            GameObject spawnerBoid = Instantiate(boidPrefab, position, rotation);
            
            spawnerBoid.name = "Boid " + i;
            
            //randomizing the size
            spawnerBoid.transform.localScale = new Vector3(Random.Range(1,5), Random.Range(3,5), Random.Range(2,5));
            
            //randomizing the color
            spawnerBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0,1,0.5f,1,0.5f,1));

            spawnerBoid.GetComponent<Rigidbody>().linearVelocity = Random.insideUnitSphere * 0.3f;
            
            //get boid component from newly instantiated copy
            Boid boidComponent = spawnerBoid.GetComponent<Boid>();
            
            //randomizing properties of each boid
            boidComponent.speedMax = Random.Range(0.5f, 2);
            boidComponent.accelMax = Random.Range(0.5f, 2);
            
            //add the new boid to a list we can use later
            boids.Add(boidComponent);
        }
    }

 
    private void Update()
    {
        //changing the control mode based on the situation 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlMode = ControlMode.Seek;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlMode = ControlMode.Pursue;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlMode = ControlMode.Food;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controlMode = ControlMode.Obstacle;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        //checking if the mouse left click is pressed
        if (Input.GetMouseButtonDown(0) && controlMode == ControlMode.Food)
        {
            SpawnFood();
        }
        if (Input.GetMouseButtonDown(0) && controlMode == ControlMode.Obstacle)
        {
            SpawnObstacle();
        }
    }
    
    //spawning the food prefab
    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation);
    }
    
    private void SpawnObstacle()
    {
        Instantiate(stonePrefab, targetObject.transform.position, Random.rotation);
    }
    
    private void FixedUpdate()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(ray, out hitInfo,100);
        
        if (didHit)
        {
            targetObject.transform.position = hitInfo.point;
        }

        //For each boid, check if they can find food, if so, move towards it.
        //If not, follow control mode.
        for (int i = 0; i < boids.Count; i++)
        {
            var food = CheckForFood(i);
            if (food != null)
            {
                FoodSeekModeControl(i, food);
            }
            else
            {
                switch (controlMode)
                {
                    case ControlMode.Seek:
                    {
                        SeekModeControl(i);
                        break;
                    }
                    case ControlMode.Pursue:
                    {
                        PursueModeControl(i);
                        break;
                    }
                }
            }
        }

    }

    private void FoodSeekModeControl(int index, Food foodTarget)
    {
        print("found closest food");
        Vector3 accel = boids[index].Seek(foodTarget.transform.position, boids[index].accelMax);
        float distanceToFood = Vector3.Distance(boids[index].transform.position, foodTarget.transform.position);
        boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;

        if (distanceToFood < FoodSeekRadius)
         print("GAAAAAAAME");
        //if closer to food, limit the speed
        boids[index].RB.linearVelocity = 
            (distanceToFood < FoodSeekRadius) ?
                boids[index].RB.linearVelocity.normalized * Mathf.Lerp(boids[index].accelMax * 0.1f, 1, FoodSeekRadius/distanceToFood) :
                boids[index].RB.linearVelocity;
        
        boids[index].Arrive();
    }

    private Food CheckForFood(int index)
    {
        Collider[] colliders = Physics.OverlapSphere(boids[index].transform.position, FoodSeekRadius);

        Food closestFood = null;
        float distance = float.MaxValue;

        //for each collider in this radius
        foreach (Collider collider in colliders)
        {
            //check if the food has component
            Food food = collider.GetComponent<Food>();

            //if it has this component, it will not be null, if not null, we can seek the food
            if (food != null)
            {
                if (Vector3.Distance(food.transform.position, boids[index].transform.position) < distance)
                {
                    closestFood = food;
                    distance = Vector3.Distance(food.transform.position, boids[index].transform.position);
                }

            }
        }
        
        return closestFood;

    }


    private void SeekModeControl(int index)
    {
        //call seek function on each booid to calculate the acceleration vector
        Vector3 accel = boids[index].Seek(targetObject.transform.position, boids[index].accelMax);

        if (Input.GetMouseButton(0))
        {
            //apply acceleration
            boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;
            //draw acceleration
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
        else if (Input.GetMouseButton(1))
        {
            //apply acceleration
            boids[index].RB.linearVelocity -= accel * Time.fixedDeltaTime;
            //draw acceleration
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
    }

    private void PursueModeControl(int index)
    {
        //call puruse function to calculate acceleration
        Vector3 accel = boids[index].Pursue(targetObject.transform.position, boids[index].accelMax, boids[index].speedMax);
        Debug.DrawRay(boids[index].transform.position, accel, Color.green);


        if (Input.GetMouseButton(0))
        {
            //apply acceleration
            boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;
            //draw acceleration
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
        else if (Input.GetMouseButton(1))
        {
            //apply acceleration
            boids[index].RB.linearVelocity -= accel * Time.fixedDeltaTime;
            //draw acceleration
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }

    }

}
