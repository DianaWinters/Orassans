using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;

namespace CommunityCoreLibrary
{
    public class PawnBioDef : Def
    {

        #region XML Data

        public BackstoryDef childhoodDef;
        public BackstoryDef adulthoodDef;
        public string firstName;
        public string nickName = "";
        public string lastName;
        public GenderPossibility gender = GenderPossibility.Either;

        #endregion

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            PawnBio bio = new PawnBio()
            {
                gender = this.gender
            };

            if (!this.firstName.NullOrEmpty() && !this.lastName.NullOrEmpty())
            {
                bio.name = new NameTriple(this.firstName, this.nickName, this.lastName);
            }
            else
            {
                Debug.LogError("PawnBio with defName: " + this.defName + " has empty first or last name. It will not be added."+ "Backstories");
                return;
            }
            
            if (this.childhoodDef != null)
            {
                bio.childhood             = this.childhoodDef;
                bio.childhood.shuffleable = false;
                bio.childhood.slot        = BackstorySlot.Childhood;
            }
            else
            {
                Debug.LogError("PawnBio with defName: " + this.defName + " has null childhood. It will not be added." + "Backstories");
                return;
            }
            
            if (this.adulthoodDef != null)
            {
                bio.adulthood             = this.adulthoodDef;
                bio.adulthood.shuffleable = false;
                bio.adulthood.slot        = BackstorySlot.Adulthood;
            }
            else
            {
                Debug.LogError("PawnBio with defName: " + this.defName + " has null adulthood. It will not be added." + "Backstories");
                return;
            }

            bio.name.ResolveMissingPieces();

            if (!SolidBioDatabase.allBios.Contains(bio))
                SolidBioDatabase.allBios.Add(bio);
        }
    }
}