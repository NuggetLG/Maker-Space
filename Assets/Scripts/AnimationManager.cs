using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void TriggerAnimation()
    {
        animator.Play("DialogueUiStart");
    }
    
}
