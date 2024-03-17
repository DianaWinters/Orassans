using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Orassans
{
    public class ProjectileSwitcherComp : ThingComp
    {
        public CompProperties_ProjectileSwitcher SwitcherProps
        {
            get
            {
                return props as CompProperties_ProjectileSwitcher;
            }
        }

        public ProjectileSwitcherInfo ActiveProjectile
        {
            get
            {
                return activeProjectile;
            }
        }

        private ProjectileSwitcherInfo activeProjectile;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            activeProjectile = SwitcherProps.projectiles.First();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            int activeProjectileIndex = -1;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Values.Look(ref activeProjectileIndex, "activeProjectileIndex", 0);
                activeProjectile = SwitcherProps.projectiles[activeProjectileIndex];
            }
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                activeProjectileIndex = SwitcherProps.projectiles.IndexOf(activeProjectile);
                Scribe_Values.Look(ref activeProjectileIndex, "activeProjectileIndex", 0);
            }
        }

        public IEnumerable<Gizmo> GetGizmos()
        {
            if(Find.Selector.NumSelected == 1)
            {
                yield return new Command_Action()
                {
                    defaultLabel = SwitcherProps.switchProjectileLabelKey.Translate(ActiveProjectile.nameKey.Translate()),
                    defaultDesc = SwitcherProps.switchProjectileDescriptionKey.Translate(ActiveProjectile.descriptionKey.Translate()),
                    action = delegate ()
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        foreach (ProjectileSwitcherInfo info in SwitcherProps.projectiles)
                        {
                            FloatMenuOption option = new FloatMenuOption(info.nameKey.Translate(), delegate ()
                            {
                                ProjectileSwitcherInfo projectileInfo = info;
                                activeProjectile = projectileInfo;
                            });
                            options.Add(option);
                        }

                        if (options.Count > 0)
                        {
                            FloatMenu floatMenu = new FloatMenu(options);
                            Find.WindowStack.Add(floatMenu);
                        }
                    },
                    icon = ActiveProjectile.iconTexture,
                    Order = -100
                };
            }
        }
    }
}
