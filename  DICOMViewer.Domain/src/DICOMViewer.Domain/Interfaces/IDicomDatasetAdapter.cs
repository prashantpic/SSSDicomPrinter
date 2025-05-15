using DICOMViewer.Domain.ValueObjects;

namespace DICOMViewer.Domain.Interfaces
{
    public interface IDicomDatasetAdapter
    {
        bool TryGetString(DicomTagPath tagPath, out string? value);
        void SetString(DicomTagPath tagPath, string value);
        void RemoveElement(DicomTagPath tagPath);
        PixelSpacing? GetPixelSpacing();
        string? GetSopInstanceUid();
    }
}