using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Orassans
{
    public class CompProperties_ProjectileSwitcher : CompProperties
    {
        public CompProperties_ProjectileSwitcher()
        {
            compClass = typeof(ProjectileSwitcherComp);
        }

        public List<ProjectileSwitcherInfo> projectiles = new List<ProjectileSwitcherInfo>();

        public string switchProjectileLabelKey = "OrassanProjectileSwitcherLabel";
        public string switchProjectileDescriptionKey = "OrassanProjectileSwitcherDescription";

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);

            foreach(ProjectileSwitcherInfo info in projectiles)
            {
                info.ResolveAll();
            }
        }
    }
}
