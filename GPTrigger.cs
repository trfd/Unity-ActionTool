using UnityEngine;
using System.Collections;

using Utils;

namespace ActionTool
{
    public class GPTrigger : MonoBehaviour
    {

        #region Public Members

        public GPEventID _event;

        [HideInInspector]
        public ObjectFilter _filter; 

        #endregion

        void OnTriggerEnter(Collider collider)
        {
            if (_filter.IsValid(collider.gameObject))
            {
                EventManager.Instance.PostEvent(_event.Name);
                SendMessage("OnGPTrigger", _event.Name);
            }

        }


    }

}
