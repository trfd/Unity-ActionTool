using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
    [GPActionAlias("Shuriken/Play Particles")]
    public class GPActionPlayParticles : GPAction
    {
        #region Public Members

        public List<ParticleSystem> _systems;

        #endregion 

        protected override void OnTrigger()
        {
            foreach (ParticleSystem syst in _systems)
                syst.Play(false);

            End();
        }
    }

}
