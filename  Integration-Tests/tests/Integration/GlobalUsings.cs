global using Xunit;
global using FluentAssertions;
global using Moq;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using System.IO;
global using System.Diagnostics;

// SUT Namespaces
global using TheSSS.DicomViewer.Application.Services;
global using TheSSS.DicomViewer.Infrastructure.Data;
global using TheSSS.DicomViewer.Domain.Entities;
global using TheSSS.DicomViewer.Common.Logging;
global using TheSSS.DicomViewer.Presentation.ViewModels;

// Test Project Specific Namespaces (will be added as files are created)
// global using TheSSS.DicomViewer.IntegrationTests.Fixtures;
// global using TheSSS.DicomViewer.IntegrationTests.Mocks;
// global using TheSSS.DicomViewer.IntegrationTests.Helpers;