namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public interface IRenderer<in TCanvas, in TDataSource, in TRenderParams>
    {
        void Render(TCanvas canvas, TDataSource dataSource, TRenderParams renderParams);
    }
}