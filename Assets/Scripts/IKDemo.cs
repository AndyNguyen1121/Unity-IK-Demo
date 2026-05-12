using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class IKDemo : MonoBehaviour
{
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Animator animator;
    [FormerlySerializedAs("grabOffset")] [SerializeField] private Vector3 rightGrabOffset;
    [SerializeField] private Vector3 leftGrabOffset;
    [FormerlySerializedAs("handRotationOffset")] [SerializeField] private Vector3 rightHandRotationOffset;
    [FormerlySerializedAs("leftGrabRotationOffset")] [SerializeField] private Vector3 leftHandRotationOffset;

    [SerializeField] private Vector3 grabbedObjectOffset;
    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 finalGrabOffset = rightHandTarget.forward * rightGrabOffset.z;
        finalGrabOffset += rightHandTarget.right * rightGrabOffset.x;
        finalGrabOffset += rightHandTarget.up * rightGrabOffset.y;

        Quaternion rightHandFinalRotOffset = Quaternion.Euler(rightHandRotationOffset);
        
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position + finalGrabOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("IK_RightHand_Position_Weight"));
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * rightHandFinalRotOffset);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, animator.GetFloat("IK_RightHand_Rotation_Weight"));
        
        Vector3 finalLeftGrabOffset = rightHandTarget.forward * leftGrabOffset.z;
        finalLeftGrabOffset += rightHandTarget.right * leftGrabOffset.x;
        finalLeftGrabOffset += rightHandTarget.up * leftGrabOffset.y;
        
        Quaternion leftHandFinalRotOffset = Quaternion.Euler(leftHandRotationOffset);
        
        animator.SetIKPosition(AvatarIKGoal.LeftHand, rightHandTarget.position + finalLeftGrabOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetFloat("IK_LeftHand_Position_Weight"));
        animator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandTarget.rotation * leftHandFinalRotOffset);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, animator.GetFloat("IK_LeftHand_Rotation_Weight"));

    }

    public void PickUpItem()
    {
        StartCoroutine(SmoothAttach(rightHandTarget, animator.GetBoneTransform(HumanBodyBones.LeftHand)));
    }
    
    public IEnumerator SmoothAttach(Transform item, Transform hand)
    {
        
        Vector3 startPos = item.position;
        float t = 0f;
        float duration = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            item.position = Vector3.Lerp(startPos, hand.position, t);
            item.up = hand.forward;
            yield return null;
        }
        item.SetParent(hand, false);
        item.up = hand.forward;
        item.localPosition = grabbedObjectOffset;
    }
}
