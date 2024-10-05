using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public AnimationCurve heightCurve;
    public float Height;
    public float initialDistance;
    Tweener tweener;
    public float Damage;
    public NPCCharacter source;
    CharacterTeam CharacterTeam;
    // Use this for initialization
    void Start()
    {
        CharacterTeam = source.GetComponent<TeamSelector>().CharacterTeam;
        initialDistance = Vector3.Distance(transform.position, Target.position);

        tweener = transform.DOMove(Target.position, Speed).SetSpeedBased(true);
        tweener.OnUpdate(() => transform.position = new Vector3(transform.position.x, heightCurve.Evaluate(tweener.ElapsedPercentage()) * Height, transform.position.z));
        tweener.OnComplete(() => Destroy(gameObject));
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(tweener.ElapsedPercentage() < 0.2f)
        {
            return;
        }
        NPCCharacter npc = collider.gameObject.GetComponent<NPCCharacter>();

        if (npc)
        {
            npc.Damage(Damage, source, CharacterTeam);
        }
        Destroy(gameObject);
    }
}