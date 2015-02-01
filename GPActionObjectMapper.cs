//
// GPActionObjectMapper.cs
//
// Author:
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//
// Copyright (c) 2014 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Utils;

namespace ActionTool
{
    public class GPActionObjectMapper : MonoBehaviour , ISerializationCallbackReceiver
    {
        [System.Serializable]
        public class GPActionObjectMap : DictionaryWrapper<EventHandler,GameObject>
        {}

	    #region Private Members

        /// <summary>
        /// Maps EventHanlder to GameObject. Those game object are GPActionHolderObject
        /// </summary>
        [UnityEngine.SerializeField]
		private GPActionObjectMap m_actionObjectMap;

	    #endregion

	    #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GPActionObjectMapper()
        {
            m_actionObjectMap = new GPActionObjectMap();
        }

	    #endregion

        #region Public Interface

        #region Context Menu

        [ContextMenu("Show Children")]
        private void ShowAllActions()
        {
            for (int i = 0; i < this.transform.childCount; ++i)
                this.transform.GetChild(i).gameObject.hideFlags = HideFlags.None;
        }


        [ContextMenu("Hide Children")]
        private void HideAllActions()
        {
            for (int i = 0; i < this.transform.childCount; ++i)
                this.transform.GetChild(i).gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        }

        #endregion

        /// <summary>
        /// Adds an EventHandler to the mapping.
        /// </summary>
        /// <param name="handler"></param>
        public GameObject AddEventHandler(EventHandler handler)
        {
			GameObject holder = CreateGPActionHolderObject(handler);
            m_actionObjectMap.Dictionary.Add(handler, holder);
			return holder;
        }

        /// <summary>
        /// Add an action to the EventHandler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public GPAction AddAction(EventHandler handler,System.Type actionType)
        {
            if (handler == null)
                throw new System.ArgumentNullException();

            if (!typeof(GPAction).IsAssignableFrom(actionType))
                throw new System.ArgumentException("Type 'actionType' must inherit from GPAction");

            GameObject holder;

            if(!m_actionObjectMap.Dictionary.TryGetValue(handler, out holder))
				holder = AddEventHandler(handler);

            return (GPAction) holder.AddComponent(actionType);
        }

		/// <summary>
		/// Removes the event handler from the map.
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void RemoveEventHandler(EventHandler handler)
		{
			try
			{
				DestroyImmediate(m_actionObjectMap.Dictionary[handler]);
				m_actionObjectMap.Dictionary.Remove(handler);
			}
			catch(KeyNotFoundException)
			{
				Debug.LogWarning("Try to remove unexisting handler");
			}
		}

		public GPAction[] GetAllActions(EventHandler handler)
		{
			if (handler == null)
				throw new System.ArgumentNullException();

			GameObject holder;
			
			if(m_actionObjectMap.Dictionary.TryGetValue(handler, out holder))
				return holder.GetComponents<GPAction>();
		
			return new GPAction[0];
		}

#if UNITY_EDITOR

		public void CheckPrefabConnection(EventHandler handler)
		{
			GameObject obj;
			
			try
			{
				obj = m_actionObjectMap.Dictionary[handler];
			}
			catch (KeyNotFoundException)
			{ 
				obj = AddEventHandler(handler); 
			}

			Object prefab = PrefabUtility.GetPrefabParent(obj);
			
			if(prefab == null)
				handler.PrefabAction = null;
		}

        /// <summary>
        /// Imports an action prefab for the specified EventHandler
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="prefab"></param>
        public void ImportGPActionObjectHolderPrefab(EventHandler handler,Object prefab)
        {
			GameObject instPrefab = (GameObject) PrefabUtility.InstantiatePrefab(prefab);

            try
			{ m_actionObjectMap.Dictionary[handler] = instPrefab; }
            catch (KeyNotFoundException) 
			{ m_actionObjectMap.Dictionary.Add(handler,instPrefab); }

			handler.PrefabAction = prefab;
			handler.Action = instPrefab.GetComponent<GPActionExport>()._rootAction;

			InitGPActionHolderObject(instPrefab);
        }

        /// <summary>
        /// Exports the current GPActionHolderObject as action prefab
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Object ExportGPActionObjectHolderPrefab(EventHandler handler)
        {
			GameObject obj;

            try
            {
				obj = m_actionObjectMap.Dictionary[handler];
            }
			catch (KeyNotFoundException) 
			{ 
				obj = AddEventHandler(handler); 
			}

			obj.GetComponentOrCreate<GPActionExport>()._rootAction = handler.Action;

			string str = EditorUtility.SaveFilePanel("Export Action","Assets","New Action","prefab");
			
			if(string.IsNullOrEmpty(str))
				return null;

			str = "Assets/"+FileUtils.GetRelativePath(str,Application.dataPath);

			handler.PrefabAction = PrefabUtility.CreatePrefab(str, obj, ReplacePrefabOptions.ConnectToPrefab);

			return handler.PrefabAction;
        }

        /// <summary>
        /// Applies the modification of GPActionHolderObject to its prefab if any.
        /// </summary>
        /// <param name="handler"></param>
        public void ApplyGPActionObjectHolderToPrefab(EventHandler handler)
        {
			GameObject obj;

            try
            {
                obj = m_actionObjectMap.Dictionary[handler];
            }
            catch (KeyNotFoundException)
			{ 
				obj = AddEventHandler(handler); 
			}

			Object prefab = PrefabUtility.GetPrefabParent(obj);

			if(prefab == null)
			{
				handler.PrefabAction = null;
				return;
			}

			PrefabUtility.ReplacePrefab(obj, prefab,ReplacePrefabOptions.ConnectToPrefab);
        }

        /// <summary>
        /// Resets all GPActionHolderObject modifications to the prefab values
        /// </summary>
        /// <param name="handler"></param>
        public void RevertGPActionObjectHolderToPrefab(EventHandler handler)
        {
			GameObject obj;
			
			try
			{
				obj = m_actionObjectMap.Dictionary[handler];
			}
			catch (KeyNotFoundException)
			{ 
				obj = AddEventHandler(handler); 
			}
			
			PrefabUtility.RevertPrefabInstance(obj);
        }

		public void ResetGPActionObjectHolder(EventHandler handler)
		{
			try
			{
				DestroyImmediate(m_actionObjectMap.Dictionary[handler]);
				m_actionObjectMap.Dictionary.Remove(handler);
			}
			catch (KeyNotFoundException)
			{
			}

			AddEventHandler(handler); 

			handler.Action = null;
		}

		public void BreakGPActionObjectHolderFromPrefab(EventHandler handler)
		{
			GameObject obj;
			
			try
			{
				obj = m_actionObjectMap.Dictionary[handler];
			}
			catch (KeyNotFoundException)
			{ 
				obj = AddEventHandler(handler); 
			}

			PrefabUtility.DisconnectPrefabInstance(obj);
			handler.PrefabAction = null;
		}

#endif

        #endregion

        #region Conditions

        public GPCondition AddCondition(System.Type type, EventHandler handler)
        {
            if (handler == null)
                throw new System.ArgumentNullException();

            if (!typeof(GPCondition).IsAssignableFrom(type))
                throw new System.ArgumentException("Type "+type.FullName+" must inherit from GPCondition");

            GameObject holder;

            if (!m_actionObjectMap.Dictionary.TryGetValue(handler, out holder))
                holder = AddEventHandler(handler);

            GPCondition condition = (GPCondition)holder.AddComponent(type);

            condition.SetHandler(handler);

            return condition;
        }

        #endregion

        #region Protected Interface

        /// <summary>
        /// Create a GPActionHolderObject in the hierarchy
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected GameObject CreateGPActionHolderObject(EventHandler handler)
        {
            GameObject holder = new GameObject();

            InitGPActionHolderObject(holder);

            return holder;
        }

        /// <summary>
        /// Init a GPActionHolderObject properties.
        /// </summary>
        /// <param name="holder"></param>
        protected void InitGPActionHolderObject(GameObject holder)
        {
			holder.name = "__Holder__";
            holder.transform.parent = this.transform;
            holder.hideFlags = HideFlags.HideInHierarchy;
        }

        #endregion

		#region Serialization Callback

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		#endregion
    }

    public class GPActionHolderObjectNotFoundException : System.Exception
    {
        public GPActionHolderObjectNotFoundException() 
            : base("GPAction Holder object not found")
        {}
    }
}
