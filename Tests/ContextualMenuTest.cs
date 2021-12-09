// MIT License
// Copyright (c) 2021 Chris Warin
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace com.chwar.xrui.Tests
{
    public class ContextualMenuTest
    {
        private GameObject _go;

        [SetUp]
        public void Init()
        {
            _go = new GameObject() {name = "XRUI"};
            var xrui = _go.AddComponent<XRUI>();
            _go.AddComponent<Camera>().tag = "MainCamera";
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            Debug.Log("XRUI Initialized");
        }

        [TearDown]
        public void Cleanup()
        {
            GameObject.DestroyImmediate(_go);
            var contextualMenu = GameObject.FindObjectOfType<XRUIContextualMenu>();
            // Already deleted when clicked upon
            if(contextualMenu is not null)
                GameObject.DestroyImmediate(contextualMenu.gameObject);
        }

        [Test]
        public void ContextualMenuTestCreate()
        {
            XRUI.Instance.ShowContextualMenu(new Vector2(0, 0), true);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            Assert.NotNull(context);
        }

        [Test]
        public void ContextualMenuTestCreateWithOffset()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(0, 0), true, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            Assert.True(context.positionOffsetLeft == 10f && context.positionOffsetRight == 10f);
        }

        [Test]
        public void ContextualMenuTestAddEntryWithoutTemplate()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(0, 0), true, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            context.menuElementTemplate = null;
            Assert.Throws<ArgumentNullException>(() => context.AddMenuElement());
        }
        
        [Test]
        public void ContextualMenuTestAddEntry()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(0, 0), true, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            var entry = context.AddMenuElement();
            entry.Q<Label>("Text").text = "Test";
            Assert.True(context.UIDocument.rootVisualElement.Contains(entry));
        }

        [Test]
        public void ContextualMenuTestPositionRelativeToParent()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(0, 0), true, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            context.PositionRelativeToParent(null);
            Assert.True(context.positionOffsetLeft == 10f && context.positionOffsetRight == 10f);
        }
        
        [UnityTest]
        public IEnumerator ContextualMenuTestPositionRelativeToParentRight()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(1920, 0), false, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            yield return new WaitForSeconds(1);
            context.PositionRelativeToParent(null);
            Assert.True(context.positionOffsetLeft == 10f && context.positionOffsetRight == 10f);
        }

        [UnityTest]
        public IEnumerator ContextualMenuTestDisposeMenu()
        {
            XRUI.Instance.ShowContextualMenu(null, new Vector2(1920, 0), false, 10f, 10f);
            var context = Object.FindObjectOfType<XRUIContextualMenu>();
            context.DisposeMenu();
            yield return new WaitForSeconds(1);
            Assert.Null(GameObject.FindObjectOfType<XRUIContextualMenu>());
        }
    }
}