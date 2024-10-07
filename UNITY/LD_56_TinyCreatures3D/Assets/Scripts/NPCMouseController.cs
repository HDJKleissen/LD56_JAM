using Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCMouseController : NPCCharacter
{
    [SerializeField] Disc _selectedIndicator;

    public bool Selected { get; private set; }

    public Guid LastCommandGuid;

    protected override void ChooseAttackTarget()
    {
        if (followTarget || resourceTarget || resourceGroupTarget || townHallTarget)
        {
            return;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _targetDetectRange);
        List<NPCCharacter> possibleTargets = new List<NPCCharacter>();
        NPCCharacter closest = null;
        foreach (Collider hitCollider in hitColliders)
        {
            NPCEnemyController enemy = hitCollider.GetComponentInChildren<NPCEnemyController>();
            BossController boss = hitCollider.GetComponentInChildren<BossController>();
            if (enemy)
            {
                if (!closest)
                {
                    closest = enemy;
                }
                else
                {
                    if(Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                    {
                        closest = enemy;
                    }
                }
                possibleTargets.Add(enemy);
            }
            if (boss)
            {
                if (!closest)
                {
                    closest = boss;
                }
                else
                {
                    if (Vector3.Distance(transform.position, boss.transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                    {
                        closest = boss;
                    }
                }
                possibleTargets.Add(boss);

            }
        }

        if (possibleTargets.Count == 0) { return; }

        possibleTargets.OrderBy(mouse => -Vector3.Distance(mouse.transform.position, transform.position));

        bool atLeastOneTargetInRange = possibleTargets.Exists(mouse => Vector3.Distance(mouse.transform.position, transform.position) <= _targetChooseRange);

        if (possibleTargets.Count > 0 && atLeastOneTargetInRange)
        {
            //target = possibleTargets[Random.Range(0, possibleTargets.Count)];
            SetAttackTarget(closest);
        }
    }
    private void OnTriggerStay(Collider collider)
    {
        NPCMouseController mouse = collider.GetComponent<NPCMouseController>();
        if (mouse && mouse.AtDestination && LastCommandGuid == mouse.LastCommandGuid)
        {
            agent.destination = transform.position;
        }
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;
        _selectedIndicator.enabled = selected;
    }

}
