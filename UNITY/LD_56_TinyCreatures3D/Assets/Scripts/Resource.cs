using UnityEngine;
using System.Collections;


public class Resource : MonoBehaviour
{
    public ResourceGroup Group;
    public ResourceType Type;
    public int StartingAmount;
    public int Amount;

    public int MiceMiningAmount;
    [SerializeField] private Sprite[] ChunkSprites = new Sprite[0];

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void StartMine()
    {
        MiceMiningAmount++;
    }

    public void AbortMine()
    {
        MiceMiningAmount--;
    }

    public Sprite GetChunkSprite()
    {
        return ChunkSprites[Random.Range(0, ChunkSprites.Length)];
    }

    public int FinishMine(int amount)
    {
        MiceMiningAmount--;
        if(amount > Amount)
        {
            Amount = 0;
            return Amount;
        }

        Amount -= amount;
        return amount;
    }
}

public enum ResourceType
{
    None,
    Crystal
}