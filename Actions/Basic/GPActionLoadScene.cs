//
// GPActionLoadScene.cs
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
	[GPActionAlias("Basic/Load Scene")]
	public class GPActionLoadScene : GPAction
	{
        public enum LoadMode
        {
            STANDARD,
            ASYNC
        }
        public enum AsyncLoadAction
        {
            PLAY_WHEN_READY,
            WAIT_FOR_ACTIVATE,
            ACTIVATE
        }
        #region Private Members

        private static AsyncOperation M_AsyncLoadBuffer;

        #endregion

        #region Public Members

        public string m_name;

        public LoadMode _mode;
        public AsyncLoadAction _asyncLoadAction = AsyncLoadAction.PLAY_WHEN_READY;

        #endregion

		#region Properties

		public AsyncOperation LoadingOp
		{
			get{ return M_AsyncLoadBuffer; }
		}

		#endregion

        #region GPAction Override

        /// <summary>
		/// Raised each time action is triggered
		/// </summary>
		protected override void OnTrigger()
		{
            if(_mode == LoadMode.STANDARD){
                Application.LoadLevel(m_name);
            }
            else if (_mode == LoadMode.ASYNC)
            {
                switch (_asyncLoadAction)
                {
                    case AsyncLoadAction.PLAY_WHEN_READY :
                        Application.LoadLevelAsync(m_name);
                        break;

                    case AsyncLoadAction.WAIT_FOR_ACTIVATE :
                        if (M_AsyncLoadBuffer != null) 
                        {
                            Debug.LogWarning("An Async Level Loading is already waiting! It will be overidden by the action.");
                        }    
                        M_AsyncLoadBuffer = Application.LoadLevelAsync(m_name);
                        M_AsyncLoadBuffer.allowSceneActivation = false;
                        break;

                    case AsyncLoadAction.ACTIVATE :
                        if (M_AsyncLoadBuffer == null)
                        {
                            Debug.LogError("There is no Async Level loading waiting for activation. Abort !");
                        }
                        else
                        {
                            M_AsyncLoadBuffer.allowSceneActivation = true;
                        }
                        break;
                }
                
            }
			End();

		}
		
		#endregion
	}
}