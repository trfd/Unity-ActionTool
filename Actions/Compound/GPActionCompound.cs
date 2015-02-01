//
// GPActionCompound.cs
//
// Author(s):
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ActionTool
{
	[GPActionHide]
	[System.Serializable]
	public class GPActionCompound : GPAction , IActionOwner , ISerializationCallbackReceiver
	{
		#region Private Members

		/// <summary>
		/// List of GPAction of compound action
		/// </summary>
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		private List<GPAction> m_actions;

		#endregion

		#region Public Members

		#endregion

		#region Constructor

		public GPActionCompound()
		{
			m_actions = new List<GPAction>();
		}

		#endregion

		#region Public Interface


		#region GPAction Override

		public override void SetParentHandler(EventHandler handler)
		{
			base.SetParentHandler(handler);

			foreach(GPAction action in m_actions)
				action.SetParentHandler(handler);
		}

		public override void OnDrawGizmos()
		{
			foreach(GPAction action in m_actions)
				action.OnDrawGizmos();
		}
		
		public override void OnDrawGizmosSelected()
		{
			foreach(GPAction action in m_actions)
				action.OnDrawGizmosSelected();
		}

		#endregion

		#region Children Action Management

		public virtual int ActionCount()
		{
			return m_actions.Count;
		}

		public virtual GPAction ActionAtIndex(int idx)
		{
			return m_actions[idx];
		}

		public virtual GPAction AddAction(System.Type t)
		{
			return AddAction(this.ParentHandler.GetGPActionObjectMapperOrCreate().AddAction(this.ParentHandler,t));
		}

		public virtual GPAction AddAction(GPAction action)
		{
			m_actions.Add(action);

#if UNITY_EDITOR
			/*
			_rightNodes[m_actions.Count-1]._connection = 
				new ActionEditorConnection(_rightNodes[m_actions.Count],m_actions.Last()._leftNode);

			m_actions.Last()._leftNode._connection = _rightNodes[m_actions.Count]._connection;

			AddRightNode();
			*/
			CreateAllRightNodes();
#endif
			return m_actions.Last();
		}

		public virtual void SetActionAt(int idx, GPAction action)
		{
#if UNITY_EDITOR			
			/*
			_rightNodes[idx]._connection = new ActionEditorConnection(_rightNodes[idx],action._leftNode);
			m_actions[idx]._leftNode._connection = _rightNodes[idx]._connection;
			*/
			CreateAllRightNodes();
#endif
			m_actions[idx] = action;
		}

		public virtual void RemoveAction(GPAction action)
		{
			int idx = m_actions.IndexOf(action);

			RemoveActionAt(idx);
		}

		public virtual void RemoveActionAt(int idx)
		{
			m_actions.RemoveAt(idx);

#if UNITY_EDITOR			
			/*
			_rightNodes.RemoveAt(idx);
			m_actions[idx]._leftNode._connection = null;
			*/
			CreateAllRightNodes();
#endif

		}

		#endregion

		#region Nodes

		protected override void CreateNodes()
		{
			base.CreateNodes();

			//AddRightNode();

			CreateAllRightNodes();
		}

		protected void CreateAllRightNodes()
		{
			if(m_actions == null)
				m_actions = new List<GPAction>();

			if(_rightNodes != null)
				_rightNodes.Clear();
			else
				_rightNodes = new List<ActionEditorNode>();

			foreach(GPAction action in m_actions)
			{
				ActionEditorNode node = AddRightNode();

				node._owner = this;
				node._connection = new ActionEditorConnection(node,action._leftNode);
				action._leftNode._connection = node._connection;
			}

			AddRightNode();
		}

		#endregion

		#endregion

		#region IActionOwner

		public void Connect(GPAction child)
		{
			AddAction(child);
		}

		public void Disconnect(GPAction child)
		{
			RemoveAction(child);
		}

		public void DisconnectAll()
		{
			foreach(ActionEditorNode node in _rightNodes)
			{
				if(node._connection == null)
					continue;

				node._connection._nodeChild._connection = null;
				node._connection._nodeParent._connection = null;
			}

			m_actions.Clear();

			CreateAllRightNodes();
		}

		#endregion

		#region ISerializable Callback

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			CreateAllRightNodes();
		}

		#endregion
	}
}