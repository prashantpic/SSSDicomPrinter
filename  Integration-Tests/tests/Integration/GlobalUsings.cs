global using Xunit;
global using FluentAssertions;
global using Moq;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;

global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

global using TheSSS.DicomViewer.Application;
global using TheSSS.DicomViewer.Application.Services; // Assuming service interfaces are here
global using TheSSS.DicomViewer.Domain.Entities; // Assuming domain entities are here
global using TheSSS.DicomViewer.Domain.ValueObjects; // Assuming value objects are here
global using TheSSS.DicomViewer.Infrastructure.Persistence; // For DicomDbContext
global using TheSSS.DicomViewer.Infrastructure.Services; // Assuming infra services are here
global using TheSSS.DicomViewer.Common.Models; // Assuming common models are here
global using TheSSS.DicomViewer.Common.Extensions; // Assuming common extensions are here

global using TheSSS.DicomViewer.IntegrationTests.Fixtures;
global using TheSSS.DicomViewer.IntegrationTests.Mocks;
global using TheSSS.DicomViewer.IntegrationTests.Helpers;