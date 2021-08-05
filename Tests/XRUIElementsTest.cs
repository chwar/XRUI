using NUnit.Framework;
using UnityEngine;

namespace com.chwar.xrui.Tests
{
    [TestFixture]
    public class XRUIElementsTest
    {
        [SetUp]
        public void Init()
        {
            var go = new GameObject() {name = "Modal"};
            var xrui = go.AddComponent<XRUI>();
            xrui.xruiConfigurationAsset = Resources.Load<XRUIConfiguration>("DefaultXRUIConfiguration");
            Debug.Log("XRUI Initialized");
        }
        
        
    }
}