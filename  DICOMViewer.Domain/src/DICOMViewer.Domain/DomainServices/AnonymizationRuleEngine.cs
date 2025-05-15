using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;
using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;
using TheSSS.DICOMViewer.Domain.Interfaces;

namespace TheSSS.DICOMViewer.Domain.DomainServices;

public class AnonymizationRuleEngine
{
    private readonly ILoggerAdapter _logger;

    public AnonymizationRuleEngine(ILoggerAdapter logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ApplyProfile(IDicomDatasetAdapter datasetAdapter, AnonymizationProfile profile)
    {
        if (datasetAdapter == null) throw new ArgumentNullException(nameof(datasetAdapter));
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        foreach (var rule in profile.Rules)
        {
            try
            {
                ApplyRule(datasetAdapter, rule);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to apply rule for tag {rule.DicomTagPath}");
                throw new DomainException($"Error applying rule for tag {rule.DicomTagPath}", ex);
            }
        }
    }

    private void ApplyRule(IDicomDatasetAdapter datasetAdapter, MetadataAnonymizationRule rule)
    {
        switch (rule.ActionType)
        {
            case AnonymizationActionType.Remove:
                datasetAdapter.RemoveElement(rule.DicomTagPath);
                break;
            
            case AnonymizationActionType.ReplaceWithFixedValue:
                datasetAdapter.SetString(rule.DicomTagPath, rule.ReplacementValue!);
                break;
            
            case AnonymizationActionType.Hash:
            case AnonymizationActionType.DateOffset:
            case AnonymizationActionType.Clean:
                datasetAdapter.AnonymizeTag(rule.DicomTagPath, rule.ActionType, rule.ReplacementValue);
                break;
            
            default:
                throw new BusinessRuleViolationException($"Unsupported action type: {rule.ActionType}");
        }
    }
}