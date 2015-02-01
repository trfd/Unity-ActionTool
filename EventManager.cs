using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace ActionTool
{
	public class EventManager : MonoBehaviour
	{
		[System.Serializable]
		public class EventIDMap : Utils.DictionaryWrapper<string, GPEventID>
		{
		}

        #region Singleton

		private static EventManager m_instance;

		public static EventManager Instance 
        {
			get 
            {
				if (m_instance == null) 
                {
					m_instance = GameObject.FindObjectOfType<EventManager>();
					//DontDestroyOnLoad(m_instance.gameObject);
				}

				return m_instance;
			}
		}

		void Awake ()
		{
			if (m_instance == null) 
            {
				m_instance = this;
                m_instance.Init();

                //DontDestroyOnLoad(this);
            } 
            else
            {
				if (this != m_instance)
					Destroy (this.gameObject);
			}
		}

        #endregion

        #region Delegates

		public delegate void EventDelegate (GPEvent evt);

        #endregion

        #region Private Members

		private bool m_reservedEventsAdded;

		/// <summary>
		/// Whether or not the list of GPEventID is dirty.
		/// </summary>
		private bool m_isEventIDMapDirty = true;

		/// <summary>
		/// Map GPEventID.ID to listener delegates
		/// </summary>
		private Dictionary<int, EventDelegate> m_eventMap;
        
		/// <summary>
		/// Maps event string name to GPEventID for ease of use.
		/// </summary>
		private EventIDMap m_eventIDMap;

		/// <summary>
		/// List of all GPEventID declared.
		/// </summary>
		[UnityEngine.SerializeField]
		private List<GPEventID> m_eventIDList;

		/// <summary>
		/// List of event names
		/// </summary>
		private string[] m_eventIDNameList;

        #endregion

		#region Properties

		public Dictionary<string,GPEventID> EventMap 
		{
			get
			{ 
				if(m_isEventIDMapDirty)
					CreateEventIDMap();
			
				return m_eventIDMap.Dictionary; 
			}
		}

		public GPEventID[] EventIDs 
		{
		    get
		    {
                if(m_eventIDList == null)
                    m_eventIDList = new List<GPEventID>();

		        return m_eventIDList.ToArray();
		    }
		}

		public string[] EventNames 
		{
			get 
			{
				if(m_isEventIDMapDirty)
					CreateEventIDMap();

				return m_eventIDNameList;
			}
		}

		public bool ReservedEventsAdded
		{
			get{ return m_reservedEventsAdded; }
		}

		#endregion

	    private void Init()
	    {
	        m_eventMap = new Dictionary<int, EventDelegate>();
	    }

		void OnValidate()
		{
			if(!m_reservedEventsAdded)
				AddReservedEvents();
		}

        #region Registration

		public void Register (int evtID, EventDelegate del)
		{
            if (!IsIDRegistered(evtID))
		    {
                Debug.LogError("Event (ID: " + evtID+") is not registered in EventManager");
		    }
             
			try
            {
				m_eventMap[evtID] += del;
			} 
            catch (KeyNotFoundException) 
            {
				m_eventMap.Add(evtID,del);
			}
		}

		public void Register(string evtName, EventDelegate del)
		{
			try 
            {
				GPEventID evtID = EventNameToID(evtName);

				if (evtID.Equals (GPEventID.Invalid))
					throw new KeyNotFoundException ();		

				Register (evtID.ID, del);
			} 
            catch (KeyNotFoundException) 
            {
				Debug.LogError ("Can not register for event " + evtName);
			}
		}

		public void Unregister (int evtID, EventDelegate del)
		{
			try 
            {
				m_eventMap [evtID] -= del;
			} 
            catch (KeyNotFoundException) 
            {
			}
		}

		public void Unregister (string evtName, EventDelegate del)
		{
			try 
			{
				GPEventID evtID = EventNameToID (evtName);
				
				if (evtID == GPEventID.Invalid)
					throw new KeyNotFoundException ();		

				Unregister (evtID.ID, del);
			} 
			catch (KeyNotFoundException) 
			{
			}
		}

		public void RefreshIDList()
		{
			CreateEventIDMap();
		}

        #endregion

		#region EventID Management

		public GPEventID EventNameToID(string evtName)
		{
            if(m_isEventIDMapDirty)
                CreateEventIDMap();

			try 
			{
				return m_eventIDMap.Dictionary [evtName];
			}
			catch (KeyNotFoundException) 
			{
				return GPEventID.Invalid;
			}
		}

		public void AddEventName()
		{
			int maxID = 0;

			foreach(GPEventID evtID in m_eventIDList) 
			{
                int id = evtID.ID;

				if (maxID <= id)
					maxID = id;
			}

			string newEventName = "Unnammed "+(maxID+1).ToString();

			m_eventIDList.Add(new GPEventID{ ID=maxID+1, Name=newEventName });

			m_isEventIDMapDirty = true;
		}

        public void AddEventName( string eventName )
        {
            if (this.NameExist(eventName)) return;

            int maxID = 0;

            foreach (GPEventID evtID in m_eventIDList)
            {
                int id = evtID.ID;

                if (maxID <= id)
                    maxID = id;
            }

            m_eventIDList.Add(new GPEventID { ID = maxID + 1, Name = eventName });

            m_isEventIDMapDirty = true;
        }

		private void AddEventName(int id, string eventName )
		{
			if (this.NameExist(eventName)) return;

			if(m_eventIDList == null)
				m_eventIDList = new List<ActionTool.GPEventID>();

			m_eventIDList.Add(new GPEventID { ID = id, Name = eventName });
			
			m_isEventIDMapDirty = true;
		}

		public void AddReservedEvents()
		{
			AddEventName(-1, "Start");
			AddEventName(-2, "OnCollisionEnter");
			AddEventName(-3, "OnCollisionExit");
			AddEventName(-4, "OnTriggerEnter");
			AddEventName(-5, "OnTriggerExit");
			AddEventName(-6, "OnEnable");
			AddEventName(-7, "OnDisable");
			AddEventName(-8, "EvtInteract");
			AddEventName(-9, "EvtInteractibleEnter");
			AddEventName(-10,"EvtInteractibleStay");
			AddEventName(-11,"EvtInteractibleExit");
			AddEventName(-12,"EvtTrigger");

			m_reservedEventsAdded = true;
		}

		public void RemoveEventName(GPEventID id)
		{
			m_eventIDList.Remove(id);

			m_isEventIDMapDirty = true;
		}

		public void CheckNames(GPEventID id)
		{
			Debug.Log ("Map matching: " + (m_eventIDMap.Dictionary [id.Name].ID == id.ID));

			foreach (GPEventID eventId in m_eventIDList) 
			{
				if (eventId.ID == id.ID) 
				{
					Debug.Log ("List matching: " + (eventId.Name == id.Name));
					break;
				}
			}
		}

        public bool NameExist (string name)
		{
			if(m_eventIDList == null)
				return false;

            foreach (GPEventID eventId in m_eventIDList)
            {
                if (eventId.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

		private void CreateEventIDMap()
		{
			m_eventIDMap = new EventIDMap();
			m_eventIDNameList = new string[m_eventIDList.Count];

			for(int i=0 ; i<m_eventIDList.Count ; i++)
			{
				m_eventIDMap.Dictionary.Add(m_eventIDList[i].Name,m_eventIDList[i]);
				m_eventIDNameList[i] = m_eventIDList[i].Name;
			}

			m_isEventIDMapDirty = false;
		}

		public int IndexOfEventID (GPEventID id)
		{
			if (m_isEventIDMapDirty)
				CreateEventIDMap();

			return m_eventIDList.IndexOf(id);
		}

		public int IndexOfID (int id)
		{
			if (m_isEventIDMapDirty)
				CreateEventIDMap();

			for (int i=0; i<m_eventIDList.Count; i++) {
				if (m_eventIDList [i].ID == id)
					return i;
			}

			return -1;
		}

        /// <summary>
        /// Check if ID is registered in EventManager ID Map.
        /// (Attention, it does not check whether or not a delegate is set for this event ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    public bool IsIDRegistered(int id)
	    {
            foreach (GPEventID eventId  in m_eventIDList)
            {
                if (eventId.ID == id)
                    return true;
            }

            return false;
	    }

		#endregion

        #region Post Events

		public void PostEvent(string evtName, GameObject obj = null)
		{
			EventDelegate value;

			try 
			{
				GPEventID evtID = EventNameToID (evtName);
				
				if (evtID == GPEventID.Invalid)
					throw new KeyNotFoundException ();
				
				if (m_eventMap.TryGetValue (evtID.ID, out value)) 
				{
					GPEvent evt = new GPEvent();

					evt.EventID = evtID;
					evt.RelatedObject = obj;

					value (evt);
				} 
				else
					throw new KeyNotFoundException ();
			} 
			catch (KeyNotFoundException) 
			{
			}
		}

        public void PostRelativeEvent(GameObject postToObj, string evtName, GameObject obj = null)
        {
            if(postToObj == null)
                throw new NullReferenceException();

            EventDelegate value;

            try
            {
                GPEventID evtID = EventNameToID(evtName);

                if (evtID == GPEventID.Invalid)
                    throw new KeyNotFoundException();

                EventHandler handlerToTrigger = null;
                
                EventHandler[] handlers = postToObj.GetComponents<EventHandler>();

                foreach(EventHandler handler in handlers)
                {
                    if (handler._eventID.Equals(evtID))
                    {
                        if(handlerToTrigger == null)
                            handlerToTrigger = handler;
                        else
                            Debug.LogError("Several handler listening to "+evtID.Name+" found in object "+
                                postToObj.name+". Only the first handler will be triggered");
                       
                    }
                }

                if(handlerToTrigger == null)
                {
                    Debug.LogWarning("Can not post relative event to "+ postToObj.name +
                        ". No event handler listening to " + evtID.Name + " found in object.");
                    return;
                }
                    
                if (m_eventMap.TryGetValue(evtID.ID, out value))
                {
                    GPEvent evt = new GPEvent();

                    evt.EventID = evtID;
                    evt.RelatedObject = obj;

                    handlerToTrigger.EventTrigger(evt);
                }
                else
                    throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        #endregion
	}
}
