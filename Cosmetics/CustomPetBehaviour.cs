using FungleAPI.Assets;
using FungleAPI.MonoBehaviours;
using PowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class CustomPetBehaviour : MonoBehaviour
    {
        public CustomPet Data;
        public PetBehaviour Pet => GetComponent<PetBehaviour>();
        public SpriteAnimator Animator;
        public AnimationClip Anim => Pet.animator.Clip;
        public SpriteSheet Current => Animator.animation;
        public void Update()
        {
            if (Anim == Pet.walkClip && Current != Data.Walk)
            {
                Animator.SetAnimation(Data.Walk);
            }
            if (Anim == Pet.idleClip && Current != Data.Idle)
            {
                Animator.SetAnimation(Data.Idle);
            }
            if (Anim == Pet.petClip && Current != Data.Petted)
            {
                Animator.SetAnimation(Data.Petted);
            }
            if (Anim == Pet.scaredClip && Current != Data.Scared)
            {
                Animator.SetAnimation(Data.Scared);
            }
            if (Anim == Pet.sadClip && Current != Data.Sad)
            {
                Animator.SetAnimation(Data.Sad);
            }
        }
    }
}
