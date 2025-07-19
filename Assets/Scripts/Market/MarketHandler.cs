using UnityEngine;

public class MarketHandler : MonoBehaviour
{
    public GameObject marketPopup;
    public void StartInteraction(Interactable interactableGO)
    {
        switch (interactableGO.interactionType)
        {
            case TypeInteraction.Market:
                BuyAtMarket(interactableGO);
                break;
        }
    }

    public void BuyAtMarket(Interactable interactableGO)
    {
        if (marketPopup.activeInHierarchy) { return; }

        marketPopup.SetActive(true);

    }
}
