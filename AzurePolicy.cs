using Pulumi;
using System.IO;
using System.Text.Json;
class AzurePolicy
{

    public AzurePolicy(Input<string> resourceGroupName, string policyName, string filePath)
    {
        var policyContent = File.ReadAllText(filePath);
        var policyObj = JsonSerializer.Deserialize<Root>(policyContent);
        var azurePolicy = new Pulumi.Azure.Policy.Definition(policyName, new()
        {
            DisplayName = policyName,
            Mode = policyObj.mode,
            Parameters = JsonSerializer.Serialize(policyObj.parameters),
            PolicyRule = JsonSerializer.Serialize(policyObj.policyRule),
            PolicyType = "Custom",
        });
        PolicyDefinitionId = azurePolicy.Id;
    }

    [Output] public Output<string> PolicyDefinitionId { get; set; }
}