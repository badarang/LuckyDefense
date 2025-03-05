using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Transform flippable;
    private Animator animator;
    private Material spriteMat;
    private Coroutine hitAnimationCoroutine;
    private float hitEffectValue = 0;
    private float attackAnimationLength;
    public float AttackAnimationLength => attackAnimationLength;

    void Start()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMat = Instantiate(spriteRenderer.material);
        spriteRenderer.material = spriteMat;
        
        flippable = transform.parent.transform;
        
        if (gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            InitUnit();
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            InitEnemy();
        }
    }

    void InitUnit()
    {
        AnimationClip attackClip = GetAttackAnimationClip();
        if (attackClip != null)
        {
            attackAnimationLength = attackClip.length; 
        }
    }

    void InitEnemy()
    {
        
    }
    
    private AnimationClip GetAttackAnimationClip()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        AnimationClip attackClip = null;
        foreach (AnimationClip clip in overrideController.animationClips)
        {
            if (clip.name.Contains("Attack"))
            {
                attackClip = clip;
                break;
            }
        }
        return attackClip;
    }
    
    public void StartHitAnimation(bool isAttackerOnRight)
    {
        if (hitAnimationCoroutine != null)
        {
            StopCoroutine(hitAnimationCoroutine);
        }
        hitAnimationCoroutine = StartCoroutine(HitAnimation(isAttackerOnRight));
    }
    
    private IEnumerator HitAnimation(bool isAttackerOnRight)
    {
        hitEffectValue = .8f;
        
        Transform unitTransform = transform;
        Vector3 originalScale = unitTransform.localScale;
        Vector3 originalPosition = unitTransform.localPosition;
        float moveDistance = 0.005f;
        float rotationAngle = 15f;
        
        int direction = isAttackerOnRight ? -1 : 1;
        
        Sequence sequence = DOTween.Sequence();

        sequence.Append(unitTransform.DOScale(new Vector3(0.6f, 1.4f, 1f), 0.07f));
        sequence.Join(unitTransform.DOLocalMoveX(originalPosition.x + direction * -moveDistance, 0.07f));
        sequence.Join(unitTransform.DOLocalRotate(new Vector3(0, 0, -direction * rotationAngle), 0.07f));

        sequence.Append(unitTransform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.07f));
        sequence.Join(unitTransform.DOLocalMoveX(originalPosition.x, 0.07f));
        sequence.Join(unitTransform.DOLocalRotate(Vector3.zero, 0.07f));

        sequence.Append(unitTransform.DOScale(originalScale, 0.07f));

        yield return sequence.WaitForCompletion();
    }
    
    public void ToggleOutline(bool toggle)
    {
        if (toggle)
        {
            animator.SetInteger("_OutlineAlpha", 1);
        }
        else
        {
            animator.SetInteger("_OutlineAlpha", 0);
        }
    }
    
    public void StartDieAnimation()
    {
        animator.SetTrigger("Die");
    }
    
    void Update()
    {
        if (hitEffectValue > 0)
        {
            spriteMat.SetFloat("_HitEffectBlend", hitEffectValue);
            hitEffectValue -= Time.deltaTime * 10;
        }
    }
}
