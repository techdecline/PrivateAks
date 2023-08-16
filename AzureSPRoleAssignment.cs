using Pulumi;
using Pulumi.Random;
using AzureNative = Pulumi.AzureNative;
class AzureSPRoleAssignment
{
    public AzureSPRoleAssignment(string roleDefinitionName, string subscriptionId, Input<string> principalId, Input<string> scope)
    {
        var azureHelper = new AzureHelper(subscriptionId);
        var roleDefinitionId = azureHelper.GetRoleByName(roleDefinitionName);
        string roleDefinitionSanitizedName = roleDefinitionName.Replace(" ","");
        var roleAssignmentGuid = new Pulumi.Random.RandomUuid($"roleAssignmentGuid-{roleDefinitionSanitizedName}");

        var roleAssignment = new AzureNative.Authorization.RoleAssignment($"roleAssignment-{roleDefinitionSanitizedName}", new()
        {
            PrincipalId = principalId,
            PrincipalType = "ServicePrincipal",
            RoleAssignmentName = roleAssignmentGuid.Result,
            RoleDefinitionId = roleDefinitionId,
            Scope = scope,
        });
    }
}