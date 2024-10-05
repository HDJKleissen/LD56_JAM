using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;

public class NPCCharacter : MonoBehaviour
{
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackMeleeMaxRange;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _targetChooseRange;
    [SerializeField] protected float _targetDetectRange;
    [SerializeField] protected float _throwRotateIntensity;
    [SerializeField] protected float _throwRotateDuration;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer _sprite;

    [SerializeField] private GameObject projectilePrefab;

    public Vector3 Destination => agent.destination;
    public bool AtDestination => Vector3.Distance(transform.position, agent.destination) <= 0.1f;

    protected NPCCharacter target;

    protected List<NPCCharacter> targetedBy = new List<NPCCharacter>();

    public int TargetedByAmount => targetedBy.Count;

    bool attacking;
    float attackTimer = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer <= _attackCooldown)
        {
            attackTimer += Time.deltaTime;
        }

        if (target && Vector3.Distance(target.transform.position, transform.position) < _attackRange)
        {
            agent.destination = transform.position;

            if (!attacking)
            {
                attacking = true;
            }
            
            TryAttack();
        }
        else if (target)
        {
            agent.destination = target.transform.position;
            attacking = false;
        }
        else
        {
            attacking = false;
            ChooseTarget();
        }

        if (attacking)
        {
            return;
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

    private void TryAttack()
    {
        float targetDir = Mathf.Sign((target.transform.position - transform.position).x);
        if (targetDir < 0)
        {
            _sprite.flipX = true;
        }
        else if (targetDir > 0)
        {
            _sprite.flipX = false;
        }

        if(attackTimer >= _attackCooldown)
        {
            attackTimer = 0;
            if (_attackRange <= _attackMeleeMaxRange)
            {
                Destroy(target.gameObject);
                target = null;
            }
            else
            {
                Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
                projectile.transform.position = transform.position;
                projectile.Target = target.transform;

                transform.DOPunchRotation(new Vector3(0, 0, _throwRotateIntensity * targetDir), _throwRotateDuration, 0, 0.6f);
            }
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