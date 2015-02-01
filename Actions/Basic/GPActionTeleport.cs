//
// GPActionTeleport.cs
//
// Author(s):
//       Baptiste Dupy <baptiste.dupy@gmail.com>
//       Fabien Ziebel <nexibaf@gmail.com>
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
	/// <summary>
	/// Based on Teleport behaviour
	/// </summary>
	[GPActionAlias("Basic/Teleport To")]
	public class GPActionTeleport : GPAction
	{
		#region Public Members

		public GameObject _destination;
		public bool _keepOffset = false;
		public bool _keepTargetRotation = false;
		
		#endregion
		
		#region GPAction Override
		
		/// <summary>
		/// Raised each time action is triggered
		/// </summary>
		protected override void OnTrigger()
		{
			if(ParentHandler.CurrentEvent.RelatedObject == null)
			{
				Debug.LogError("Need something to teleport");
				End();
				return;
			}

			if(ParentHandler.CurrentEvent.RelatedObject is Collider)
				Teleport((Collider)ParentHandler.CurrentEvent.RelatedObject);
			else if(ParentHandler.CurrentEvent.RelatedObject is GameObject)
			{
				Collider coll = ((GameObject)ParentHandler.CurrentEvent.RelatedObject).GetComponent<Collider>();

				if(coll != null)
					Teleport(coll);
			}

			End();
		}
		
		#endregion

		private void Teleport(Collider other)
		{
			if (this._destination == null)
			{
				Debug.LogError ("Trigger Teleport : the teleport lack the destination !!");
				return;
			}
			
			//Determine whether the target appear exactly at destination or with the offset he had with the trigger
			if(_keepOffset)
			{
				other.gameObject.transform.position = _destination.transform.position + 
					(other.gameObject.transform.position - this.ParentGameObject.transform.position) ;
			}
			else{
				other.gameObject.transform.position = _destination.transform.position ;
			}
			
			//Determine whether the target get the same rotation as the destination
			if (!_keepTargetRotation){
				other.gameObject.transform.forward = _destination.transform.forward;
				if(other.rigidbody){
					other.rigidbody.velocity =  new Vector3(0,0,0);
				}
			}
		}

		#region Gizmos

		/// <summary>
		/// Render this instance.
		/// </summary>
		private void Render()
		{
			Gizmos.DrawSphere (this.ParentGameObject.transform.position,0.2f);
		
			if (_destination == null)
				return;

			Gizmos.DrawLine (this.ParentGameObject.transform.position, _destination.transform.position);
		}

		public override void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Render();
		}

		public override void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Render();
		}


		#endregion
	}
}