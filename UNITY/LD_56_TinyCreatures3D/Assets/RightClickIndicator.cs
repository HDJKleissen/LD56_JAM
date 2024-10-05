using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RightClickIndicator : MonoBehaviour
{
    [SerializeField] private Disc disc;
    [SerializeField] private float discEndRadius;
    [SerializeField] private float discMidThickness;

    // Start is called before the first frame update
    void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq.Insert(0, DOTween.To(() => disc.Radius, value => disc.Radius = value, discEndRadius, 0.2f));
        seq.Insert(0, DOTween.To(() => disc.Thickness, value => disc.Thickness = value, discMidThickness, 0.1f));
        seq.Insert(0.1f, DOTween.To(() => disc.Thickness, value => disc.Thickness = value, 0, 0.1f));
        seq.OnComplete(() => Destroy(gameObject));

        seq.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
