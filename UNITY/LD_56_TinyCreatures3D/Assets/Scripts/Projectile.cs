using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public Transform Target;
    public float Speed;
    public AnimationCurve heightCurve;
    public float initialDistance;
    Tweener tweener;

    // Use this for initialization
    void Start()
    {
        initialDistance = Vector3.Distance(transform.position, Target.position);

        tweener = transform.DOMove(Target.position, Speed).SetSpeedBased(true);
        tweener.OnComplete(() => Destroy(gameObject));
        
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, heightCurve.Evaluate(tweener.ElapsedPercentage()), transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}