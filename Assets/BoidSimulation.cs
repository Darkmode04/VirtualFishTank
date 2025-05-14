using System.Collections.Generic;
using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{

    public GameObject boidPrefab = null;
    public int boidToSpawn = 10;

    public List<Boid> boids = null;

    
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

        // Spawn the boids
        for (int i = 0; i < boidToSpawn; i++)
        {
           Vector3 position = new Vector3(Random.Range(0.7f,-1), Random.Range(0, 1.3f), Random.Range(-0.4f, 0));
           Quaternion rotation = Random.rotation;

            GameObject spawnerBoid = Instantiate(boidPrefab, position, rotation);

            spawnerBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0,1,0.5f,1,0.5f,1));
        }




    }

    // Update is called once per frame
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

    }

}
