using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
    [GPActionAlias("Switchs/Renderers")]
    public class GPActionSwitchRenderer : GPAction
    {
        public List<Renderer> _disableRenderers;
        public List<Renderer> _enableRenderers;
        protected override void OnTrigger()
        {
            foreach (Renderer rd in _disableRenderers)
                rd.enabled = false;

            foreach (Renderer rd in _enableRenderers)
                rd.enabled = true;
        }
    }

}
