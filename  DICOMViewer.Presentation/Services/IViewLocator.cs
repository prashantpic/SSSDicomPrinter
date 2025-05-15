using System;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public interface IViewLocator
    {
        Type ResolveViewType(Type viewModelType);
    }
}