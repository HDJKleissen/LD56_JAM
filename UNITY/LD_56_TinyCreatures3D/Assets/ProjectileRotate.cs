using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRotate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _projectileSprite;

    Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = (previousPosition - transform.position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 65, Vector3.forward);
        previousPosition = transform.position;
    }
}
