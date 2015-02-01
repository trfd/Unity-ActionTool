using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
	[ExecuteInEditMode]
    public class EventHandler : MonoBehaviour , IActionOwner , INodeOwner , ISerializationCallbackReceiver
    {
        #region Enum Definition
	
        /// <summary>
        /// State of the handler
        /// </summary>
        public enum HandlerState
        {
            NONE,
            RUNNING,
            SLEEPING,
            TERMINATED
        }

        #endregion

        #region Static Members

        public const int s_kindFixedCountMask = 0x01;
        public const int s_kindLockMask       = 0x02;

        #endregion 

        #region Private Members

		/// <summary>
		/// Action to trigger
		/// </summary>
		[HideInInspector]
		[UnityEngine.SerializeField]
		private GPAction m_action;

		/// <summary>
		/// Whether or not the handler can fire a fixed or infinite number of time
		/// its action.
		/// </summary>
		[UnityEngine.SerializeField]
		private bool m_usesFixedCount;

		/// <summary>
		/// Whether or not the handler can fire its action while it's already running.
		/// </summary>
		[UnityEngine.SerializeField]
		private bool m_usesLockUntilCompletion;

        /// <summary>
        /// Number of times the event has been triggered
        /// </summary>
        private int m_triggerCount;

        /// <summary>
        /// Current state of the handler
        /// </summary>
        private HandlerState m_currState = HandlerState.NONE;

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private UnityEngine.Object m_prefabAction;

		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		private GPActionRelatedObject m_relativeObjectAction;

#if UNITY_EDITOR
		
		/// <summary>
		/// Holds whether or not the initialization is due
		/// </summary>
		private bool m_dueNodeInit;

		[SerializeField]
		[HideInInspector]
		private ActionEditorNode m_eventNode;

#endif

        #endregion

        #region Public Members

        /// <summary>
        /// Name of the event the handler is listening
        /// </summary>
        public GPEventID _eventID;

        /// <summary>
        /// Maximum number of time the event can be triggered
        /// </summary>
        public int _maxTriggerCount;

#if UNITY_EDITOR

		[UnityEngine.HideInInspector]
		public Rect _windowRect = new Rect(0,0,100,50);

#endif

        #endregion

        #region Properties

		public GPAction Action
		{
			get{ return m_action;  }
			set
			{ 
#if UNITY_EDITOR
				Disconnect(m_action);
#endif
				m_action = value; 
#if UNITY_EDITOR
				Connect(m_action);
#endif
			}
		}

		public UnityEngine.Object PrefabAction
		{
			get{ return m_prefabAction;  }
			set{ m_prefabAction = value; }
		}

        /// <summary>
        /// Readonly access to the handler state.
        /// </summary>
        public HandlerState State
        {
            get { return m_currState; }
        }

		public GPEvent CurrentEvent
		{
			get; set;
		}

#if UNITY_EDITOR
		
		public Rect WindowRect
		{
			get{ return _windowRect; }
			set{ _windowRect = value; }
		}
	
		public ActionEditorNode EventNode
		{
			get
			{
				if(m_dueNodeInit)
					CreateEventNode();
				return m_eventNode;
			}
		}
		
#endif

        #endregion

		#region Constructors

		public EventHandler()
		{
		}

		#endregion

        #region ContextMenu

        [ContextMenu("Show GPActionObject")]
        private void ShowAllActions()
        {
            GameObject obj = GetGPActionObjectOrCreate();

            obj.hideFlags = HideFlags.None;
        }


        [ContextMenu("Hide GPActionObject")]
        private void HideAllActions()
        {
            GameObject obj = GetGPActionObjectOrCreate();

            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        #endregion 

        #region MonoBehaviour

        void Start()
        {
			if(m_relativeObjectAction == null)
				m_relativeObjectAction = (GPActionRelatedObject) GetGPActionObjectMapperOrCreate()
					.AddAction(this,typeof(GPActionRelatedObject));

			if(!Application.isPlaying)
				return;

            Init();

			if(_eventID.Name == "Start")
				this.EventTrigger(_eventID);
        }

        void Update()
        {
			if(!Application.isPlaying)
				return;

            if(Action == null)
                return;

            if(Action.HasEnded)
            {
				CurrentEvent = null;

            	if(HasReachedMaxTriggerCount())
             		m_currState = HandlerState.TERMINATED;
            	else
            		m_currState = HandlerState.SLEEPING;
			
				return;
            }

			if(Action.IsRunning)
				Action.Update();
        }

		void OnEnable()
		{
			if(Application.isPlaying && _eventID.Name == "OnEnable")
				this.EventTrigger(_eventID);
		}

		void OnDisable()
		{
			if(Application.isPlaying && _eventID.Name == "OnDisable")
				this.EventTrigger(_eventID);
		}

		void OnCollisionEnter(Collision collision)
		{
			if(_eventID.Name == "OnCollisionEnter")
				this.EventTrigger(_eventID,collision.gameObject);
		}

		void OnCollisionExit(Collision collision)
		{
			if(_eventID.Name == "OnCollisionExit")
				this.EventTrigger(_eventID,collision.gameObject);
		}

		void OnTriggerEnter(Collider other)
		{
			if(_eventID.Name == "OnCollisionEnter")
				this.EventTrigger(_eventID,other.gameObject);
		}
		
		void OnTriggerExit(Collider other)
		{
			if(_eventID.Name == "OnCollisionExit")
				this.EventTrigger(_eventID,other.gameObject);
		}

		void OnDrawGizmos()
		{
			if(Action != null)
				Action.OnDrawGizmos();
		}

		void OnDrawGizmosSelected()
		{
			if(Action != null)
				Action.OnDrawGizmosSelected();
		}

		void OnDestroy()
		{
			GPActionObjectMapper obj = GetGPActionObjectMapper();

			if(obj == null)
				return;

			obj.RemoveEventHandler(this);
		}

        #endregion

        #region Event Listening

        public void Init()
        {
            if(_eventID == GPEventID.Invalid)
                throw new System.Exception("Null event name");

            EventManager.Instance.Register(_eventID.ID, EventTrigger);

			if(Action != null)
				Action.SetParentHandler(this);
        }

        public void EventTrigger(GPEvent evt)
        {
            if (!evt.EventID.Equals(_eventID) || Action == null)
                return;

			CurrentEvent = evt;

			m_relativeObjectAction._relatedObject = evt.RelatedObject;

            if(CanTriggerAction())
               TriggerAction();
        }

		private void EventTrigger(GPEventID id, GameObject relatedObj = null)
		{
			GPEvent evt = new GPEvent();
			
			evt.EventID = id;
			evt.RelatedObject = relatedObj;
			
			this.EventTrigger(evt);
		}

        #endregion

#if UNITY_EDITOR

		public void CreateEventNode()
		{
			m_dueNodeInit = false;

			m_eventNode = new ActionEditorNode();

			m_eventNode._owner = this;
			m_eventNode._center = new Vector2(92,25);

			if(m_action != null)
				Connect(m_action);
			else
				m_eventNode._connection = null;
		}

		#region IActionOwner

		public void Connect(GPAction action)
		{
			if(action == null)
				return;

			if(action._leftNode._connection != null)
			{
				((IActionOwner)action._leftNode._connection._nodeParent._owner).Disconnect(action);
			}

			EventNode._connection = new ActionEditorConnection(EventNode,action._leftNode);
			action._leftNode._connection = EventNode._connection;
		}

		public void Disconnect(GPAction Action)
		{
			if(m_action == null)
				return;

			m_action._leftNode._connection = null;
			EventNode._connection = null;
		}

		public void DisconnectAll()
		{
			Disconnect(m_action);
		}

		#endregion

#endif

		#region Serialization Callbacks

		public void OnBeforeSerialize(){}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			m_dueNodeInit = true;
#endif
		}

		#endregion

        #region Private Utils

        private bool CanTriggerAction()
        {
            if(HasReachedMaxTriggerCount())
                return false;
           
			if (m_usesLockUntilCompletion && Action.IsRunning)
                return false;

            return true;
        }

        private bool HasReachedMaxTriggerCount()
        {
        	// Check if handler kind has fixed count and 
			// if current count allows to run one more time.

            return (m_usesFixedCount && m_triggerCount >= _maxTriggerCount);
        }

        private void TriggerAction()
        {
            m_currState = HandlerState.RUNNING;
       	
            m_triggerCount++;
            Action.Trigger();
        }

        #endregion

        #region GPActionObjectMapper Wrapping

        public virtual GameObject GetGPActionObject()
        {
            return GPActionUtils.GetGPActionObject(this.gameObject);
        }

        public virtual GPActionObjectMapper GetGPActionObjectMapper()
        {
            return GPActionUtils.GetGPActionObjectMapper(this.gameObject);
        }

        public virtual GameObject GetGPActionObjectOrCreate()
        {
            return GPActionUtils.GetGPActionObjectOrCreate(this.gameObject);
        }

        public virtual GPActionObjectMapper GetGPActionObjectMapperOrCreate()
        {
            return GPActionUtils.GetGPActionObjectMapperOrCreate(this.gameObject);
        }

        public virtual GPAction AddAction(System.Type actionType)
        {
            GPAction action = GetGPActionObjectMapperOrCreate().AddAction(this,actionType);

            action.enabled = false;

            action._name = System.Guid.NewGuid().ToString();

            action.SetParentHandler(this);

            return action;
        }

        #endregion

	
    }
}
