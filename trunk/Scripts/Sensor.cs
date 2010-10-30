using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour
{
    public Move2D parent;
    public bool collision = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
		collision = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Floor" && other.tag != "Flock")
        {
			collision = true;
        }
    }

}
