using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    RoadGenerator road;
    public Rigidbody rb;

    public Vector3 direction;

    GameObject navObject;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public float cal;

    public bool isLookAgent = false;

    public bool isArriveDestination = false;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        rb = GetComponent<Rigidbody>();

        CreateNavMeshAgentObject();
    }

    private void Update()
    {
        CalculateDirection();
        CheckIsSetDestination();

        //if (isLookAgent)
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        //else
        //    StartCoroutine("UpdateAIRotation");

        //StartCoroutine("LookNavMeshAgent");

        //if (cal > 8)
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        //else
        //    transform.rotation = agent.transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);

    }

    void CreateNavMeshAgentObject()
    {
        navObject = new GameObject("AI NavMesh Agent");
        navObject.transform.position = transform.position;
        
        agent = navObject.AddComponent<NavMeshAgent>();

        agent.speed = 20f;
        agent.radius = 0.1f;
        agent.acceleration = 50f;
        agent.avoidancePriority = 0;
        agent.tag = "AI";
        agent.autoBraking = true;

        //for (int i = 1; i < road.vertices.Length; ++i)
        
        int destination = Random.Range(1, road.middleRoadNum);
        agent.SetDestination(road.passibleItemPlace[destination]);
    }

    void CheckIsSetDestination()
    {
        if( agent.transform.position == agent.destination)
        {
            isArriveDestination = true;
        }
    }

    void CalculateDirection()
    {
        direction = agent.transform.position - transform.position;
        cal = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
        //print(cal);

        if (cal > 5)// 8밑으로 줄이지 말것
            agent.isStopped = true;
        else
            agent.isStopped = false;
    }

    IEnumerator goBack()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);

        yield return new WaitForSeconds(1f);
    }

    IEnumerator UpdateAIRotation()
    {
        yield return new WaitForSeconds(0.1f);
        transform.rotation = agent.transform.rotation;
    }

    IEnumerator LookNavMeshAgent()
    {
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "mapBoxCollider")
        {
            isLookAgent = true;
            //StartCoroutine("goBack");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "mapBoxCollider")
        {
            isLookAgent = true;
        }
        //else if (collision.transform.tag == "map") 값 안들어감
        //{
        //    AIRBController rbController = GetComponent<AIRBController>();
        //    rbController.topGearTorque = 1000;
        //    rbController.firstGearTorque = 1000;
        //    rbController.maxSpeed = 50;
        //    print("map");
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "mapBoxCollider")
        {
            new WaitForSeconds(2f);
            isLookAgent = false;
        }
        //else if (collision.transform.tag == "map") 값 안들어감
        //{
        //    AIRBController rbController = GetComponent<AIRBController>();
        //    rbController.topGearTorque = 800;
        //    rbController.firstGearTorque = 800;
        //    rbController.maxSpeed = 30;
        //}
    }
    */
}
