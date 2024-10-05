using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using System;
using Shapes;

public class NPCCharacter : MonoBehaviour
{
    [SerializeField] protected float MaxHealth;
    [SerializeField] protected float Health;
    [SerializeField] private Line HealthBar;

    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackMeleeMaxRange;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _targetChooseRange;
    [SerializeField] protected float _targetDetectRange;
    [SerializeField] protected float _throwRotateIntensity;
    [SerializeField] protected float _throwRotateDuration;
    [SerializeField] protected float _attackDamage;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer _sprite;

    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Sprite _meleeSpriteIdle;
    [SerializeField] private Sprite _meleeSpriteAttack;
    [SerializeField] private Sprite _rangedSpriteIdle;
    [SerializeField] private Sprite _rangedSpriteAttack;
    [SerializeField] private Sprite _workerSpriteIdle;
    [SerializeField] private Sprite _workerSpriteAttack;

    public Vector3 Destination => agent.destination;
    public bool AtDestination => Vector3.Distance(transform.position, agent.destination) <= 0.3f;

    protected Transform followTarget;
    protected NPCCharacter attackTarget;

    float healthBarXLeft, healthBarXRight;

    bool IsMelee => _attackRange <= _attackMeleeMaxRange;
    bool IsRanged => _attackRange > _attackMeleeMaxRange;
    bool IsWorker => _attackRange <= 0;

    public void Damage(float damage, NPCCharacter source, CharacterTeam sourceTeam)
    {
        if(sourceTeam != GetComponent<TeamSelector>().CharacterTeam)
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

        }

        HealthBar.enabled = true;
        Health -= damage;

        DOTween.To(
                () => HealthBar.End.x,
                value => HealthBar.End = new Vector3(value, HealthBar.End.y, HealthBar.End.z),
                healthBarXLeft + (Health / MaxHealth)/2,
                0.2f
                ).OnComplete(() =>
                {
                    if (Health <= 0)
                    {
                        Destroy(gameObject);
                    }
                });
    }

    protected List<NPCCharacter> targetedBy = new List<NPCCharacter>();

    public int TargetedByAmount => targetedBy.Count;

    bool attacking;
    float attackTimer = 0;

    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;

        healthBarXLeft = HealthBar.Start.x;
        healthBarXRight = HealthBar.End.x;
        HealthBar.enabled = false;

        if (IsMelee)
        {
            _sprite.sprite = _meleeSpriteIdle;
        }
        if (IsRanged)
        {
            _sprite.sprite = _rangedSpriteIdle;
        }
        if (IsWorker)
        {
            _sprite.sprite = _workerSpriteIdle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer <= _attackCooldown)
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTarget && Vector3.Distance(attackTarget.transform.position, transform.position) < _attackRange)
        {
            agent.destination = transform.position;

            if (!attacking)
            {
                attacking = true;
            }

            TryAttack();
        }
        else if (attackTarget)
        {
            agent.destination = attackTarget.transform.position;
            attacking = false;
        }
        else if (followTarget)
        {
            if (agent.destination != followTarget.transform.position)
            {
                agent.destination = followTarget.transform.position;
            }
            attacking = false;
        }
        else
        {
            attacking = false;
            ChooseAttackTarget();
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
        float targetDir = Mathf.Sign((attackTarget.transform.position - transform.position).x);
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
            if (IsMelee)
            {
                _sprite.sprite = _meleeSpriteAttack;
            }
            if (IsRanged)
            {
                _sprite.sprite = _rangedSpriteAttack;
            }
            attackTimer = 0;
            if (_attackRange <= _attackMeleeMaxRange)
            {
                attackTarget.Damage(_attackDamage, this, GetComponent<TeamSelector>().CharacterTeam);
                attackTarget = null;
            }
            else
            {
                Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
                projectile.transform.position = transform.position;
                projectile.Target = attackTarget.transform;
                projectile.Damage = _attackDamage;
                projectile.source = this;

                transform.DOPunchRotation(new Vector3(0, 0, _throwRotateIntensity * targetDir), _throwRotateDuration, 0, 0.6f);
            }
            StartCoroutine(CoroutineHelper.DelaySeconds(() =>
            {
                if (IsMelee)
                {
                    _sprite.sprite = _meleeSpriteIdle;
                }
                if (IsRanged)
                {
                    _sprite.sprite = _rangedSpriteIdle;
                }
            }, 0.25f));
        }
    }

    protected virtual void ChooseAttackTarget() { }

    public void SetDestination(Vector3 position)
    {
        agent.destination = position;
    }

    public void SetTargetedBy(NPCCharacter character)
    {
        targetedBy.Add(character);
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        attackTarget = null;
    }
    public void SetAttackTarget(NPCCharacter target)
    {
        if (!IsWorker)
        {
            attackTarget = target;
        }
    }
}