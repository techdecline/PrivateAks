
using Pulumi;
using AzureNative = Pulumi.AzureNative;
using Pulumi.AzureNative.Authorization;
using System.Collections.Generic;
class AksCluster
{
    public AksCluster(Input<string> resourceGroupName, Input<string> privateDnsZoneId, Input<string> subnetId, Input<string> sshPublicKey, string subscriptionId, Input<string> resourceGroupId, Input<string> vnetId)
    {
        // Create Identity for Cluster
        var azureHelper = new AzureHelper(subscriptionId);
        string roleDefinitionManagedIdentityOperatorId = azureHelper.GetRoleByName("Managed Identity Operator");
        var clusterIdentity = new ManagedIdentity(resourceGroupName, "clusterIdentity", roleDefinitionManagedIdentityOperatorId, resourceGroupId);

        var roleAssignmentPrivateDnsContributor = new AzureRoleAssignment("Private DNS Zone Contributor", subscriptionId, clusterIdentity.PrincipalId, privateDnsZoneId);
        var roleAssignmentNetworkContributor = new AzureRoleAssignment("Network Contributor", subscriptionId, clusterIdentity.PrincipalId, vnetId);

        AzureNative.ContainerService.ManagedCluster managedCluster = new AzureNative.ContainerService.ManagedCluster("managedCluster", new()
        {
            AgentPoolProfiles = new[]
            {
                new AzureNative.ContainerService.Inputs.ManagedClusterAgentPoolProfileArgs
                {
                    AvailabilityZones = new[]
                {
                    "1", "2", "3",
                },
                Count = 3,
                EnableNodePublicIP = false,
                Mode = "System",
                Name = "systempool",
                OsType = "Linux",
                OsDiskSizeGB = 30,
                Type = "VirtualMachineScaleSets",
                VmSize = "standard_d2s_v5",
                VnetSubnetID = subnetId
                },
            },
            ApiServerAccessProfile = new AzureNative.ContainerService.Inputs.ManagedClusterAPIServerAccessProfileArgs
            {
                EnablePrivateCluster = true,
                EnablePrivateClusterPublicFQDN = false,
                PrivateDNSZone = privateDnsZoneId
            },
            AutoScalerProfile = new AzureNative.ContainerService.Inputs.ManagedClusterPropertiesAutoScalerProfileArgs
            {
                ScaleDownDelayAfterAdd = "15m",
                ScanInterval = "20s",
            },
            DnsPrefix = "dnsprefix1",
            EnableRBAC = true,
            KubernetesVersion = "1.26.6",
            LinuxProfile = new AzureNative.ContainerService.Inputs.ContainerServiceLinuxProfileArgs
            {
                AdminUsername = "azureuser",
                Ssh = new AzureNative.ContainerService.Inputs.ContainerServiceSshConfigurationArgs
                {
                    PublicKeys = new[]
                    {
                        new AzureNative.ContainerService.Inputs.ContainerServiceSshPublicKeyArgs
                        {
                            KeyData = sshPublicKey,
                        },
                    },
                },
            },
            Identity = new AzureNative.ContainerService.Inputs.ManagedClusterIdentityArgs
            {
                Type = AzureNative.ContainerService.ResourceIdentityType.UserAssigned,
                UserAssignedIdentities =
                {
                    { clusterIdentity.Id}
                },
            },
            NetworkProfile = new AzureNative.ContainerService.Inputs.ContainerServiceNetworkProfileArgs
            {
                LoadBalancerProfile = new AzureNative.ContainerService.Inputs.ManagedClusterLoadBalancerProfileArgs
                {
                    ManagedOutboundIPs = new AzureNative.ContainerService.Inputs.ManagedClusterLoadBalancerProfileManagedOutboundIPsArgs
                    {
                        Count = 2,
                    },
                },
                LoadBalancerSku = "standard",
                OutboundType = "loadBalancer",
            },
            ResourceGroupName = resourceGroupName,
            ResourceName = "clustername1",
            Sku = new AzureNative.ContainerService.Inputs.ManagedClusterSKUArgs
            {
                Name = "Base",
                Tier = "Free",
            },
        });
    }
}