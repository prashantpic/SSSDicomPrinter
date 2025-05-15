namespace TheSSS.DICOMViewer.Domain.Interfaces;
using TheSSS.DICOMViewer.Domain.ValueObjects;

public interface IDicomDatasetAdapter
{
    bool TryGetString(DicomTagPath tagPath, out string? value);
    void SetString(DicomTagPath tagPath, string value);
    void RemoveElement(DicomTagPath tagPath);
    PixelSpacing? GetPixelSpacing();
    string? GetSopInstanceUid();
}