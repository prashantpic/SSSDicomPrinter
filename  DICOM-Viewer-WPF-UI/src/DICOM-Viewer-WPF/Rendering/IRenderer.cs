namespace TheSSS.DicomViewer.Presentation.Rendering;

public interface IRenderer
{
    void Render(object canvasContext, object imageViewModel, object renderParameters);
}