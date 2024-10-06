using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ResourceGroup : MonoBehaviour
{
    List<Resource> resources = new List<Resource>();
    public int lowestMinerAmount;

    // Use this for initialization
    void Start()
    {
        lowestMinerAmount = 0;
        resources = new List<Resource>(GetComponentsInChildren<Resource>());
        foreach(Resource res in resources)
        {
            res.Group = this;
        }
    }

    public Resource RequestResource()
    {
        if (resources.Count > 0)
        {
            return resources[UnityEngine.Random.Range(0, resources.Count)];
        }

        return null;
    }

    internal void RemoveResource(Resource resource)
    {
        if (resources.Contains(resource))
        {
            resources.Remove(resource);
        }
    }
}