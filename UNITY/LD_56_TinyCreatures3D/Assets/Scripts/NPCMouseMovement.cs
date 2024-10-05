using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMouseMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] CharacterController _controller;
    [SerializeField] SpriteRenderer _selectedIndicator;
    [SerializeField] float _moveSpeed;

    Vector3 _destination;

    public Vector3 Destination => agent.destination;
    public bool AtDestination => Vector3.Distance(transform.position, agent.destination) <= 0.1f;
    public bool Selected { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionStay(Collision collision)
    {
        NPCMouseMovement mouse = collision.gameObject.GetComponent<NPCMouseMovement>();
        if (mouse && mouse.AtDestination && mouse.Destination == _destination)
        {
            Debug.Log("Bing bong " + name);
            agent.destination = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AtDestination)
        {
            agent.destination = transform.position;
        }
    }

    public void SetDestination(Vector3 position)
    {
        //AtDestination = false;
        agent.destination = position;
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;
        _selectedIndicator.enabled = selected;
    }
}
