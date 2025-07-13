using FungleAPI.Assets;
using FungleAPI.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.InitializeLocalInstruction;

namespace FungleAPI.Cosmetics
{
    public class CustomPet
    {
        internal static List<CustomPet> Pets = new List<CustomPet>();
        public CustomPet(string petId, StringNames name, SpriteSheet walk, SpriteSheet idle, SpriteSheet petted, SpriteSheet scared, SpriteSheet sad)
        {
            petId = "pet_" + petId;
            foreach (CustomPet pet in Pets)
            {
                if (pet.Data.ProductId == petId)
                {
                    return;
                }
            }
            Data = new PetData();
            foreach (PetData d in HatManager.Instance.allPets)
            {
                if (!d.IsEmpty)
                {
                    Data.PetPrefabRef = d.PetPrefabRef;
                    Data.PetPrefabRef.SubObjectName = petId;
                }
            }
            Data.ProductId = petId;
            Data.Free = true;
            Data.StoreName = name;
            Data.NotInStore = true;
            Walk = walk;
            Idle = idle;
            Petted = petted;
            Scared = scared;
            Sad = sad;
            Pets.Add(this);
        }
        public PetData Data;
        public Vector3 ShadowPosition;
        public Vector3 ShadowScale;
        public SpriteSheet Walk;
        public SpriteSheet Idle;
        public SpriteSheet Petted;
        public SpriteSheet Scared;
        public SpriteSheet Sad;
    }
}
