using System;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent; 
using Microsoft.Azure.Management.ResourceManager.Fluent; 
using Microsoft.Azure.Management.ResourceManager.Fluent.Core; 
 
namespace AzureAuth 
{ 
    class Program 
    { 
 
        static void Main(string[] args) 
        { 
            // azureauth.json mit "az ad sp create-for-rbac --sdk-auth" erstellen 
            // pull in the location of the authentication properties file from the environment 
            // https://docs.microsoft.com/de-de/dotnet/azure/authentication 
            // Todo: Environment Variable 
        
            // pull in the location of the authentication properties file from the environment
            var credentials = SdkContext.AzureCredentialsFactory
                .FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();
            


          var resourceGroup1 = azure.ResourceGroups.Define("Test_Resourcen_Gruppe_RG1") 
                 .WithRegion(Region.EuropeWest) 
                .Create(); 
           var resourceGroup2 = azure.ResourceGroups.Define("Test_Resourcen_Gruppe_RG2") 
                 .WithRegion(Region.EuropeWest) 
                 .Create(); 
           var resourceGroup3 = azure.ResourceGroups.Define("Test_Resourcen_Gruppe_RG3") 
                .WithRegion(Region.EuropeWest) 
                .Create(); 
            // https://docs.microsoft.com/enus/dotnet/api/microsoft.azure.management.resourcemanager.fluent.core.collectionactions.isupportsdeletingbyname.deletebyname?view=azure-dotnet 
            // azure.ResourceGroups.DeleteByName("Test_Resourcen_Gruppe_RG1"); 
            // azure.ResourceGroups.DeleteByName("Test_Resourcen_Gruppe_RG2"); 
            var rgs = azure.ResourceGroups.List(); 
            foreach (var rg in rgs) 
            { 
                Console.WriteLine($"{rg.Id}\n{rg.Key}\n{rg.Name}\n{rg.RegionName}\n"); 
            }  
            
            //////////////////// WIN VM
            
            var network = azure.Networks.Define("sampleVirtualNetwork")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup1)  
                .WithAddressSpace("10.0.0.0/16")  
                .WithSubnet("sampleSubNet", "10.0.0.0/24")  
                .Create(); 
            
            
            var publicIPAddress = azure.PublicIPAddresses.Define("samplePublicIP")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup1)  
                .WithDynamicIP()  
                .Create();  

            
            var networkInterface = azure.NetworkInterfaces.Define("sampleNetWorkInterface")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup1)  
                .WithExistingPrimaryNetwork(network)  
                .WithSubnet("sampleSubNet")  
                .WithPrimaryPrivateIPAddressDynamic()  
                .WithExistingPrimaryPublicIPAddress(publicIPAddress)  
                .Create();  

            Console.WriteLine("Creating a Windows VM");

            azure.VirtualMachines.Define("WinVM")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup1)  
                .WithExistingPrimaryNetworkInterface(networkInterface)                  
                .WithLatestWindowsImage("MicrosoftWindowsDesktop", "Windows-10", "20h2-pro")  
                .WithAdminUsername("sampleUser")  
                .WithAdminPassword("Sample.123")  
                .WithComputerName("WinVM")  
                .WithSize(VirtualMachineSizeTypes.StandardD2sV3)  
                .Create(); 

            Console.WriteLine("Created a Windows VM");
            
            
            
            
            ////////////////////////Linux VM
            
            var network2 = azure.Networks.Define("sampleVirtualNetwork2")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup2)  
                .WithAddressSpace("10.0.0.0/16")  
                .WithSubnet("sampleSubNet", "10.0.0.0/24")  
                .Create(); 
            
            
            var publicIPAddress2 = azure.PublicIPAddresses.Define("samplePublicIP2")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup2)  
                .WithDynamicIP()  
                .Create();  

            
            var networkInterface2 = azure.NetworkInterfaces.Define("sampleNetWorkInterface2")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup2)  
                .WithExistingPrimaryNetwork(network)  
                .WithSubnet("sampleSubNet")  
                .WithPrimaryPrivateIPAddressDynamic()  
                .WithExistingPrimaryPublicIPAddress(publicIPAddress)  
                .Create();  

            Console.WriteLine("Creating a Linux VM");

            azure.VirtualMachines.Define("LinVM")  
                .WithRegion(Region.EuropeWest)  
                .WithExistingResourceGroup(resourceGroup2)  
                .WithExistingPrimaryNetworkInterface(networkInterface)                  
                .WithLatestLinuxImage("Canonical", "UbuntuServer", "18.04-LTS")  
                .WithRootUsername("rootId")
                .WithRootPassword("Pass1234.")
                .WithComputerName("LinVM")
                .WithSize(VirtualMachineSizeTypes.StandardD2sV3)
                .Create(); 
                
                  Console.WriteLine("Created a Linux VM");
               
                  
                  ///////////////////////////////////////////////////////////////

            azure.WebApps.Define("MyFirstWebApp")
                .WithRegion(Region.EuropeWest)
                .WithExistingResourceGroup(resourceGroup3)
                .WithNewWindowsPlan(PricingTier.FreeF1)
                .Create();

        } 
 
    } 
}