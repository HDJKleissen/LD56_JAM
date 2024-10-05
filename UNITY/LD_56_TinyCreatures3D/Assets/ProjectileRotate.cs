using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRotate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _projectileSprite;
    [SerializeField] private Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = body.velocity;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        _projectileSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
}
