using UnityEngine;
using System.Collections;
using System;

// Note: This is programmed for objects that consider +Z as up
public class Move2D : MonoBehaviour
{
    public Transform thisObject;

    public GameObject target;
	public Sensor leftSensor;
	public Sensor rightSensor;
	public Sensor centerSensor;
	public Sensor stopSensor;
	
	public float turnAngle = 0;
	public float lastTurnAngle = 0;
	bool turning = false;
	
    float[] position = { 0, 0, 0 };
    float[] destination = { 0, 0, 0 };
    float[] rotation = { 0, 0, 0 };

    float destinationAngleXZ = 0;
    float destinationAngleXY = 0;

    double distanceToTarget = 0;

    float speed = 0.005f; // movement speed in meters/sec
	float turningForwardSpeed = 0.005f; // movement speed in meters/sec
    public float turningSpeed = 0.1f;

    float flockDistance = 0.5f;
    float separationDistance = 0.4f;
	int separationStrength = 6; // Multiplies separation vector if objects are within half the separation distance of each other.
	
	float cohesionWeight = 1.0f; // Multiplier for cohesion vector
	float separationWeight = 1.0f; // Multiplier for separation vector
	float alignmentWeight = 0.05f; // Multiplier for alignment vector
	float targetWeight = 0.05f; // Multiplier for target vector
	public bool flock = true; // Flag used to stop flocking during collision
	
    // Use this for initialization
    void Start()
    {
        thisObject = this.transform;
        thisObject.position = new Vector3(UnityEngine.Random.Range(-2.0f, 2.0f), 0.06f, UnityEngine.Random.Range(-2.0f, 2.0f));
        thisObject.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);

        //destination[0] = UnityEngine.Random.Range(-200.0f, 200.0f);
        //destination[1] = UnityEngine.Random.Range(0.0f, 0.0f);
        //destination[2] = UnityEngine.Random.Range(-200.0f, 200.0f);
        //target = (GameObject)Instantiate(GameObject.Find("Pillar"), new Vector3(destination[0], destination[1], destination[2]), transform.rotation);
    }// Start

    // Update is called once per frame
    void Update()
    {
        position[0] = thisObject.position.x;
        position[1] = thisObject.position.y;
        position[2] = thisObject.position.z;
		
        // Get user input
        //float y = Input.GetAxis("Horizontal");
        //float x = Input.GetAxis("Vertical");

        // Move object based on input
        //transform.Translate( speed, 0, 0 );
        //transform.Rotate( 0, y, 0 );

        // Flocking movement
        // Get current position
        destination[0] = centerSensor.transform.position.x;//position[0];
        destination[2] = centerSensor.transform.position.z;//position[2];
		//destination[0] = position[0];
        //destination[2] = position[2];
		
        Vector3 cohesionVector = Cohesion() * cohesionWeight;
        Vector3 separationVector = Separation() * separationWeight;
        Vector3 alignmentVector = Alignment() * alignmentWeight;
        Vector3 targetVector = FollowTarget() * targetWeight;
		
		if( flock ){
			destination[0] += cohesionVector.x;
			destination[2] += cohesionVector.z;
			destination[0] += separationVector.x;
			destination[2] += separationVector.z;
			destination[0] += targetVector.x;
			destination[2] += targetVector.z;
			//destination[1] += cohesionVector.y;
			//destination[1] += separationVector.y;

			transform.Rotate( 0, alignmentVector.y, 0 );
		}
        //Debug.Log("Position - Distance to Destination: " + distanceToDestination );
        RotateToDestination();
        Move();

    }// Update

    void RotateToDestination()
    {
        destinationAngleXZ = -(float)GetAngleBetween(position[0], position[2], destination[0], destination[2]);
		destinationAngleXY = (float)GetAngleBetween(position[0], position[1], destination[0], destination[1]);

        if (destinationAngleXZ < 0)
        {
            destinationAngleXZ += 360.0f;
        }
        if (destinationAngleXY < 0)
        {
            destinationAngleXY += 360.0f;
        }

        // Get the current rotation of this object
        rotation[0] = thisObject.eulerAngles.x;
        rotation[1] = thisObject.eulerAngles.y;
        rotation[2] = thisObject.eulerAngles.z;

        //Debug.Log("Position - Current : " + position[0] + "," + position[2] + " Target : " + destination[0] + "," + destination[2]);
        //Debug.Log("RotationXZ - Current : " + rotation[1] + " Target : " + destinationAngleXZ + " RotationXY - Current : " + rotation[2] + " Target : " + destinationAngleXY);

        // ---- Rotation on the XZ plane ----
        // Special rotation cases involving 360-0 crossover
        if (destinationAngleXZ - rotation[1] > 180)
            transform.Rotate(0, -turningSpeed, 0);
        else if (destinationAngleXZ - rotation[1] < -180)
            transform.Rotate(0, turningSpeed, 0);

        // Normal rotation cases
        else if (rotation[1] < destinationAngleXZ - 1)
            transform.Rotate(0, turningSpeed, 0);
        else if (rotation[1] > destinationAngleXZ + 1)
            transform.Rotate(0, -turningSpeed, 0);
        /*
        // ---- Rotation on the XY plane ----
        // Special rotation cases involving 360-0 crossover
        if( destinationAngleXY - rotation[2] > 180 )
            transform.Rotate( 0, 0, -turningSpeed );
        else if( destinationAngleXY - rotation[2] < -180 )
            transform.Rotate( 0, 0, turningSpeed );
		
        // Normal rotation cases
        else if (rotation[2] < destinationAngleXY - 1)
            transform.Rotate( 0, 0, turningSpeed );
        else if (rotation[2] > destinationAngleXY + 1 )
            transform.Rotate( 0, 0, -turningSpeed );
        */
		//if( !leftSensor.turn && rightSensor.turn )
		//	transform.Rotate( 0, 10, 0 );
		//if( leftSensor.turn || rightSensor.turn )
		//	transform.Rotate( 0, 10, 0 );
		//else if( leftSensor.turn && rightSensor.turn ){
			//transform.Rotate( 0, 50, 0 );
		//}
    }// RotateToDestination
	
	public void setTurnAngle( float angle ){
		if( turnAngle != 0 )
			lastTurnAngle = turnAngle;
		turnAngle = angle;
	}
	
	public void Turn( float angle ){
		//if( stopSensor.collisions == 0 )
			transform.Rotate( 0, angle, 0 );
	}
	
    void Move()
    {
        // Capital ship turning (Reverse sliding)
        //transform.Translate(speed * (float)Math.Cos(-destinationAngleXZ), 0.0f, speed * (float)Math.Sin(-destinationAngleXZ), Space.World );
		if( centerSensor.collision )
		{
			if( turnAngle != 0 )
				transform.Rotate(0, turnAngle, 0);
			else
				transform.Rotate(0, lastTurnAngle, 0);
		}
		else
		{
			setTurnAngle( 0 );
		}
		
        // Car/Fighter turning
		if( !stopSensor.collision && (leftSensor.collision || rightSensor.collision)  )
		{	// Stop or slow down until done rotating away from obstacle
			transform.Translate(turningForwardSpeed, 0, 0);
			
			if( leftSensor.collision && !rightSensor.collision && turnAngle == 0 )
				setTurnAngle( turningSpeed );
			else if( !leftSensor.collision && rightSensor.collision && turnAngle == 0 )
				setTurnAngle( -turningSpeed );
			else if( leftSensor.collision && rightSensor.collision && turnAngle == 0 )
				setTurnAngle( lastTurnAngle );				
		}
		else if( stopSensor.collision )
		{
			// Stop or slow down and rotate away from obstacle - case where stop sensor is triggered
			// but no turning motions is currently going on
			//transform.Translate(0, 0, 0);
			if( turnAngle != 0 )
				transform.Rotate(0, turnAngle, 0);
			else
				transform.Rotate(0, lastTurnAngle, 0);
			flock = false;
		}
		else
		{
			flock = true;
			transform.Translate(speed, 0, 0);
		}
    }// Move

    Vector3 Cohesion()
    {
        Vector3 newVector = new Vector3(0,0,0);

        GameObject[] flock = GameObject.FindGameObjectsWithTag("Flock");
		int flockmates = 0;

        // Calculate the center of mass of the flock
        for (int i = 0; i < flock.Length; i++)
        {
            Vector3 flockmatePos = flock[i].transform.position;
			double distanceToFlockmate = GetDistanceBetween(position[0], position[1], position[2], flockmatePos.x, flockmatePos.y, flockmatePos.z);
			
            // Add vectors of flockmates, ignoring 
			if (this != flock[i] && distanceToFlockmate <= flockDistance)
            {
                newVector += flockmatePos;
				flockmates++;
            }// if
        }// for
		if( flockmates > 0 )
			newVector /= flockmates;

        // Show center of mass of flock
        //target.transform.position = newVector;

        return newVector - this.transform.position;
    }// cohesion

    Vector3 Separation(){
        Vector3 newVector = new Vector3(0,0,0);

        GameObject[] flock = GameObject.FindGameObjectsWithTag("Flock");

        // Calculate the center of mass of the flock
        for (int i = 0; i < flock.Length; i++)
        {
            Vector3 flockmatePos = flock[i].transform.position;
            double distanceToFlockmate = GetDistanceBetween(position[0], position[1], position[2], flockmatePos.x, flockmatePos.y, flockmatePos.z);

            if (this != flock[i] && (flockmatePos - thisObject.transform.position).magnitude < separationDistance && distanceToFlockmate <= flockDistance)
            {
                newVector -= flockmatePos - thisObject.transform.position;
				
				if( distanceToFlockmate < separationDistance / 2 )
					newVector -= separationStrength * (flockmatePos - thisObject.transform.position );
            }// if
        }// for
        return newVector;
    }// Separation

    Vector3 Alignment()
    {
        Vector3 newVector = new Vector3(0,0,0);

        GameObject[] flock = GameObject.FindGameObjectsWithTag("Flock");
		int flockmates = 0;
		
        // Calculate the average direction of the flock
        for (int i = 0; i < flock.Length; i++)
        {
			Vector3 flockmatePos = flock[i].transform.position;
            double distanceToFlockmate = GetDistanceBetween(position[0], position[1], position[2], flockmatePos.x, flockmatePos.y, flockmatePos.z);
			
            if (this != flock[i] && distanceToFlockmate <= flockDistance)
            {
                newVector += flock[i].transform.eulerAngles;
				flockmates++;
            }// if			
        }// for
		if( flockmates > 0 )
			newVector /= flockmates;

        return newVector - this.transform.eulerAngles;
    }// Alignment

    Vector3 FollowTarget()
    {
        Vector3 newVector = new Vector3(0, 0, 0);

        if (target != null)
        {
            newVector.x = target.transform.position.x;
			newVector.y = target.transform.position.y;
            newVector.z = target.transform.position.z;
            //distanceToTarget = GetDistanceBetween(position[0], position[2], newVector.x, newVector.z);
			/*
			if (distanceToTarget < 5.0)
			{
				//GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
				//int rand = UnityEngine.Random.Range( 0, targets.Length );
				//target = targets[rand];

				//destination[0] = target.transform.position.x;
				//destination[1] = target.transform.position.y;
				//destination[2] = target.transform.position.z;

				//newVector.x = UnityEngine.Random.Range(-200.0f, 200.0f);
				//newVector.y = UnityEngine.Random.Range(0.0f, 0.0f);
				//newVector.z = UnityEngine.Random.Range(-200.0f, 200.0f);

				//target.transform.position = newVector;
			}
			*/
		
        }// if
        return newVector - this.transform.position;
    }// FollowTarget

    double GetDistanceBetween(float x, float y, float x2, float y2)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(x - x2), 2.0) + Math.Pow(Math.Abs(y - y2), 2.0));
    }// GetDistanceBetween

    double GetDistanceBetween(float x, float y, float z, float x2, float y2, float z2)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(x - x2), 2.0) + Math.Pow(Math.Abs(y - y2), 2.0) + Math.Pow(Math.Abs(z - z2), 2.0));
    }// GetDistanceBetween

    double GetAngleBetween(float x, float y, float x2, float y2)
    {
        return Math.Atan2(y2 - y, x2 - x) * 180.0 / Math.PI;
    }// GetAngleBetween
	
	public float getTurnAngle(){
		return turnAngle;
	}
}// class
