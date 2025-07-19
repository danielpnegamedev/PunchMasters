using UnityEngine;
using UnityEngine.Events;

                // BOTEI nome PUNCHER PARA SER MAIS DIRETO  PODERIA FAZER MAIS GENERICO
                // no caso poderia ser um objeto com muitas interações 
                // ex; combatHandler, ai ele teria punch,kick,sword etc... como metodos
public class PuncherHandler : MonoBehaviour
{
    public void StartInteraction(Interactable interactableGO) // PODERIA USAR UMA INTERFACE aki ja que o nome do metodo seria igual para o Collector tambem
    {
        switch (interactableGO.interactionType)
        {
            case TypeInteraction.Punch:
                PunchRagdoll(interactableGO);
                break;
      //      case TypeInteraction.Kick:

        //        break;
          
        }
    }
    public void PunchRagdoll(Interactable interactableGO)
    {
        Vector3 punchDirection = transform.forward;

        interactableGO.gameObject.transform.root.gameObject.GetComponent<Rigidbody>().AddForce(punchDirection * 10f, ForceMode.Impulse);

        /*
        Animator animator = go.GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();


        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (rbs.Length > 0)
        {
            rbs[0].AddForce(punchDirection * 100f, ForceMode.Impulse);
        }
     */
    }

}
