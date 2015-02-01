//
// GPEvent.cs
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
    [System.Serializable]
    public class GPEventID
    {
        #region Static

        public static GPEventID Invalid = new GPEventID {ID = -1, Name = "Invalid"};

        #endregion

        #region Private Members

        [UnityEngine.SerializeField]
        private int m_ID;

        [UnityEngine.SerializeField]
        private string m_name;

        #endregion

        #region Properties

        public int ID
        {
            get { return m_ID;  }
            set { m_ID = value; }
        }

        public string Name
        {
            get { return m_name;  }
            set { m_name = value; }
        }

        #endregion

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            GPEventID p = obj as GPEventID;
            if ((System.Object)p == null)
            {
                return false;
            }

            return (ID == p.ID);
        }

        public bool Equals(GPEventID p)
        {
            if (p == null)
            {
                return false;
            }

            return (ID == p.ID);
        }

		public override int GetHashCode ()
		{
			return ID;
		}
    }

	public class GPEvent 
	{
		#region Properties

	    public GPEventID EventID
	    {
	        get; set;
	    }

		public UnityEngine.Object RelatedObject
		{
			get; set;
		}
	
		#endregion
	}
}