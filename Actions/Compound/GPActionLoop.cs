//
// GPActionLoop.cs
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

namespace ActionTool
{	
	[GPActionAlias("Compound/Loop")]
	[System.Serializable]
	public class GPActionLoop : GPActionCompound
	{
		public enum LoopType
		{
			INFINITE,
			FIXED_COUNT,
			STOP_EVENT,
		}

		#region Private Members

		/// <summary>
		/// Index of action currently playing
		/// </summary>
		private int m_currActionIndex;

		/// <summary>
		/// Number of loop already processed
		/// </summary>
		private int m_currLoopCount;

		private bool m_dueToRestart;

		#endregion

		#region Public Members

		/// <summary>
		/// Type of loop
		/// </summary>
		public LoopType _type;

		/// <summary>
		/// Max number of loop (valid for type FIXED_COUNT)
		/// </summary>
		public int _maxloopCount;

		/// <summary>
		/// Event that stops the loop (valid for type STOP_EVENT)
		/// </summary>
		public string _stopEvent;

		#endregion
		
		#region Properties
		
		public GPAction CurrentAction
		{
			get{ return ActionAtIndex(m_currActionIndex); }
		}
		
		public int CurrentActionIndex
		{
			get{ return m_currActionIndex; }
		}
		
		#endregion


		#region GPAction Override
		
		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
		protected override void OnTrigger()
		{
			if(_type == LoopType.STOP_EVENT)
				EventManager.Instance.Register(_stopEvent,StopEvent);

			m_currLoopCount = 0;

			// Stop previous running action
			
			if(m_currActionIndex >= 0 && m_currActionIndex < ActionCount() &&
				ActionAtIndex(m_currActionIndex).IsRunning)
				ActionAtIndex(m_currActionIndex).Stop();
			
			// (re)start from 0
			
			m_currActionIndex = 0;
			
			ActionAtIndex(m_currActionIndex).Trigger();
		}
		
		/// <summary>
		/// Raised each frame while action is running.
		/// Calling GPAction.End or GPAction.Stop will stop updates.
		/// </summary>
		/// <param name="dt">Dt.</param>
		protected override void OnUpdate()
		{
			if(this.HasEnded || m_currActionIndex >= ActionCount())
				return;

			if(m_dueToRestart)
			{
				ActionAtIndex(m_currActionIndex).Trigger();

				m_dueToRestart = false;
			}

			if(ActionAtIndex(m_currActionIndex).HasEnded)
			{
                if (ActionAtIndex(m_currActionIndex).State == ActionState.FAILURE)
                {
                    End(ActionState.FAILURE);
                    return;
                }

                // Else Terminated

				m_currActionIndex++;
				
				if(m_currActionIndex >= ActionCount())
				{ 
					EndLoop();
					return;
				}
				else
					ActionAtIndex(m_currActionIndex).Trigger();
			}
			else
			{
				ActionAtIndex(m_currActionIndex).Update();
			}
		}
		
		/// <summary>
		/// Raised when GPAction.Stop is called.
		/// </summary>
		protected override void OnInterrupt()
		{
			if(m_currActionIndex >= ActionCount() || 
			   !ActionAtIndex(m_currActionIndex).IsRunning)
			{
				return;
			}
			
			ActionAtIndex(m_currActionIndex).Stop();

			if(_type == LoopType.STOP_EVENT)
				EventManager.Instance.Unregister(_stopEvent,StopEvent);
		}

		protected override void OnTerminate()
		{
			if(_type == LoopType.STOP_EVENT)
				EventManager.Instance.Unregister(_stopEvent,StopEvent);
		}
		
		#endregion

		#region Loop Management

		private void EndLoop()
		{
			switch(_type)
			{
			case LoopType.INFINITE:
			case LoopType.STOP_EVENT:
				StartNewLoop();
				break;
			case LoopType.FIXED_COUNT:
			{
				if(m_currLoopCount+1 < _maxloopCount)
					StartNewLoop();
				else
					End ();
				break;
			}
			}

		}

		private void StartNewLoop()
		{
			m_currLoopCount++;
			m_currActionIndex = 0;

			m_dueToRestart = true;
		}

		#endregion

		#region Event

		private void StopEvent(GPEvent evt)
		{
			if(evt.EventID.Name == _stopEvent)
			{
				ActionAtIndex(m_currActionIndex).Stop();
				End();
			}
		}

		#endregion
	}
}
