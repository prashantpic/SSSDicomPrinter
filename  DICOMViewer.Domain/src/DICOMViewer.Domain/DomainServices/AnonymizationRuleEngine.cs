using System.Threading.Tasks;
using DICOMViewer.Domain.Interfaces;
using DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;
using DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

namespace DICOMViewer.Domain.DomainServices
{
    public class AnonymizationRuleEngine
    {
        public async Task ApplyProfileAsync(IDicomDatasetAdapter dataset, AnonymizationProfile profile)
        {
            foreach (var rule in profile.Rules)
            {
                switch (rule.ActionType)
                {
                    case AnonymizationActionType.Remove:
                        dataset.RemoveElement(rule.TagPath);
                        break;
                        
                    case AnonymizationActionType.ReplaceWithFixedValue:
                        if (string.IsNullOrEmpty(rule.ReplacementValue))
                            throw new BusinessRuleViolationException("Replacement value required for Replace action");
                        
                        dataset.SetString(rule.TagPath, rule.ReplacementValue);
                        break;
                }
            }
            await Task.CompletedTask;
        }
    }
}