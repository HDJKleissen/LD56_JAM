using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCEnemyController : NPCCharacter
{
    private void OnTriggerStay(Collider collider)
    {
        Debug.Log($"Bing bong {name}?");
        NPCEnemyController enemy = collider.GetComponent<NPCEnemyController>();
        if (enemy && enemy.AtDestination && Vector3.Distance(enemy.Destination, agent.destination) < 0.3f)
        {
            agent.destination = transform.position;
        }
    }

    protected override void ChooseAttackTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _targetDetectRange);
        List<NPCCharacter> possibleTargets = new List<NPCCharacter>();

        NPCMouseController closest = null;
        foreach (Collider hitCollider in hitColliders)
        {
            NPCMouseController enemy = hitCollider.GetComponentInChildren<NPCMouseController>();
            if (enemy)
            {
                if (!closest)
                {
                    closest = enemy;
                }
                else
                {
                    if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                    {
                        closest = enemy;
                    }
                }
                possibleTargets.Add(enemy);
            }
        }

        if (possibleTargets.Count == 0) { return; }

        possibleTargets.OrderBy(mouse => Vector3.Distance(mouse.transform.position, transform.position));

        bool atLeastOneTargetInRange = possibleTargets.Exists(mouse => Vector3.Distance(mouse.transform.position, transform.position) <= _targetChooseRange);

        if (possibleTargets.Count > 0 && atLeastOneTargetInRange)
        {
            SetAttackTarget(closest);
            //SetAttackTarget(possibleTargets[Random.Range(0, possibleTargets.Count)]);
            //SetAttackTarget(possibleTargets[0]);
        }
    }
}