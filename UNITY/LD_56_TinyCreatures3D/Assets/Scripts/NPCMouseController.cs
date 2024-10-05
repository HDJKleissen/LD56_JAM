using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMouseController : NPCCharacter
{
    [SerializeField] SpriteRenderer _selectedIndicator;

    public bool Selected { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        NPCManager.RegisterNPC(this);
    }


    private void OnTriggerStay(Collider collider)
    {
        Debug.Log($"Bing bong {name}?");
        NPCMouseController mouse = collider.GetComponent<NPCMouseController>();
        if (mouse && mouse.AtDestination && mouse.Destination == agent.destination)
        {
            Debug.Log("Bing bong " + name);
            agent.destination = transform.position;
        }
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;
        _selectedIndicator.enabled = selected;
    }
}
