using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public SpriteRenderer Sprite;
    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(move.x < 0)
        {
            Sprite.flipX = true;
        }
        else if (move.x > 0)
        {
            Sprite.flipX = false;
        }

        controller.Move(move * Time.deltaTime * MoveSpeed);
    }
}
