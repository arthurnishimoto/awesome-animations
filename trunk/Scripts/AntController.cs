using UnityEngine;
using System.Collections;

public class AntController : MonoBehaviour {
	
	float d_flockDistance = 0.5f;
    float d_separationDistance = 0.4f;
	float d_cohesionWeight = 1.0f; // Multiplier for cohesion vector
	float d_separationWeight = 1.0f; // Multiplier for separation vector
	float d_alignmentWeight = 0.05f; // Multiplier for alignment vector
	float d_targetWeight = 0.5f; // Multiplier for target vector
	float d_speed = 0.005f; // movement speed in meters/sec
	float d_turningForwardSpeed = 0.005f; // movement speed in meters/sec
    float d_turningSpeed = 1.0f;
	
	public GameObject target;
	public float flockDistance;
    public float separationDistance;
	public float cohesionWeight; // Multiplier for cohesion vector
	public float separationWeight; // Multiplier for separation vector
	public float alignmentWeight; // Multiplier for alignment vector
	public float targetWeight; // Multiplier for target vector
	
	public float speed; // movement speed in meters/sec
	public float turningForwardSpeed; // movement speed in meters/sec
    public float turningSpeed;
	
	public bool resetToDefault = false;
	
	// Use this for initialization
	void Start () {
		flockDistance = d_flockDistance;
		separationDistance = d_separationDistance;
		cohesionWeight = d_cohesionWeight; // Multiplier for cohesion vector
		separationWeight = d_separationWeight; // Multiplier for separation vector
		alignmentWeight = d_alignmentWeight; // Multiplier for alignment vector
		targetWeight = d_targetWeight; // Multiplier for target vector
		
		speed = d_speed; // movement speed in meters/sec
		turningForwardSpeed = d_turningForwardSpeed; // movement speed in meters/sec
		turningSpeed = d_turningSpeed;
		resetToDefault = false;
	}
	
	// Update is called once per frame
	void Update () {
		if( resetToDefault )
		{
			cohesionWeight = d_cohesionWeight; // Multiplier for cohesion vector
			separationWeight = d_separationWeight; // Multiplier for separation vector
			alignmentWeight = d_alignmentWeight; // Multiplier for alignment vector
			targetWeight = d_targetWeight; // Multiplier for target vector
		
			speed = d_speed; // movement speed in meters/sec
			turningForwardSpeed = d_turningForwardSpeed; // movement speed in meters/sec
			turningSpeed = d_turningSpeed;
			resetToDefault = false;
		}
		
		GameObject[] boids = GameObject.FindGameObjectsWithTag("Flock");
		foreach(GameObject boid in boids )
		{
			Move2D boidComponent = boid.GetComponent<Move2D>();
			if( boidComponent != null )
			{
				boidComponent.target = target;
				boidComponent.cohesionWeight = cohesionWeight; // Multiplier for cohesion vector
				boidComponent.separationWeight = separationWeight; // Multiplier for separation vector
				boidComponent.alignmentWeight = alignmentWeight; // Multiplier for alignment vector
				boidComponent.targetWeight = targetWeight; // Multiplier for target vector
				boidComponent.speed = speed; // movement speed in meters/sec
				boidComponent.turningForwardSpeed = turningForwardSpeed; // movement speed in meters/sec
    			boidComponent.turningSpeed = turningSpeed;
			}
		}
	}
}
