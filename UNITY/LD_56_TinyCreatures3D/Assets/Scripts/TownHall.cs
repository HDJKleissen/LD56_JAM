using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownHall : MonoBehaviour
{
    public static List<TownHall> townHalls = new List<TownHall>();
    public static TownHall FindClosest(Vector3 from)
    {
        float closestDistance = float.MaxValue;
        TownHall closestHall = null;

        foreach(TownHall hall in townHalls)
        {
            float distance = Vector3.Distance(from, hall.transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestHall = hall;
            }
        }

        return closestHall;
    }

    // Use this for initialization
    void Start()
    {
        townHalls.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}