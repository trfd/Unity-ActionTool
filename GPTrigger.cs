using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Utils;

namespace ActionTool
{
    public class GPTrigger : MonoBehaviour
    {

        #region Public Members

        public List<GPEventID> _events;

        [HideInInspector]
        public ObjectFilter _filter; 

        #endregion

        void OnTriggerEnter(Collider collider)
        {
            if (_filter.IsValid(collider.gameObject))
            {
                foreach(GPEventID evt in _events)
                {
                    EventManager.Instance.PostEvent(evt.Name);  
                }

                SendMessage("OnGPTrigger", collider.gameObject);
            }

           
        }
    }
}
