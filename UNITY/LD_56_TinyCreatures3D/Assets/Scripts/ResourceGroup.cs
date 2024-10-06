using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        //List<Resource> lowestMiners = new List<Resource>();
        //lowestMinerAmount = int.MaxValue;

        //for (int i = 0; i < resources.Count; i++)
        //{
        //    Resource res = resources[i];
        //    if (!res) { continue; }

        //    if(res.MiceMiningAmount == lowestMinerAmount)
        //    {
        //        lowestMiners.Add(res);
        //    }
        //    if(res.MiceMiningAmount < lowestMinerAmount)
        //    {
        //        i = 0;
        //        lowestMiners.Clear();
        //        lowestMinerAmount = res.MiceMiningAmount;
        //    }
        //}

        return resources[Random.Range(0, resources.Count)];
    }
}