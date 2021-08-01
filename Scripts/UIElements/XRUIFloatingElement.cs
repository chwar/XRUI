namespace com.chwar.xrui.UIElements
{
    public class XRUIFloatingElement : XRUIElement
    {
        
        /// <summary>
        /// Destroys the XRUI floating element.
        /// </summary>
        public void Destroy()
        {
            XRUI.Instance.DestroyXRUIFloatingElement(this.gameObject);
        }
    }
}