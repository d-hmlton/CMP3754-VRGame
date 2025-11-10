using System.Collections.Generic;
using UnityEngine;

public class CarRoute : MonoBehaviour
{
    public List<Transform> cwps;
    public List<Transform> route;
    public List<Collider> triggers;
    public int routeNumber = 0;
    public int targetCWP = 0;
    public bool go = false;
    public bool stopPlease = false;
    public float initialDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cwps = new List<Transform>();
        GameObject cwp;

        cwp = GameObject.Find("CWP1");
        cwps.Add(cwp.transform);

        cwp = GameObject.Find("CWP2"); cwps.Add(cwp.transform);
        cwp = GameObject.Find("CWP3"); cwps.Add(cwp.transform);
        cwp = GameObject.Find("CWP4"); cwps.Add(cwp.transform);

        SetRoute();

        initialDelay = Random.Range(2.0f, 12.0f);
        transform.position = new Vector3(0.0f, -5.0f, 0.0f);
    }

    void FixedUpdate()
    {
        if (!go)
        {
            initialDelay -= Time.deltaTime;
            if (initialDelay <= 0.0f)
            {
                go = true;
                SetRoute();
            }
            else return;
        }

        if (stopPlease)
        {
            return;
        }

        Vector3 displacement = route[targetCWP].position - transform.position;
        displacement.y = 0;
        float dist = displacement.magnitude;

        if (dist < 0.1f)
        {
            targetCWP++;
            if (targetCWP >= route.Count)
            {
                SetRoute();
                return;
            }
        }

        //calculate velocity for this frame
        Vector3 velocity = displacement;
        velocity.Normalize();
        velocity *= 10.0f;

        //apply velocity
        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(newPosition);

        //align to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity,
        10.0f * Time.deltaTime, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        rb.MoveRotation(rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag != "CarFront")
        {
            triggers.Add(other);
            stopPlease = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggers.Remove(other);
        if (triggers.Count <= 0)
        {
            stopPlease = false;
        }
    }

    void SetRoute()
    {
        //randomise the next route
        routeNumber = Random.Range(0, 4);

        //set the route waypoints
        if (routeNumber == 0) route = new List<Transform> { cwps[0], cwps[1] };
        else if (routeNumber == 1) route = new List<Transform> { cwps[0], cwps[1], cwps[2], cwps[3] };
        else if (routeNumber == 2) route = new List<Transform> { cwps[2], cwps[3] };
        else if (routeNumber == 3) route = new List<Transform> { cwps[2], cwps[3], cwps[0], cwps[1] };

        //initialise position and waypoint counter
        transform.position = new Vector3(route[0].position.x, 0.55f,
            route[0].position.z);
        targetCWP = 1;
    }
}
