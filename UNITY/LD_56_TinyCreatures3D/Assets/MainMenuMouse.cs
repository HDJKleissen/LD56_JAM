using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainMenuMouse : MonoBehaviour
{
    public NavMeshAgent agent;
    public float NavRange;
    public Vector3 currDestination;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        agent.Move(Vector3.left * 4 * Time.deltaTime);
    }
}
