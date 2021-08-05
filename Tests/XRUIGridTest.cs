using System.Collections;
using System.Collections.Generic;
using com.chwar.xrui;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.chwar.xrui.Tests
{
    public class XRUIGridTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void XRUIGridTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator XRUIGridTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
