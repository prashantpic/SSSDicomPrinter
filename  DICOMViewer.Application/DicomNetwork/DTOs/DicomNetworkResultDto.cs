namespace TheSSS.DICOMViewer.Application.DicomNetwork.DTOs
{
    public record DicomNetworkResultDto(
        bool Success,
        string Message,
        string Detail
    );
}