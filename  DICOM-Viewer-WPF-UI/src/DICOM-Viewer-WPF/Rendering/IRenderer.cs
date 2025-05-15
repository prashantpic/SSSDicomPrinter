namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public interface IRenderer<TCanvasContext, TDataSource, TRenderParameters>
    {
        void Render(TCanvasContext canvasContext, TDataSource dataSource, TRenderParameters renderParameters);
    }
}