
namespace XNetCore.CEF.Runner
{
    interface JsObject
    {
        string Name { get; }
        BrowserService Instance { get; }
    }


    internal class JsObjectImpl : JsObject
    {
        public string Name { get; set; }
        public BrowserService Instance { get; set; }
    }

}
