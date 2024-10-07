using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;

public class BossController : NPCCharacter
{
    [SerializeField] private GameObject strikePrefab;
    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= _attackCooldown)
        {
            attackTimer = 0;
            switch (Random.Range(0, 5))
            {
                case 0:
                    JumpAttack();
                    break;
                case 1:
                    StartCoroutine(RandomStrikes(5, 1f));
                    break;
                case 2:
                    TargetedStrike();
                    break;
                case 3:
                    StartCoroutine(TargetedStrikes(5, 1f));
                    break;
                case 4:
                    StartCoroutine(RandomStrikes(3, 0.4f));
                    StartCoroutine(TargetedStrikes(2, 0.5f));
                    break;
            }
        }
    }

    void JumpAttack()
    {
        _sprite.transform.DOLocalMoveY(3, .8f).SetEase(Ease.OutQuad).OnComplete(
            () => _sprite.transform.DOLocalMoveY(0, .4f).SetEase(Ease.InQuad).OnComplete(
                () =>
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange / 3);

                    foreach (Collider hitCollider in hitColliders)
                    {
                        NPCCharacter character = hitCollider.GetComponentInChildren<NPCCharacter>();
                        if (character && character.GetComponent<BossController>() == null)
                        {
                            character.Damage(4, this, CharacterTeam.ENEMY);
                        }
                    }
                })
            );
    }

    IEnumerator RandomStrikes(int amount, float timeBetween, float timeVariance = 0)
    {
        for (int i = 0; i < amount; i++)
        {
            RandomStrike();
            yield return new WaitForSeconds(timeBetween + Random.Range(-timeVariance, timeVariance));
        }
    }
    IEnumerator TargetedStrikes(int amount, float timeBetween, float timeVariance = 0)
    {
        for (int i = 0; i < amount; i++)
        {
            TargetedStrike();
            yield return new WaitForSeconds(timeBetween + Random.Range(-timeVariance, timeVariance));
        }
    }

    void RandomStrike()
    {
        Vector3 strikeLocation = transform.position + new Vector3(Random.Range(-_attackRange, _attackRange), 0, Random.Range(-_attackRange, _attackRange));
        SpawnStrike(strikeLocation);
    }

    void TargetedStrike()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange);
        List<NPCMouseController> possibleTargets = new List<NPCMouseController>();

        foreach (Collider hitCollider in hitColliders)
        {
            NPCMouseController mouse = hitCollider.GetComponentInChildren<NPCMouseController>();
            if (mouse)
            {
                possibleTargets.Add(mouse);
            }
        }

        if (possibleTargets.Count > 0)
        {
            SpawnStrike(possibleTargets[Random.Range(0, possibleTargets.Count)].transform.position);
        }
    }

    void SpawnStrike(Vector3 location)
    {
        GameObject strike = Instantiate(strikePrefab);
        strike.transform.position = location;
        strike.GetComponent<BossStrike>().boss = this;
    }
}