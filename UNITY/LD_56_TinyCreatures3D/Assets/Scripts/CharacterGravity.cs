using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : MonoBehaviour
{
    public CharacterController controller;
    public float GravityValue = -25f;
    private Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
