using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative;

return await Deployment.RunAsync(() =>
{

   // Set the stack name.
   var stackName = Deployment.Instance.StackName;

   // Create the "Hub" Resource Group and Virtual Network
   var hubResourceGroup = new Pulumi.AzureNative.Resources.ResourceGroup("rg-hub", new Pulumi.AzureNative.Resources.ResourceGroupArgs
   {
      Tags =
      {
         { "pulumistack", stackName },
      },
   });
   var hubVirtualNetwork = new Pulumi.AzureNative.Network.VirtualNetwork("hub", new Pulumi.AzureNative.Network.VirtualNetworkArgs
   {
      ResourceGroupName = hubResourceGroup.Name,
      AddressSpace = new Pulumi.AzureNative.Network.Inputs.AddressSpaceArgs
      {

         AddressPrefixes = { "10.0.0.0/16" }
      },
   });
   var hubSubnet = new Pulumi.AzureNative.Network.Subnet("shared", new Pulumi.AzureNative.Network.SubnetArgs
   {
      ResourceGroupName = hubResourceGroup.Name,
      VirtualNetworkName = hubVirtualNetwork.Name,
      AddressPrefix = "10.0.0.0/24"
   });

   // Create the "Spoke" Resource Group and Virtual Network
   var spokeResourceGroup = new Pulumi.AzureNative.Resources.ResourceGroup("rg-spoke", new Pulumi.AzureNative.Resources.ResourceGroupArgs
   {
      Tags =
      {
         { "pulumistack", stackName },
      },
   });
   var spokeVirtualNetwork = new Pulumi.AzureNative.Network.VirtualNetwork("spoke", new Pulumi.AzureNative.Network.VirtualNetworkArgs
   {
      ResourceGroupName = spokeResourceGroup.Name,
      AddressSpace = new Pulumi.AzureNative.Network.Inputs.AddressSpaceArgs
      {
         AddressPrefixes = { "10.1.0.0/16" }
      }
   });
   var spokeSubnet = new Pulumi.AzureNative.Network.Subnet("aks", new Pulumi.AzureNative.Network.SubnetArgs
   {
      ResourceGroupName = spokeResourceGroup.Name,
      VirtualNetworkName = spokeVirtualNetwork.Name,
      AddressPrefix = "10.1.0.0/24"
   });

   // Peer the spoke to hub
   var vnetPeeringSpoke = new Pulumi.AzureNative.Network.VirtualNetworkPeering("spoke-to-hub", new()
   {
      AllowForwardedTraffic = true,
      AllowGatewayTransit = false,
      AllowVirtualNetworkAccess = true,
      RemoteVirtualNetwork = new Pulumi.AzureNative.Network.Inputs.SubResourceArgs
      {
         Id = hubVirtualNetwork.Id,
      },
      ResourceGroupName = spokeResourceGroup.Name,
      UseRemoteGateways = false,
      VirtualNetworkName = spokeVirtualNetwork.Name,
      VirtualNetworkPeeringName = "spoke-to-hub",
   }, new CustomResourceOptions { DeleteBeforeReplace = true });

   // Peer the hub to spoke
   var vnetPeeringHub = new Pulumi.AzureNative.Network.VirtualNetworkPeering("hub-to-spoke", new()
   {
      AllowForwardedTraffic = true,
      AllowGatewayTransit = false,
      AllowVirtualNetworkAccess = true,
      RemoteVirtualNetwork = new Pulumi.AzureNative.Network.Inputs.SubResourceArgs
      {
         Id = spokeVirtualNetwork.Id,
      },
      ResourceGroupName = hubResourceGroup.Name,
      UseRemoteGateways = false,
      VirtualNetworkName = hubVirtualNetwork.Name,
      VirtualNetworkPeeringName = "hub-to-spoke",
   }, new CustomResourceOptions { DeleteBeforeReplace = true });

   // Now create the private zone
   var privateDnsZoneArgs = spokeResourceGroup.Apply(rg => new PrivateZoneArgs
   {
      ResourceGroupName = rg.Name,
      PrivateZoneName = $"privatelink.{rg.Location}.azmks.io",
   });

   var privateDnsZone = new Pulumi.AzureNative.Network.PrivateZone($"privatelink-{spokeResourceGroup.Apply(rg => rg.Location)}", privateDnsZoneArgs);


   return new Dictionary<string, object>
   {
         { "HubVNETId", hubVirtualNetwork.Id },
         { "SpokeVNETId", spokeVirtualNetwork.Id },
         { "VNetPeeringHubId", vnetPeeringHub.Id },
         { "VNetPeeringSpokeId", vnetPeeringSpoke.Id },
   };
});
