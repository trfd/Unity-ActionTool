using UnityEngine;
using System.Collections;

namespace ActionTool
{   
    public class EventTrigger : MonoBehaviour
    {
        #region Public Member

        /// <summary>
        /// Name of event to trigger
        /// </summary>
        public string _eventName;

        #endregion

        #region Trigger Interface

        [InspectorButton("Trigger")]
        public void Trigger()
        {
            if(string.IsNullOrEmpty(_eventName))
                throw new System.NullReferenceException("Null event name");

            EventManager.Instance.PostEvent(_eventName);
        }

        #endregion
    }
}