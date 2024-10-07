using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI queueText;
    [SerializeField] Image progressImage;
    [SerializeField] float progressMaxHeight;
    [SerializeField] GameObject unit;
    [SerializeField] int cost;
    [SerializeField] float buildTime;

    [SerializeField] int queueAmount;
    [SerializeField] float queueProgress;
    [SerializeField] bool building;
    [SerializeField] UnitCreator creator;
    [SerializeField] KeyCode shopKey;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!building)
        {
            if (queueAmount > 0 && CrystalCollection.CanPay(cost))
            {
                CrystalCollection.Pay(cost);
                building = true;
                queueAmount--;
            }
        }
        else
        {
            queueProgress += Time.deltaTime;
            if (queueProgress >= buildTime)
            {
                SpawnUnit(creator.transform.position + new Vector3(0, 0, -2));
                building = false;
                queueProgress = 0;
            }
            progressImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(0, progressMaxHeight, queueProgress / buildTime));
        }

        if (creator.PlayerIsClose() && Input.GetKeyUp(shopKey) && queueAmount < 9)
        {
            queueAmount++;
        }

        queueText.SetText(queueAmount.ToString());
    }

    public void SpawnUnit(Vector3 spawnPos)
    {
        GameObject u = Instantiate(unit);
        u.transform.position = spawnPos;
        u.GetComponent<NPCCharacter>().SetDestination(u.transform.position);
    }
}
