using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Utils;

namespace ActionTool
{
    public class GPTrigger : MonoBehaviour
    {
		public enum Type
		{
			ABSOLUTE,
			RELATIVE
		}

        #region Public Members

		public Type _type = Type.ABSOLUTE;

        public List<GPEventID> _events;

        [HideInInspector]
        public ObjectFilter _filter; 

		public List<GameObject> _relativeObjects;

        #endregion

        void OnTriggerEnter(Collider collider)
        {
            if (_filter.IsValid(collider.gameObject))
            {
                foreach(GPEventID evt in _events)
                {
					if(_type == Type.ABSOLUTE)
                    	EventManager.Instance.PostEvent(evt.Name);  
					else
					{
						foreach(GameObject obj in _relativeObjects)
							EventManager.Instance.PostRelativeEvent(obj,evt.Name);
					}
                }

                SendMessage("OnGPTrigger", collider.gameObject);
            }
        }
    }
}
