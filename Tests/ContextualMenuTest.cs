using System;
using System.Collections;
using com.chwar.xrui.UIElements;
using NUnit.Framework;
using UnityEditor;
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