using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public SpriteRenderer Sprite;
    public float MoveSpeed;

    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite callSprite;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private GameObject _rightClickIndicatorPrefab;

    private List<NPCMouseController> followingMice = new List<NPCMouseController>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (move.x < 0)
        {
            Sprite.flipX = true;
        }
        else if (move.x > 0)
        {
            Sprite.flipX = false;
        }

        controller.Move(move * Time.deltaTime * MoveSpeed);

        if (Input.GetMouseButton(0))
        {
            Sprite.sprite = callSprite;
        }
        else
        {
            Sprite.sprite = idleSprite;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 clickLocation;
            if (Physics.Raycast(ray, out RaycastHit hitData, float.PositiveInfinity, _groundMask))
            {
                clickLocation = hitData.point;

                GameObject clickIndicator = Instantiate(_rightClickIndicatorPrefab);
                clickIndicator.transform.position = clickLocation + new Vector3(0,0.1f,0);

                Vector2[] locationInCircle = Sunflower(followingMice.Count);
                //Vector3 avgMousePos = Vector3.zero;

                //for (int i = 0; i < followingMice.Count; i++)
                //{
                //    NPCMouseController mouse = followingMice[i];
                //    if (mouse == null)
                //    {
                //        continue;
                //    }
                //    avgMousePos += mouse.transform.position;
                //}
                //avgMousePos /= followingMice.Where(mouse => mouse != null).Count();

                for (int i = 0; i < followingMice.Count; i++)
                {
                    NPCMouseController mouse = followingMice[i];
                    if (mouse == null)
                    {
                        continue;
                    }
                    //mouse.SetDestination(mouse.transform.position);
                    Guid cmdGuid = Guid.NewGuid();
                    mouse.LastCommandGuid = cmdGuid;
                    mouse.SetDestination(clickLocation + new Vector3(locationInCircle[i].x, 0, locationInCircle[i].y) * Mathf.Clamp(followingMice.Count / (Mathf.PI * 1.75f), 1.25f, 2.5f));
                    mouse.SetFollowTarget(null);
                }

                followingMice.Clear();
            }
        }
    }

    public void AddFollower(NPCMouseController mouse)
    {
        followingMice.Add(mouse);
        mouse.SetFollowTarget(transform);
    }

    Vector2[] Sunflower(int n, float alpha = 0, bool geodesic = false)
    {
        float phi = (1 + Mathf.Sqrt(5)) / 2;//golden ratio
        float angle_stride = 360 * phi;
        float radius(float k, float n, float b)
        {
            return k > n - b ? 1 : Mathf.Sqrt(k - 0.5f) / Mathf.Sqrt(n - (b + 1) / 2);
        }

        int b = (int)(alpha * Mathf.Sqrt(n));  //# number of boundary points

        List<Vector2> points = new List<Vector2>();
        for (int k = 0; k < n; k++)
        {
            float r = radius(k, n, b);
            float theta = geodesic ? k * 360 * phi : k * angle_stride;
            float x = !float.IsNaN(r * Mathf.Cos(theta)) ? r * Mathf.Cos(theta) : 0;
            float y = !float.IsNaN(r * Mathf.Sin(theta)) ? r * Mathf.Sin(theta) : 0;
            points.Add(new Vector2(x, y));
        }
        return points.ToArray();
    }
}