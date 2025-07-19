using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectorHandler : MonoBehaviour
{
    public List<Transform> stackedBodies = new List<Transform>();

    public UIManager uiManager;

    public float currencyCurrent;
    public float currencyIncrease;

    private bool isUpdatingBodies = false;
    private List<Vector3> velocities = new List<Vector3>();

    [Header("Prefabs")]
    public GameObject currencyPrefab;

    [Header("Stack Settings")]
    public Vector3 stackOffset = new Vector3(0, 3f, -0.5f);
    public float bodySpacing = -5f;
    public float baseSmoothTime = 0.05f;
    public float maxSmoothTime = 100f;
    public float swayAmplitude = 1000f;
    public float swayFrequency = 1f;

    [Header("Delivery Settings")]
    public float bodyDeliveryArcHeight = 10f;        // Altura do arco dos corpos
    public Vector3 currencyDeliveryOffset = new Vector3(7f, 0f, 0f); // Offset à direita da lixeira
    public float currencyArcHeight = 3f;          // Altura do arco do dinheiro

    public void StartInteraction(Interactable interactableGO)
    {
        switch (interactableGO.interactionType)
        {
            case TypeInteraction.CollectCurrency:
                CollectCurrency(interactableGO);
                break;
            case TypeInteraction.CollectBody:
                CollectBody(interactableGO);
                break;
            case TypeInteraction.BodyDelivery:
                BodyDelivery(interactableGO);
                break;
        }
    }

    public void CollectCurrency(Interactable interactableGO)
    {
        uiManager.IncreaseCurrentCurrency();
        uiManager.UpdateCurrentCurrencyTxt();
        Transform target = transform.root;
        Transform money = interactableGO.transform;

      //  DisablePhysics(interactableGO);
        StartCoroutine(DeliverCurrencySmooth(money, target,0.1f));
    }

    

    public void CollectBody(Interactable interactableGO)
    {
        DisablePhysics(interactableGO);

        Transform body = interactableGO.transform.root;
        body.SetParent(transform);

        // Deitar o corpo de barriga para cima
        body.rotation = Quaternion.LookRotation(transform.up, -transform.forward);

        stackedBodies.Add(body);
        velocities.Add(Vector3.zero);

        if (!isUpdatingBodies)
            StartCoroutine(UpdateStackBodies());
    }

    private IEnumerator UpdateStackBodies()
    {
        isUpdatingBodies = true;

        while (stackedBodies.Count > 0)
        {
            float time = Time.time;

            for (int i = 0; i < stackedBodies.Count; i++)
            {
                if (i >= velocities.Count)
                    velocities.Add(Vector3.zero);

                Transform current = stackedBodies[i];

                // ----- POSIÇÃO BASE -----
                Vector3 baseTarget;
                if (i == 0)
                {
                    baseTarget = transform.position + transform.TransformDirection(stackOffset);
                }
                else
                {
                    baseTarget = stackedBodies[i - 1].position + Vector3.up * bodySpacing;
                }

                // ----- SUAVIDADE -----
                float lerpFactor = (float)i / Mathf.Max(1, stackedBodies.Count - 1);
                float smoothTime = Mathf.Lerp(baseSmoothTime, maxSmoothTime, lerpFactor);

                // ----- BALANÇO -----
                float sway = Mathf.Sin(time * swayFrequency + i * 0.5f) *
                             swayAmplitude * lerpFactor;
                Vector3 swayOffset = transform.right * sway;

                Vector3 targetPos = baseTarget + swayOffset;

                // ----- MOVIMENTO -----
                Vector3 vel = velocities[i];
                current.position = Vector3.SmoothDamp(current.position, targetPos, ref vel, smoothTime);
                velocities[i] = vel;

                // ----- ROTAÇÃO -----
                Quaternion targetRot = Quaternion.LookRotation(transform.up, -transform.forward);
                current.rotation = Quaternion.Slerp(current.rotation, targetRot, Time.deltaTime * (5f + 5f * lerpFactor));
            }

            yield return null;
        }

        isUpdatingBodies = false;
    }

    public void DisablePhysics(Interactable interactableGO)
    {
        GameObject root = interactableGO.gameObject.transform.root.gameObject;

        foreach (Collider col in root.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
    }

    public void BodyDelivery(Interactable interactableGO)
    {
        if (stackedBodies.Count == 0) return;

        Transform target = interactableGO.transform; // Lata de lixo

        for (int i = stackedBodies.Count - 1; i >= 0; i--)
        {
            Transform body = stackedBodies[i];
            stackedBodies.RemoveAt(i);
            velocities.RemoveAt(i);

            // Corpo indo para a lixeira
            StartCoroutine(DeliverBodySmooth(body, target, i * 0.1f));

            // Spawna um dinheiro para cada corpo
            if (currencyPrefab != null)
            {
                GameObject currency = Instantiate(currencyPrefab, body.position, Quaternion.identity);
                StartCoroutine(DeliverCurrencySmooth(currency.transform, target, i * 0.1f,false));
            }
        }
    }

    private IEnumerator DeliverBodySmooth(Transform body, Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);

        float duration = 0.6f;
        float elapsed = 0f;

        Vector3 startPos = body.position;
        Vector3 midPos = startPos + Vector3.up * bodyDeliveryArcHeight;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);

            Vector3 curvedPos = Vector3.Lerp(startPos, midPos, t);
            body.position = Vector3.Lerp(curvedPos, target.position, t);

            body.Rotate(Vector3.up * 360f * Time.deltaTime, Space.World);

            yield return null;
        }

        body.position = target.position;
        Destroy(body.gameObject);
    }

    private IEnumerator DeliverCurrencySmooth(Transform currency, Transform target, float delay, bool canDestroy = true)
    {
        yield return new WaitForSeconds(delay + 0.1f);

        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPos = currency.position;
        Vector3 finalTarget = target.position + target.TransformDirection(currencyDeliveryOffset);
        
        Vector3 midPos = startPos + Vector3.up * currencyArcHeight + (Random.insideUnitSphere * 0.1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);

            Vector3 curvedPos = Vector3.Lerp(startPos, midPos, t);
            currency.position = Vector3.Lerp(curvedPos, finalTarget, t);

            currency.Rotate(Vector3.up * 720f * Time.deltaTime, Space.World);
            yield return null;
        }

        currency.position = finalTarget;

        if (canDestroy) { Destroy(currency.gameObject, 0.1f); }
            
    }
}
