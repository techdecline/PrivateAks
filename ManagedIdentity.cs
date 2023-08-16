using Pulumi;
using AzureNative = Pulumi.AzureNative;
using System.Collections.Generic;
using Pulumi.AzureNative.Authorization;

class ManagedIdentity
{
    public ManagedIdentity(Input<string> resourceGroupName, string managedIdentityName, Dictionary<string, Input<string>> roleAssignmentList, string subscriptionId)
    {
        var identity = new AzureNative.ManagedIdentity.UserAssignedIdentity(managedIdentityName, new()
        {
            ResourceGroupName = resourceGroupName,
        });
        
        foreach (var roleAssignment in roleAssignmentList)
        {
            new AzureSPRoleAssignment(roleAssignment.Key, subscriptionId,identity.PrincipalId, roleAssignment.Value); 
        }

        ClientId = identity.ClientId;
        PrincipalId = identity.PrincipalId;
        Id = identity.Id;
    }

    [Output] public Output<string> ClientId { get; set; }
    [Output] public Output<string> PrincipalId { get; set; }
    [Output] public Output<string> Id { get; set; }
}