using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitCreator : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float interactRange;
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    [SerializeField] private List<Vector3> interactCheckPoints = new List<Vector3>();

    public bool PlayerIsClose()
    {
        bool close = false;
        foreach(Vector3 pos in interactCheckPoints)
        {
            if (Vector3.Distance(player.transform.position, pos) <= interactRange)
            {
                close = true;
            }
        }
        return close;
    }

    bool shopOpen;

    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        if (shopItems.Count == 0)
        {
            shopItems = new List<ShopItem>(GetComponentsInChildren<ShopItem>());
        }

        interactCheckPoints.Add(transform.position + new Vector3(-3, 0, 0));
        interactCheckPoints.Add(transform.position);
        interactCheckPoints.Add(transform.position + new Vector3(3, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerIsClose() != shopOpen)
        {
            shopOpen = PlayerIsClose();

            foreach (ShopItem si in shopItems)
            {
                si.transform.DOScale(shopOpen ? 1 : 0, 0.2f);
            }
        }
    }
}