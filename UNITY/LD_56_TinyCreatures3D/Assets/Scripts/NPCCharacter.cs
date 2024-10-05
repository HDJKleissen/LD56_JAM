using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCCharacter : MonoBehaviour
{
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _targetChooseRange;
    [SerializeField] protected float _targetDetectRange;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer _sprite;

    public Vector3 Destination => agent.destination;
    public bool AtDestination => Vector3.Distance(transform.position, agent.destination) <= 0.1f;

    protected NPCCharacter target;

    protected List<NPCCharacter> targetedBy = new List<NPCCharacter>();

    public int TargetedByAmount => targetedBy.Count;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target && Vector3.Distance(target.transform.position, transform.position) < _attackRange)
        {
            agent.destination = transform.position;
        }
        else if (target)
        {
            agent.destination = target.transform.position;
        }
        else
        {
            ChooseTarget();
        }

        if (agent.velocity.x < 0)
        {
            _sprite.flipX = true;
        }
        else if (agent.velocity.x > 0)
        {
            _sprite.flipX = false;
        }
    }

    protected virtual void ChooseTarget() { }

    public void SetDestination(Vector3 position)
    {
        agent.destination = position;
    }

    public void SetTargetedBy(NPCCharacter targetedBy)
    {

    }
}