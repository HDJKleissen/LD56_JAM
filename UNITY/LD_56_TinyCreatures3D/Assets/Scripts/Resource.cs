using UnityEngine;
using System.Collections;
using Shapes;
using DG.Tweening;

public class Resource : MonoBehaviour
{
    public ResourceGroup Group;
    public ResourceType Type;
    public int StartingAmount;
    public int Amount;

    public int MiceMiningAmount;
    [SerializeField] private Sprite[] ChunkSprites = new Sprite[0];

    [SerializeField] private Sprite[] CrystalSprites = new Sprite[3];
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private Disc SelectionDisc;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Amount > StartingAmount * 0.66f)
        {
            if (sprite.sprite != CrystalSprites[0])
            {
                sprite.sprite = CrystalSprites[0];
            }
        }
        else if (Amount > StartingAmount * 0.33f)
        {
            if (sprite.sprite != CrystalSprites[1])
            {
                sprite.sprite = CrystalSprites[1];
            }
        }
        else if (Amount <= StartingAmount * 0.33f)
        {
            if (sprite.sprite != CrystalSprites[2])
            {
                sprite.sprite = CrystalSprites[2];
            }
        }

        if (Amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Group.RemoveResource(this);
    }

    public void BlinkSelectionCircle()
    {
        Sequence seq = DOTween.Sequence();
        seq.Insert(0, DOTween.To(() => SelectionDisc.Color.a, value => SelectionDisc.Color = new Color(SelectionDisc.Color.r, SelectionDisc.Color.g, SelectionDisc.Color.b, value), 1, 0.2f));
        seq.Insert(0.2f, DOTween.To(() => SelectionDisc.Color.a, value => SelectionDisc.Color = new Color(SelectionDisc.Color.r, SelectionDisc.Color.g, SelectionDisc.Color.b, value), 0, 0.2f));
        
        seq.Play();
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