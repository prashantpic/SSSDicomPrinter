using System;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public class ViewLocator : IViewLocator
    {
        public Type ResolveViewType(Type viewModelType)
        {
            var viewName = viewModelType.FullName!.Replace("ViewModel", "View");
            return Type.GetType(viewName) ?? throw new InvalidOperationException($"View not found for {viewModelType}");
        }
    }
}