using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
    [GPActionAlias("Switchs/Colliders")]
    public class GPActionSwitchColliders : GPAction
    {
        public List<Collider> _disableColliders;
        public List<Collider> _enableColliders;
        protected override void OnTrigger()
        {
            foreach (Collider rd in _disableColliders)
                rd.enabled = false;

            foreach (Collider rd in _enableColliders)
                rd.enabled = true;
        }
    }

}
