#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ActionTool
{
	#region Node

	public interface INodeOwner
	{
		Rect WindowRect
		{
			get; set;
		}
	}

	[System.Serializable]
	public class ActionEditorNode
	{
		public INodeOwner _owner;
		
		public bool _selected;
		
		public ActionEditorConnection _connection;

		public Vector2 _center;

		public void Draw()
		{
			if(_selected)
				Handles.color = Color.white;
			else
				Handles.color = Color.gray;

			if(_connection == null)
				Handles.DrawWireDisc((Vector3)_center,Vector3.forward,5);
			else
				Handles.DrawSolidDisc((Vector3)_center,Vector3.forward,5);
		}
	}
	
	#endregion
	
	#region NodeConnection
	
	public class ActionEditorConnection
	{
		public ActionEditorNode _nodeParent;
		public ActionEditorNode _nodeChild;

		public bool IsValid
		{
			get{ return _nodeParent != null && _nodeChild != null; }
		}

		public ActionEditorConnection(ActionEditorNode parent, ActionEditorNode child)
		{
			_nodeParent = parent;
			_nodeChild = child;
		}
	}
	
	#endregion

    [System.Serializable]
    public class GPAction : UnityEngine.MonoBehaviour , INodeOwner
    {
        public enum ActionState
        {
            NONE,
            RUNNNING,
            TERMINATED,
			FAILURE
        }

        #region Private Members

		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private EventHandler m_parentHandler;

        /// <summary>
        /// Current state of action
        /// </summary>
        private ActionState m_currState = ActionState.NONE;

        #endregion 

		#region Public Members

		public string _name;

#if UNITY_EDITOR

		[UnityEngine.HideInInspector]
		public Rect _windowRect = new Rect(0,0,100,50);

		[UnityEngine.HideInInspector]
		public ActionEditorNode _leftNode;

		[UnityEngine.HideInInspector]
		public List<ActionEditorNode> _rightNodes;

#endif

		#endregion

        #region Properties

		public string EditionName
		{
			get; set;
		}

		public ActionState State
		{
			get{ return m_currState; }
		}

        /// <summary>
        /// Returns whether or not the action is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return m_currState == ActionState.RUNNNING; }
        }


        /// <summary>
        /// Returns whether or not the action has ended
        /// </summary>
        public bool HasEnded
        {
            get { return (this.HasStarted && m_currState != ActionState.RUNNNING); }
        }

        /// <summary>
        /// Returns whether or not the action has ended.
        /// </summary>
        public bool HasStarted
        {
            get 
            { return (m_currState != ActionState.NONE); 
            }
        }

		/// <summary>
		/// Parent game object
		/// </summary>
		/// <value>The parent game object.</value>
		public GameObject ParentGameObject
		{
			get{ return m_parentHandler.gameObject; }
		}

		/// <summary>
		/// Parent event handler.
		/// </summary>
		/// <value>The parent handler.</value>
		public EventHandler ParentHandler
		{
			get{ return m_parentHandler; }
			set
			{
				SetParentHandler(value);
			}
		}

#if UNITY_EDITOR

		public Rect WindowRect
		{
			get{ return _windowRect; }
			set{ _windowRect = value; }
		}

#endif

        #endregion

		#region Constructor

		public GPAction()
		{
			CreateNodes();
		}

		#endregion

        #region Public Interface

        public void Trigger()
        {
            m_currState = ActionState.RUNNNING;
            OnTrigger();
        }

        public void Update()
        {
            if(HasEnded)
                return;
                
            OnUpdate();
        }

		/// <summary>
		/// Interrupt action
		/// </summary>
		public void Stop()
		{
			if(m_currState == ActionState.RUNNNING)
				OnInterrupt();
		}

        #endregion

        #region Override Interface

		public virtual void SetParentHandler(EventHandler handler)
		{
			m_parentHandler = handler;
		}

		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
        protected virtual void OnTrigger()
        {
        }

		/// <summary>
		/// Raised each frame while action is running.
		/// Calling GPAction.End or GPAction.Stop will stop updates.
		/// </summary>
		/// <param name="dt">Dt.</param>
        protected virtual void OnUpdate()
        {
        }

		/// <summary>
		/// Raised when GPAction.Stop is called.
		/// </summary>
		protected virtual void OnInterrupt()
		{
		}

		/// <summary>
		/// Raised when action ended (typically when GPAction.End is called)
		/// </summary>
		protected virtual void OnTerminate()
		{
		}

		public virtual void OnDrawGizmos()
		{
		}

		public virtual void OnDrawGizmosSelected()
		{
		}

		/// <summary>
		/// Should be called by subclass to terminate action.
		/// </summary>
        protected virtual void End(ActionState state = ActionState.TERMINATED)
        {
			m_currState = state;
			OnTerminate();
        }

		#region Node

		protected virtual void CreateNodes()
		{
			CreateLeftNode();

			_rightNodes = new List<ActionEditorNode>();
		}

		protected virtual ActionEditorNode CreateLeftNode()
		{
			if(this.GetType().GetCustomAttributes(typeof(GPActionNoInputAttribute),false).Length > 0)
			{
				_leftNode = null;
				return _leftNode;
			}

			_leftNode = new ActionEditorNode();
			
			_leftNode._owner = this;
			_leftNode._connection = null;
			_leftNode._center = new Vector2(8,25);

			return _leftNode;
		}

		protected virtual ActionEditorNode AddRightNode()
		{
			_rightNodes.Add(new ActionEditorNode());

			_rightNodes.Last()._owner = this;
			_rightNodes.Last()._connection = null;
			_rightNodes.Last()._center = new Vector2(92,25+16*(_rightNodes.Count-1));

			_windowRect.height = Mathf.Max(40,35+16*(_rightNodes.Count-1));

			return _rightNodes.Last();
		}
        
		#endregion

        #endregion

#if UNITY_EDITOR

		public virtual void DrawWindowContent()
		{
		}

#endif
    }

	public interface IActionOwner 
	{
		void Connect(GPAction child);
		void Disconnect(GPAction child);
		void DisconnectAll();
	}
}

