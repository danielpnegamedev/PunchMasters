using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI currentCurrencyTxt;
    public float currentCurrency;

    public TextMeshProUGUI valueUpgradeTxt;
    public List<float> upgradeValues;
    public int upgradeValueIndex;

    public GameObject insuficientCurrencyPopup;
    public GameObject suficientCurrencyPopup;

    public float GetCurrentUpgrateValue() => upgradeValues[upgradeValueIndex];

    public void UpdateUpgrade() =>
        upgradeValueIndex = Mathf.Clamp(++upgradeValueIndex, 0, upgradeValues.Count - 1);

    public void IncreaseCurrentCurrency() => currentCurrency++;

    public void BuyUpgrade()
    {
        float cost = GetCurrentUpgrateValue();

        if (currentCurrency >= cost)
        {
            currentCurrency -= cost;
            UpdateUpgrade();
            UpdateCurrentCurrencyTxt();
            UpdateCurrentValueUpgradeTxt();
            suficientCurrencyPopup.SetActive(true); 
        }
        else
        {
            insuficientCurrencyPopup.SetActive(true);
            Debug.Log("Moeda insuficiente!");
        }
    }

    public void UpdateCurrentCurrencyTxt() =>
        currentCurrencyTxt.text = currentCurrency.ToString("F0"); // ou "F2" p/ 2 casas decimais

    public void UpdateCurrentValueUpgradeTxt() =>
        valueUpgradeTxt.text = GetCurrentUpgrateValue().ToString("F0");
}
