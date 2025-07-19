using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHandler : MonoBehaviour
{
    public Animator animator;
    private bool isAnimationPlaying = false;
    private float originalAnimationLength;

    [Header("Animation Settings")]
    public AnimationData animationData;

    [HideInInspector] public UnityEvent<InteractionData> OnInteractionReady;

    public void StartSequence(InteractionData _interactionData) => StartCoroutine(RunSequenceCoroutine(_interactionData));
    private IEnumerator RunSequenceCoroutine(InteractionData _interactionData)
    {
        if (isAnimationPlaying) yield break;
        isAnimationPlaying = true;

        animationData = _interactionData.animationData;
        animationData.CalculateDurations();
        animator.Play(animationData.animationName.ToString(), 1, 0f);

        yield return null; // espera 1 frame

        var stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        originalAnimationLength = stateInfo.length;
        float speed = CalculateAnimationSpeed();
        if (animationData.isLooping) speed = 1;
        animator.speed = speed;

        yield return new WaitForSeconds(animationData.calculatedInteractionTime);

        // ocorre a interação desejada no tempo desejado da animação 
        // talvez o script precise de um nome mais generico
        // ja q ele lida com a animação e com o trigger do evento da interação

        if (animationData.shouldInteract)  {  OnInteractionReady.Invoke(_interactionData);  }

        //
        //
        //

        yield return new WaitForSeconds(animationData.calculatedRemainingTime);

        ResetAnimationState();
    }
    public void ResetAnimationState()
    {
        isAnimationPlaying = false;
        animator.Play("Empty", 1, 0f);
        animator.speed = 1f;
    }
    public float CalculateAnimationSpeed() => (animationData.calculatedDuration > 0.01f) ? originalAnimationLength / animationData.calculatedDuration : 1f;

}

[Serializable]
public class AnimationData /// aki no caso ele poderia ser um SCRIPTABLE OBJECT para cadastrar e deixar salvo no projeto 
{
    public TypeAnimation animationName;
    public TypeInteraction interactionType; // nome deve ser os mesmo das animações cadastradas no animator
    public bool isLooping;
    public bool isMovementAllowed;

    [Header("Animation Duration")]
    public float baseDuration;

    [Header("Animation Modifiers")]
    public float speed;
    public float speedPercentMultiplier;

    [Header("Debug")]
    public float calculatedDuration;
    public float calculatedInteractionTime;
    public float calculatedRemainingTime;

    [Header("Interaction Moment")]
    public bool  shouldInteract;
    public float interactAtPercent;

    public void CalculateDurations()
    {
        CalculateAnimationFinalDuration();
        CalculateProjectileSpawnTime();
        CalculateRamainingTime();
    }
    public float CalculateAnimationFinalDuration() => calculatedDuration = Mathf.Max(0.1f, baseDuration - (speed * speedPercentMultiplier));
    public float CalculateProjectileSpawnTime() => calculatedInteractionTime = (interactAtPercent / 100f) * calculatedDuration;
    public float CalculateRamainingTime() => calculatedRemainingTime = calculatedDuration - calculatedInteractionTime;

}