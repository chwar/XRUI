// MIT License
// Copyright (c) 2022 Pablo Gutierrez Millares
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Custom class for input modules that send UI input in XR. Adapted for UIToolkit runtime worldspace panels.
    /// </summary>
    public class XRUIInputModuleFix : UIInputModuleFix
    {
        struct RegisteredInteractor
        {
            public IUIInteractor interactor;
            public TrackedDeviceModel model;

            public RegisteredInteractor(IUIInteractor interactor, int deviceIndex)
            {
                this.interactor = interactor;
                model = new TrackedDeviceModel(deviceIndex);
            }
        }

        [SerializeField]
        [Tooltip("If true, will forward 3D tracked device data to UI elements.")]
        bool m_EnableXRInput = true;

        [SerializeField]
        protected bool usePenPointerIdBase;
        
        int m_RollingInteractorIndex = PointerId.touchPointerIdBase;
        bool m_penBaseIdAssigned;

        readonly List<RegisteredInteractor> m_RegisteredInteractors = new();
        

        public override int ConvertUIToolkitPointerId(PointerEventData sourcePointerData)
        {
            return sourcePointerData.pointerId;
        }

        /// <summary>
        /// Register an <see cref="IUIInteractor"/> with the UI system.
        /// Calling this will enable it to start interacting with UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to use.</param>
        public void RegisterInteractor(IUIInteractor interactor)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                    return;
            }

            if (usePenPointerIdBase && !m_penBaseIdAssigned)
            {
                m_penBaseIdAssigned = true;
                m_RegisteredInteractors.Add(new RegisteredInteractor(interactor, PointerId.penPointerIdBase));
            }
            else
            {
                if (usePenPointerIdBase && m_RollingInteractorIndex == PointerId.penPointerIdBase)
                    ++m_RollingInteractorIndex;
                
                m_RegisteredInteractors.Add(new RegisteredInteractor(interactor, m_RollingInteractorIndex++));
            }
        }

        /// <summary>
        /// Unregisters an <see cref="IUIInteractor"/> with the UI system.
        /// This cancels all UI Interaction and makes the <see cref="IUIInteractor"/> no longer able to affect UI.
        /// </summary>
        /// <param name="interactor">The <see cref="IUIInteractor"/> to stop using.</param>
        public void UnregisterInteractor(IUIInteractor interactor)
        {
            for (var i = 0; i < m_RegisteredInteractors.Count; i++)
            {
                if (m_RegisteredInteractors[i].interactor == interactor)
                {
                    var registeredInteractor = m_RegisteredInteractors[i];
                    registeredInteractor.interactor = null;
                    m_RegisteredInteractors[i] = registeredInteractor;
                    return;
                }
            }
        }

        protected override void DoProcess()
        {
            base.DoProcess();

            if (m_EnableXRInput)
            {
                for (var i = 0; i < m_RegisteredInteractors.Count; i++)
                {
                    var registeredInteractor = m_RegisteredInteractors[i];

                    // If device is removed, we send a default state to unclick any UI
                    if (registeredInteractor.interactor == null)
                    {
                        registeredInteractor.model.Reset(false);
                        ProcessTrackedDevice(ref registeredInteractor.model, true);
                        m_RegisteredInteractors.RemoveAt(i--);
                    }
                    else
                    {
                        registeredInteractor.interactor.UpdateUIModel(ref registeredInteractor.model);
                        ProcessTrackedDevice(ref registeredInteractor.model);
                        m_RegisteredInteractors[i] = registeredInteractor;
                    }
                }
            }
        }
    }
}
