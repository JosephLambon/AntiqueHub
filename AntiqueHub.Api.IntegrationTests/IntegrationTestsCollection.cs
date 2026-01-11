using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AntiqueHub.Core.Entities;
using AntiqueHub.Core.Models;

namespace AntiqueHub.Api.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestsCollection))]
public class IntegrationTestsCollection : ICollectionFixture<PostgreSqlContainerFixture>, ICollectionFixture<AzuriteContainerFixture> { }