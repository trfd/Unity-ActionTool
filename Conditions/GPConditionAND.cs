//
// GPConditionAND.cs
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ActionTool
{
    [GPConditionAlias("Logic/AND")]
    [ExecuteInEditMode]
    public class GPConditionAND : GPCondition
    {
        #region Private Members

        [SerializeField]
        private GPCondition m_a;

        [SerializeField]
        private GPCondition m_b;

        #endregion

        #region Properties
        public GPCondition A
        {
            get { return m_a; }
            set
            {
                m_a = value;

                if (m_a != null)
                    m_a.SetHandler(this.Handler);
            }
        }

        public GPCondition B
        {
            get { return m_b; }
            set
            {
                m_b = value;

                if (m_b != null)
                    m_b.SetHandler(this.Handler);
            }
        }
        #endregion

        public override bool Evaluate() { return m_a.Evaluate() && m_b.Evaluate(); }

        public override void SetHandler(EventHandler handler)
        {
            base.SetHandler(handler);

            if (m_a != null)
                m_a.SetHandler(handler);

            if (m_b != null)
                m_b.SetHandler(handler);
        }
        void OnDestroy()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (m_a) GameObject.DestroyImmediate(m_a);
                if (m_b) GameObject.DestroyImmediate(m_b);
            };
#else
            if(m_a) GameObject.Destroy(m_a);
            if(m_b) GameObject.Destroy(m_b);
#endif
        }
    }
}