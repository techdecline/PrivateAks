using Pulumi;
using Pulumi.Random;
using AzureNative = Pulumi.AzureNative;
class AzureRoleAssignment
{
    public AzureRoleAssignment(string roleDefinitionName, string subscriptionId, Input<string> principalId, Input<string> scope)
    {
        var azureHelper = new AzureHelper(subscriptionId);
        var roleDefinitionId = azureHelper.GetRoleByName(roleDefinitionName);
        var roleAssignmentGuid = new Pulumi.Random.RandomUuid("roleAssignmentGuid");

        var roleAssignment = new AzureNative.Authorization.RoleAssignment("roleAssignment", new()
        {
            PrincipalId = principalId,
            PrincipalType = "ServicePrincipal",
            RoleAssignmentName = roleAssignmentGuid.Result,
            RoleDefinitionId = roleDefinitionId,
            Scope = scope,
        });
    }
}