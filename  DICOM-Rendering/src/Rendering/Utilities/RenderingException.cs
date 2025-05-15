namespace TheSSS.DicomViewer.Rendering.Utilities;

public class RenderingException : Exception
{
    public RenderingException() { }
    public RenderingException(string message) : base(message) { }
    public RenderingException(string message, Exception inner) : base(message, inner) { }
}