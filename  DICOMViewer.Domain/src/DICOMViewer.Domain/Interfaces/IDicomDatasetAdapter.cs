using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Interfaces;

public interface IDicomDatasetAdapter
{
    bool TryGetString(DicomTagPath tagPath, out string? value);
    void SetString(DicomTagPath tagPath, string value);
    void RemoveElement(DicomTagPath tagPath);
    PixelSpacing? GetPixelSpacing();
    string? GetSopInstanceUid();
    bool AnonymizeTag(DicomTagPath tagPath, AnonymizationActionType actionType, params object?[] parameters);
}