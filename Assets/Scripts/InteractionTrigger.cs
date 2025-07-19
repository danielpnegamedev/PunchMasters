using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour
{
    [HideInInspector] public UnityEvent<InteractionData> OnInteractionStart;

    public TypeInteraction interactionType;
    
    private void OnTriggerEnter(Collider other)
    {
       
    }
    public void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Interactable>(out Interactable interactable)) { return; }
        Debug.Log("ué1");

        if (gameObject.transform.root.gameObject == interactable.gameObject.transform.root.gameObject) { return; }
        Debug.Log("ué2");
        if (interactable.interactionType != interactionType) { return; } // garante que as interações estejam alinhadas

        Debug.Log("ué3");
        InteractionData newInteraction = new InteractionData();
        newInteraction.interactable = interactable;
        newInteraction.animationData.interactionType = interactionType;

        OnInteractionStart.Invoke(newInteraction);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<InteractionController>(out InteractionController interactionController)) { return; }

        // do things

    }
}
