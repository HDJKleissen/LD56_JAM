using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CrystalCollection : MonoBehaviour
{
    public static int CrystalAmount;
    [SerializeField]
    private TextMeshProUGUI text;
    private static int crystalTextAmount;
    
    // Start is called before the first frame update
    void Start()
    {
        CrystalAmount = 0;
    }

    public static bool CanPay(int amount)
    {
        return amount <= CrystalAmount;
    }

    public static void Pay(int amount)
    {
        if (CanPay(amount))
        {
            CrystalAmount -= amount;
            UpdateText();
        }
    }

    public static void Add(int amount)
    {
        CrystalAmount += amount;
        UpdateText();
    }

    private static void UpdateText()
    {
        DOTween.To(() => crystalTextAmount, value => crystalTextAmount = value, CrystalAmount, 0.1f);
    }

    private void Update()
    {
        text.SetText(crystalTextAmount.ToString());
    }
}
