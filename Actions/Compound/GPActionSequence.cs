//
// GPActionSequence.cs
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
	[GPActionAlias("Compound/Sequence")]
	[System.Serializable]
	public class GPActionSequence : GPActionCompound
	{
		#region Private Members

		private int m_currActionIndex;

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
		protected override void OnUpdate()
		{
			if(this.HasEnded || m_currActionIndex >= ActionCount())
				return;

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
					End(ActionState.TERMINATED); 
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
			if(m_currActionIndex >=  ActionCount() || 
			   !ActionAtIndex(m_currActionIndex).IsRunning)
			{
				return;
			}

			ActionAtIndex(m_currActionIndex).Stop();
		}

		#endregion
	}
}
