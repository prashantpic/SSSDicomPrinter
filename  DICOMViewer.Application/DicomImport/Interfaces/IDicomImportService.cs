using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.DicomImport.DTOs;

namespace TheSSS.DICOMViewer.Application.DicomImport.Interfaces
{
    public interface IDicomImportService
    {
        Task<ImportResultDto> ImportFilesAsync(IEnumerable<string> filePaths);
        Task<ImportResultDto> ImportDirectoryAsync(string directoryPath);
    }
}