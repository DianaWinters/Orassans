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
            BackstoryDatabase.TryGetWithIdentifier(this.childhoodDef.defName, out Backstory childhood);
            if (childhood != null)
            {
                bio.childhood = childhood;
                bio.childhood.shuffleable = false;
                bio.childhood.slot = BackstorySlot.Childhood;
            }
            else
            {
                Debug.LogError("PawnBio with defName: " + this.defName + " has null childhood. It will not be added." + "Backstories");
                return;
            }
            BackstoryDatabase.TryGetWithIdentifier(this.adulthoodDef.defName, out Backstory adulthood);
            if (adulthood != null)
            {
                bio.adulthood = adulthood;
                bio.adulthood.shuffleable = false;
                bio.adulthood.slot = BackstorySlot.Adulthood;
            }
            else
            {
                Debug.LogError("PawnBio with defName: " + this.defName + " has null adulthood. It will not be added." + "Backstories");
                return;
            }

            bio.name.ResolveMissingPieces();

            bool flag = false;
            foreach (string error in bio.ConfigErrors())
            {
                if (!flag)
                {
                    flag = true;
                    Debug.LogError("Config error(s) in PawnBioDef " + this.defName + ". Skipping..." + "Backstories");
                }
                Debug.LogError(error + "Backstories");
            }
            if (flag)
            {
                return;
            }

            if (!SolidBioDatabase.allBios.Contains(bio))
                SolidBioDatabase.allBios.Add(bio);
        }
    }
}