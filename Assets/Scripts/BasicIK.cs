using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class BasicIK : MonoBehaviour
    {
        [Range(0, 1)]
        public float weight;

        [SerializeField] private GameObject handle;
        [SerializeField] private Animator animator;
        private void OnAnimatorIK(int layerIndex)
        {
            // set rotation and position
            animator.SetIKPosition(AvatarIKGoal.RightHand, handle.transform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, handle.transform.rotation);
            
            // Set weight
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
        }
    }
}