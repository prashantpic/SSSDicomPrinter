namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;
using TheSSS.DICOMViewer.Domain.Validation;
using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

public class AnonymizationProfile
{
    public AnonymizationProfileId Id { get; }
    public string ProfileName { get; private set; }
    public string? ProfileDescription { get; private set; }
    private readonly List<MetadataAnonymizationRule> _rules = new();
    public IReadOnlyList<MetadataAnonymizationRule> Rules => _rules.AsReadOnly();

    private Anonym