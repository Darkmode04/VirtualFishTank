using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BoidSimulationControl : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject boidPrefab = null;
    public GameObject foodPrefab = null;
    public int boidToSpawn = 10;

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
 
        targetObject = GameObject.Find("Target");

        // Spawn the boids
        for (int i = 0; i < boidToSpawn; i++)
        {
            Vector3 position = new Vector3(Random.Range(0.7f, -1), Random.Range(0, 1.3f), Random.Range(-0.4f, 0));
            Quaternion rotation = Random.rotation;
            GameObject spawnerBoid = Instantiate(boidPrefab, position, rotation);
            spawnerBoid.name = "Boid " + i;

            spawnerBoid.transform.localScale = new Vector3(Random.Range(1, 5), Random.Range(3, 5), Random.Range(2, 5));

            spawnerBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1));

            spawnerBoid.GetComponent<Rigidbody>().linearVelocity = Random.insideUnitSphere * 0.3f;

            Boid boidComponent = spawnerBoid.GetComponent<Boid>();

            boidComponent.speedMax = Random.Range(0.5f, 2);
            boidComponent.accelMax = Random.Range(0.5f, 2);

            boids.Add(boidComponent);
        }

    }


    private void Update()
    {
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

        if (Input.GetMouseButtonDown(0) && controlMode == ControlMode.Food)
        {
            SpawnFood();
        }
    }

    //spawning the food 
    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation);
    }

    private void FixedUpdate()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(ray, out hitInfo, 100);

        if (didHit)
        {
            targetObject.transform.position = hitInfo.point;
        }

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
        boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;

        boids[index].Arrive();
    }

    private Food CheckForFood(int index)
    {

        float foodSeekRadius = 2f;
        Collider[] colliders = Physics.OverlapSphere(boids[index].transform.position, foodSeekRadius);

        Food closestFood = null;
        float distance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            Food food = collider.GetComponent<Food>();

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


        //seek function
    private void SeekModeControl(int index)
    {
        Vector3 accel = boids[index].Seek(targetObject.transform.position, boids[index].accelMax);

        if (Input.GetMouseButton(0))
        {
            boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
        else if (Input.GetMouseButton(1))
        {
            boids[index].RB.linearVelocity -= accel * Time.fixedDeltaTime;
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
    }

        // puruse function 
    private void PursueModeControl(int index)
    {
        Vector3 accel = boids[index].Pursue(targetObject.transform.position, boids[index].accelMax, boids[index].speedMax);
        Debug.DrawRay(boids[index].transform.position, accel, Color.green);


        if (Input.GetMouseButton(0))
        {
            boids[index].RB.linearVelocity += accel * Time.fixedDeltaTime;
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }
        else if (Input.GetMouseButton(1))
        {
            boids[index].RB.linearVelocity -= accel * Time.fixedDeltaTime;
            Debug.DrawRay(boids[index].transform.position, accel, Color.green);
        }

    }

}
