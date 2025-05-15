namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public interface IRenderer<in TCanvasContext, in TDataSource, in TRenderParameters>
    {
        void Render(TCanvasContext canvasContext, TDataSource dataSource, TRenderParameters renderParameters);
    }
}