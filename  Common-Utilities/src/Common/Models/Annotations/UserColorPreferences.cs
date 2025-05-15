using System.Collections.Generic;
using TheSSS.DicomViewer.Common.Enums;

namespace TheSSS.DicomViewer.Common.Models.Annotations
{
    public class UserColorPreferences
    {
        public Dictionary<AnnotationType, AnnotationColorProfile> Preferences { get; set; } = new Dictionary<AnnotationType, AnnotationColorProfile>();
    }
}