using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public AnimationCurve heightCurve;
    public AnimationCurve bossHeightCurve;
    public float Height;
    Tweener tweener;
    public float Damage;
    public NPCCharacter source;
    CharacterTeam CharacterTeam;
    public bool targetingBoss;

    // Use this for initialization
    void Start()
    {
        CharacterTeam = source.GetComponent<TeamSelector>().CharacterTeam;

        Debug.Log(targetingBoss);
        AnimationCurve curve = targetingBoss ? bossHeightCurve : heightCurve;

        tweener = transform.DOMove(Target.position, Speed).SetSpeedBased(true);
        tweener.OnUpdate(() => transform.position = new Vector3(transform.position.x, curve.Evaluate(tweener.ElapsedPercentage()) * Height, transform.position.z));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(tweener.ElapsedPercentage() < 0.2f)
        {
            return;
        }
        NPCCharacter npc = collider.gameObject.GetComponent<NPCCharacter>();
        BossController boss = collider.gameObject.GetComponent<BossController>();

        if (boss)
        {
            boss.Damage(Damage, source, CharacterTeam);
            Destroy(gameObject);
        }
        else if (npc)
        {
            if (npc.GetComponent<TeamSelector>().CharacterTeam != CharacterTeam)
            {
                npc.Damage(Damage, source, CharacterTeam);
                Destroy(gameObject);
            }
        }
        else if (collider.gameObject.GetComponent<Projectile>() != null)
        {
            Destroy(gameObject);
        }
    }
}