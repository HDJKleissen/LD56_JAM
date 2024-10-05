using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCEnemyController : NPCCharacter
{
    private void Start()
    {
        NPCManager.RegisterNPC(this);
    }

    private void OnTriggerStay(Collider collider)
    {
        Debug.Log($"Bing bong {name}?");
        NPCEnemyController enemy = collider.GetComponent<NPCEnemyController>();
        if (enemy && enemy.AtDestination && enemy.Destination == agent.destination)
        {
            Debug.Log("Bing bong " + name);
            agent.destination = transform.position;
        }
    }

    protected override void ChooseTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _targetDetectRange);
        List<NPCCharacter> possibleTargets = new List<NPCCharacter>();
        foreach (Collider hitCollider in hitColliders)
        {
            NPCMouseController mouse = hitCollider.GetComponentInChildren<NPCMouseController>();
            if (mouse)
            {
                possibleTargets.Add(mouse);
            }
        }

        if (possibleTargets.Count == 0) { return; }

        possibleTargets.OrderBy(mouse => Vector3.Distance(mouse.transform.position, transform.position));

        bool atLeastOneTargetInRange = possibleTargets.Exists(mouse => Vector3.Distance(mouse.transform.position, transform.position) <= _targetChooseRange);

        if (possibleTargets.Count > 0 && atLeastOneTargetInRange)
        {
            //target = possibleTargets[Random.Range(0, possibleTargets.Count)];
            target = possibleTargets[0];
        }
    }
}