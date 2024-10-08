﻿using UnityEngine;
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
    [SerializeField] protected Line HealthBar;

    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackMeleeMaxRange;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _targetChooseRange;
    [SerializeField] protected float _targetDetectRange;
    [SerializeField] protected float _throwRotateIntensity;
    [SerializeField] protected float _throwRotateDuration;
    [SerializeField] protected float _attackDamage;
    [SerializeField] protected int _mineStrength;
    [SerializeField] protected float _mineTime;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer _sprite;
    [SerializeField] protected SpriteRenderer _carryingSprite;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject mineParticlesPrefab;

    [SerializeField] private Sprite _meleeSpriteIdle;
    [SerializeField] private Sprite _meleeSpriteAttack;
    [SerializeField] private Sprite _rangedSpriteIdle;
    [SerializeField] private Sprite _rangedSpriteAttack;
    [SerializeField] private Sprite _workerSpriteIdle;
    [SerializeField] private Sprite _workerSpriteAttack;

    public Vector3 Destination => agent.destination;
    public bool AtDestination => Vector3.Distance(transform.position, agent.destination) <= 0.2f;
    public bool AtFollowDestination => Vector3.Distance(transform.position, followTarget.position) <= 1f;
    public bool AtResourceDestination => Vector3.Distance(transform.position, resourceTarget.transform.position) <= 1f;
    public bool AtTownHallDestination => Vector3.Distance(transform.position, townHallTarget.transform.position) <= 1f;

    protected Transform followTarget;
    public NPCCharacter attackTarget;
    [SerializeField] protected Resource resourceTarget;
    protected ResourceGroup resourceGroupTarget;
    protected TownHall townHallTarget;

    protected float healthBarXLeft, healthBarXRight;

    public bool IsMelee;
    public bool IsRanged;
    public bool IsWorker;
    public bool DontAutoMine;

    bool mining;

    bool carrying = false;
    int carryingAmount;
    ResourceType carringResourceType;

    protected List<NPCCharacter> targetedBy = new List<NPCCharacter>();

    public int TargetedByAmount => targetedBy.Count;

    bool attacking;
    protected float attackTimer = 0;

    public void Damage(float damage, NPCCharacter source, CharacterTeam sourceTeam)
    {
        if (sourceTeam != GetComponent<TeamSelector>().CharacterTeam)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _targetDetectRange);
            List<NPCCharacter> possibleTargets = new List<NPCCharacter>();
            foreach (Collider hitCollider in hitColliders)
            {
                NPCCharacter mouse = hitCollider.GetComponentInChildren<NPCCharacter>();
                if (mouse && mouse.GetComponent<TeamSelector>().CharacterTeam == GetComponent<TeamSelector>().CharacterTeam)
                {
                    if (!mouse.attackTarget)
                    {
                        StartCoroutine(CoroutineHelper.DelayOneFixedFrame(() =>
                        {
                            if (!mouse.attackTarget)
                            {
                                mouse.SetAttackTarget(source);
                            }
                        }));
                    }
                }
            }

            if (!attackTarget)
            {
                StartCoroutine(CoroutineHelper.DelayOneFixedFrame(() =>
                {
                    if (!attackTarget)
                    {
                        SetAttackTarget(source);
                    }
                }));
            }
        }

        HealthBar.enabled = true;
        Health -= damage;
        HealthBar.End = new Vector3(Mathf.Clamp(healthBarXLeft + (Health / MaxHealth) / 2, healthBarXLeft, healthBarXRight), HealthBar.End.y, HealthBar.End.z);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;

        healthBarXLeft = HealthBar.Start.x;
        healthBarXRight = HealthBar.End.x;
        HealthBar.enabled = false;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        if (agent != null)
        {
            agent.enabled = true;
            agent.Move(new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, UnityEngine.Random.Range(-0.2f, 0.2f)));
        }
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
            //if (!DontAutoMine)
            //{
            //    StartCoroutine(CoroutineHelper.DelaySeconds(() =>
            //  {
            //      ResourceGroup[] groups = FindObjectsByType<ResourceGroup>(FindObjectsSortMode.None);
            //      ResourceGroup closest = null;
            //      float closestDistance = float.MaxValue;
            //      for (int i = 0; i < groups.Length; i++)
            //      {
            //          float distance = Vector3.Distance(transform.position, groups[i].transform.position);
            //          if (distance < closestDistance)
            //          {
            //              closestDistance = distance;
            //              closest = groups[i];
            //          }
            //      }
            //      if (closest != null)
            //      {
            //          SetResourceTarget(closest.RequestResource());
            //      }
            //  }, 0.2f));
            //}
        }
        //agent.avoidancePriority = UnityEngine.Random.Range(0, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWorker)
        {
            if (resourceTarget)
            {
                if (carrying)
                {
                    if (!townHallTarget)
                    {
                        TownHall closestHall = TownHall.FindClosest(transform.position);
                        townHallTarget = closestHall;
                        agent.destination = closestHall.transform.position;
                    }
                    if (AtTownHallDestination)
                    {
                        CrystalCollection.Add(carryingAmount);
                        carrying = false;
                        carryingAmount = 0;
                        carringResourceType = ResourceType.None;
                        _carryingSprite.sprite = null;
                        resourceTarget = resourceTarget.Group.RequestResource();
                    }
                }
                else if (AtResourceDestination)
                {
                    if (!mining)
                    {
                        resourceGroupTarget = resourceTarget.Group;
                        if (resourceTarget.MiceMiningAmount <= 1)
                        {
                            resourceTarget.StartMine();
                            mining = true;
                            agent.destination = transform.position;
                            StartCoroutine(CoroutineHelper.DelaySeconds(() =>
                            {
                                if (resourceTarget == null)
                                {
                                    resourceTarget = resourceGroupTarget.RequestResource();
                                    return;
                                }
                                _sprite.sprite = _workerSpriteAttack;
                                GameObject particles = Instantiate(mineParticlesPrefab);
                                particles.transform.position = (transform.position + resourceTarget.transform.position) / 2;
                            }, _mineTime - 0.2f));
                            StartCoroutine(CoroutineHelper.DelaySeconds(() =>
                            {
                                if (resourceTarget == null)
                                {
                                    resourceTarget = resourceGroupTarget.RequestResource();
                                    return;
                                }
                                mining = false;
                                carryingAmount = resourceTarget.FinishMine(_mineStrength);
                                carringResourceType = resourceTarget.Type;
                                carrying = true;
                                TownHall closestHall = TownHall.FindClosest(transform.position);
                                townHallTarget = closestHall;
                                agent.destination = closestHall.transform.position;
                                _carryingSprite.sprite = resourceTarget.GetChunkSprite();

                                StartCoroutine(CoroutineHelper.DelaySeconds(() => _sprite.sprite = _workerSpriteIdle, 0.1f));
                            }, _mineTime));
                        }
                        else
                        {
                            resourceTarget = resourceTarget.Group.RequestResource();
                        }
                    }
                    else
                    {
                        if (resourceTarget == null)
                        {
                            StopAllCoroutines();
                            resourceTarget = resourceGroupTarget.RequestResource();
                            mining = false;
                        }
                    }
                }
                else if (!mining)
                {
                    if (resourceTarget == null || resourceTarget.MiceMiningAmount > 1)
                    {
                        StopAllCoroutines();
                        resourceTarget = resourceTarget.Group.RequestResource();
                        mining = false;
                    }
                    float variance = 0f;
                    agent.destination = resourceTarget.transform.position + new Vector3(UnityEngine.Random.Range(-variance, variance), 0, UnityEngine.Random.Range(-variance, variance));
                }
            }
            else if (carrying)
            {
                if (!townHallTarget)
                {
                    TownHall closestHall = TownHall.FindClosest(transform.position);
                    townHallTarget = closestHall;
                    agent.destination = closestHall.transform.position;
                }
                if (AtTownHallDestination)
                {
                    CrystalCollection.Add(carryingAmount);
                    carrying = false;
                    carryingAmount = 0;
                    carringResourceType = ResourceType.None;
                    _carryingSprite.sprite = null;
                    resourceTarget = resourceTarget.Group.RequestResource();
                }
            }
        }

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
            if (AtFollowDestination)
            {
                agent.destination = transform.position;
            }
            else if (agent.destination != followTarget.transform.position)
            {
                agent.destination = followTarget.transform.position;
            }
            attacking = false;
        }
        else
        {
            attacking = false;
            attackTimer = 0;
            ChooseAttackTarget();
        }

        if (attacking)
        {
            return;
        }

        if (agent.velocity.x < 0.5f)
        {
            _sprite.flipX = true;
        }
        else if (agent.velocity.x > 0.5f)
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

        if (attackTimer >= _attackCooldown)
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
                projectile.targetingBoss = attackTarget.GetComponent<BossController>() != null;
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
        followTarget = null;
        attackTarget = target;
    }
    internal void SetResourceTarget(Resource resource)
    {
        if (!IsWorker)
        {
            return;
        }
        followTarget = null;
        attackTarget = null;
        resourceTarget = resource;
        resourceGroupTarget = resource.Group;
        townHallTarget = null;
    }
}