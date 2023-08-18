using Pulumi;
using Pulumi.Azure;
using System.IO;
using System.Text.Json;
class AzurePolicy
{

    public AzurePolicy(Input<string> resourceGroup, string policyName, string filePath)
    {
        var policyContent = File.ReadAllText(filePath);
        var policyObj = JsonSerializer.Deserialize<Root>(policyContent);
        var azurePolicy = new Pulumi.AzureNative.Authorization.PolicyDefinition(policyName, new PolicyDefinitionArgs()
        {
            DisplayName = policyName,
            Mode = policyObj.mode,
            Parameters = new Pulumi.AzureNative.Authorization.Inputs.ParameterDefinitionsValueArgs
            {
                
            },

        });
    }
}