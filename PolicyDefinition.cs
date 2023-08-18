// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Text.Json;
using System.Collections.Generic;
public class AllOf
{
    public string field { get; set; }
    public string equals { get; set; }
}

public class Deployment
{
    public Properties properties { get; set; }
}

public class Details
{
    public string type { get; set; }
    public ExistenceCondition existenceCondition { get; set; }
    public List<string> roleDefinitionIds { get; set; }
    public Deployment deployment { get; set; }
}

public class ExistenceCondition
{
    public List<AllOf> allOf { get; set; }
}

public class If
{
    public List<AllOf> allOf { get; set; }
}

public class Metadata
{
    public string displayName { get; set; }
    public string description { get; set; }
}

public class Parameters
{
    public TagName tagName { get; set; }
    public TagValue tagValue { get; set; }
    public VirtualNetworkId virtualNetworkId { get; set; }
    public PrivateDnsZoneName privateDnsZoneName { get; set; }
}

public class PolicyRule
{
    public If @if { get; set; }
    public Then then { get; set; }
}

public class PrivateDnsZoneName
{
    public string type { get; set; }
    public Metadata metadata { get; set; }
    public string value { get; set; }
}

public class Properties
{
    public string mode { get; set; }
    public Template template { get; set; }
    public Parameters parameters { get; set; }
    public VirtualNetwork virtualNetwork { get; set; }
    public bool registrationEnabled { get; set; }
}

public class Resource
{
    public string type { get; set; }
    public string apiVersion { get; set; }
    public string name { get; set; }
    public string location { get; set; }
    public Properties properties { get; set; }
}

public class Root
{
    public string mode { get; set; }
    public Parameters parameters { get; set; }
    public PolicyRule policyRule { get; set; }
}

public class TagName
{
    public string type { get; set; }
    public Metadata metadata { get; set; }
    public string defaultValue { get; set; }
}

public class TagValue
{
    public string type { get; set; }
    public Metadata metadata { get; set; }
    public string defaultValue { get; set; }
}

public class Template
{
//[JsonProperty("$schema")]
    public string schema { get; set; }
    public string contentVersion { get; set; }
    public Parameters parameters { get; set; }
    public Variables variables { get; set; }
    public List<Resource> resources { get; set; }
}

public class Then
{
    public string effect { get; set; }
    public Details details { get; set; }
}

public class Variables
{
    public string virtualNetworkName { get; set; }
}

public class VirtualNetwork
{
    public string id { get; set; }
}

public class VirtualNetworkId
{
    public string type { get; set; }
    public Metadata metadata { get; set; }
    public string value { get; set; }
}

