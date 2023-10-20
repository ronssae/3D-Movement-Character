using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator PlayerAnimator;
    int Horizontal, Vertical;

    private void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        Horizontal = Animator.StringToHash("Horizontal");
        Vertical = Animator.StringToHash("Vertical");
    }
    public void PlayTargetAnimation(string TargetAnimation, bool PlayerIsInteracting)
    {
        PlayerAnimator.SetBool("PlayerIsInteracting", PlayerIsInteracting);
        PlayerAnimator.CrossFade(TargetAnimation, 0.2f);
    }

    public void UpdateAnimatorValues(float HorizontalMovement, float VerticalMovement, bool PlayerIsSprinting)
    {
        //Animation Snapping
        float SnappedHorizontal;
        float SnappedVertical;

        #region Snapped Horizontal
        if (HorizontalMovement > 0 && HorizontalMovement < 0.55f)
        {
            SnappedHorizontal = 0.5f;
        }
        else if (HorizontalMovement > 0.55f)
        {
            SnappedHorizontal = 1f;
        }
        else if (HorizontalMovement < 0 && HorizontalMovement > -0.55f)
        {
            SnappedHorizontal = -0.5f;
        }
        else if (HorizontalMovement < -0.55f)
        {
            SnappedHorizontal = -1f;
        }
        else
        {
            SnappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (VerticalMovement > 0 && VerticalMovement < 0.55f)
        {
            SnappedVertical = 0.5f;
        }
        else if (VerticalMovement > 0.55f)
        {
            SnappedVertical = 1f;
        }
        else if (VerticalMovement < 0 && VerticalMovement > -0.55f)
        {
            SnappedVertical = -0.5f;
        }
        else if (VerticalMovement < -0.55f)
        {
            SnappedVertical = -1f;
        }
        else
        {
            SnappedVertical = 0;
        }
        #endregion

        if (PlayerIsSprinting)
        {
            SnappedHorizontal = HorizontalMovement;
            SnappedVertical = 2f;
        }

        PlayerAnimator.SetFloat(Horizontal, SnappedHorizontal, 0.1f, Time.deltaTime);
        PlayerAnimator.SetFloat(Vertical, SnappedVertical, 0.1f, Time.deltaTime);
    }
}
