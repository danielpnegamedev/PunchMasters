
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Cadastre as Animações e interações")]
    public List<AnimationData> AnimationData;
    public List<InteractionTrigger> interactionTrigger;

    [Header("Componentes")]
    public PuncherHandler puncherHandler;
    public MarketHandler marketHandler;
    public CollectorHandler collectorHandler;
    public AnimationHandler animationHandler;
    public AnimationData GetCurrentAnimationData(TypeInteraction animationName)   => AnimationData.FirstOrDefault(a => a.interactionType == animationName);

    #region Initialize
    private void OnEnable()
    {
        EnableEvents();
    }
    private void OnDisable()
    {
        DisableEvents();
    }
    public void EnableEvents()
    {
        foreach (InteractionTrigger trigger in interactionTrigger)
        {
            trigger.OnInteractionStart.AddListener(StartAnimationInteraction);
        }
        animationHandler.OnInteractionReady.AddListener(StartDynamicInteraction);
    }
    public void DisableEvents()
    {
        foreach (InteractionTrigger trigger in interactionTrigger)
        {
            trigger.OnInteractionStart.RemoveListener(StartAnimationInteraction);
        }
        animationHandler.OnInteractionReady.RemoveListener(StartDynamicInteraction);
    }
    #endregion

    public void StartAnimationInteraction(InteractionData newInteraction)
    {
        Debug.Log("ué4");
        newInteraction.animationData = GetCurrentAnimationData(newInteraction.animationData.interactionType); // pega animação cadastrada

        animationHandler.StartSequence(newInteraction);
    }
    public void StartDynamicInteraction(InteractionData newInteraction)
    {
        Debug.Log("StartDynamicInteraction");
        TypeInteraction interactionType = newInteraction.interactable.interactionType;

        switch (interactionType)
        {
            case TypeInteraction.Punch:
                puncherHandler.StartInteraction(newInteraction.interactable); 
                break;
            case TypeInteraction.CollectCurrency  :
                collectorHandler.StartInteraction(newInteraction.interactable);
                break;
            case TypeInteraction.CollectBody:
                collectorHandler.StartInteraction(newInteraction.interactable);
                break;
            case TypeInteraction.BodyDelivery:
                collectorHandler.StartInteraction(newInteraction.interactable);
                break;
            case TypeInteraction.Market:
                marketHandler.StartInteraction(newInteraction.interactable);
                break;
        }
    }
}

[Serializable]
public class InteractionData
{
    public AnimationData animationData = new AnimationData();
    public Interactable interactable = new Interactable();
}
