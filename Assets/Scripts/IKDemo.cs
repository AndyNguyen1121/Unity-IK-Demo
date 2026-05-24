using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class IKDemo : MonoBehaviour
{
    [Header("Assign References")]
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    
    [Header("Right Hand")]
    [SerializeField] private Vector3 rightGrabOffset;
    [SerializeField] private Vector3 rightHandRotationOffset;
    
    [Header("Left Hand")]
    [SerializeField] private Vector3 leftGrabOffset;
    [SerializeField] private Vector3 leftHandRotationOffset;
    
    // When the object snaps to the hand, the animation also snaps back into its original position
    // this is used to smoothly transition the animation with the IK
    private Vector3 placeholder;
    private Quaternion placeholderRotation;

    [SerializeField] private Vector3 grabbedObjectOffset;

    private void Start()
    {
        placeholder = target.position;
        placeholderRotation = target.rotation;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 placeholderForward = placeholderRotation * Vector3.forward;
        Vector3 placeholderRight = placeholderRotation * Vector3.right;
        Vector3 placeholderUp = placeholderRotation * Vector3.up;

        Vector3 finalGrabOffset = placeholderForward * rightGrabOffset.z + placeholderRight   * rightGrabOffset.x + placeholderUp * rightGrabOffset.y;

        // Set position and rotation based on the weight defined in animation curves
        animator.SetIKPosition(AvatarIKGoal.RightHand, placeholder + finalGrabOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("IK_RightHand_Position_Weight"));
        animator.SetIKRotation(AvatarIKGoal.RightHand, placeholderRotation * Quaternion.Euler(rightHandRotationOffset));
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, animator.GetFloat("IK_RightHand_Rotation_Weight"));

        Vector3 finalLeftGrabOffset = placeholderForward * leftGrabOffset.z + placeholderRight * leftGrabOffset.x  + placeholderUp * leftGrabOffset.y;

        animator.SetIKPosition(AvatarIKGoal.LeftHand, placeholder + finalLeftGrabOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetFloat("IK_LeftHand_Position_Weight"));
        animator.SetIKRotation(AvatarIKGoal.LeftHand, placeholderRotation * Quaternion.Euler(leftHandRotationOffset));
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, animator.GetFloat("IK_LeftHand_Rotation_Weight"));

    }

    // called in animation event
    public void PickUpItem(int handIndex)
    {
        if (handIndex == 0)
        {
            StartCoroutine(SmoothAttach(target, animator.GetBoneTransform(HumanBodyBones.LeftHand)));
        }
        else
        {
            StartCoroutine(SmoothAttach(target, animator.GetBoneTransform(HumanBodyBones.RightHand)));
        }
    }
    
    // gravitates item towards hand
    public IEnumerator SmoothAttach(Transform item, Transform hand)
    {
        Vector3 startPos = item.position;
        float t = 0f;
        float duration = 0.25f;
        

        while (t < 1f)
        {
            t = Mathf.Clamp01(t + Time.deltaTime / duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            Vector3 worldOffset = hand.TransformDirection(grabbedObjectOffset); 
            item.position = Vector3.Lerp(startPos, hand.position + worldOffset, smooth);

            yield return null;
        }
        item.SetParent(hand,false);
        item.rotation = Quaternion.LookRotation(-hand.right, -hand.forward);
        item.localPosition = grabbedObjectOffset;
    }
}
