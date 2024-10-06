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
        Resource lowestMiner = null;
        lowestMinerAmount = int.MaxValue;

        foreach(Resource res in resources)
        {
            if (!res) { continue; }

            if(res.MiceMiningAmount <= lowestMinerAmount)
            {
                lowestMiner = res;
                lowestMinerAmount = res.MiceMiningAmount;
            }
        }

        return lowestMiner;
    }
}