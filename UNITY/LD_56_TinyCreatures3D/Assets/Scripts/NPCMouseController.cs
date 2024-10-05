using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCMouseController : NPCCharacter
{
    [SerializeField] SpriteRenderer _selectedIndicator;

    public bool Selected { get; private set; }

    public Guid LastCommandGuid;

    protected override void ChooseAttackTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _targetDetectRange);
        List<NPCCharacter> possibleTargets = new List<NPCCharacter>();
        foreach (Collider hitCollider in hitColliders)
        {
            NPCEnemyController mouse = hitCollider.GetComponentInChildren<NPCEnemyController>();
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
            SetAttackTarget(possibleTargets[0]);
        }
    }
    private void OnTriggerStay(Collider collider)
    {
        NPCMouseController mouse = collider.GetComponent<NPCMouseController>();
        if (mouse && mouse.AtDestination && LastCommandGuid == mouse.LastCommandGuid)
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
