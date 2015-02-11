using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
    [GPActionAlias("Shuriken/Stop Particles")]
    public class GPActionStopParticles : GPAction
    {
        #region Public Members

        public List<ParticleSystem> _systems;

        #endregion

        protected override void OnTrigger()
        {
            foreach (ParticleSystem syst in _systems)
                syst.Stop(false);

            End();
        }
    }

}
