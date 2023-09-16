// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.RegularExpressions;
using Bicep.ConsoleExperiment;
using Spectre.Console;

public static class Example
{
    public static async Task Main(string[] args)
    {
        //create dummy deployment info
        var deploymentObj = new DeploymentInfo("azureDeploy2", "stephy-rg", "0.01 (AzHPCImageGallery/AlmaLinuxHPC-8.6-gen2/0.0.1) - Microsoft Azure", "Value");

        // Render each item in list on separate line
        AnsiConsole.Write("Deploment name: ");
        AnsiConsole.Write(new Text(deploymentObj.Name, new Style(Color.Blue)));
        AnsiConsole.WriteLine();
        AnsiConsole.Write("Resource group: ");
        AnsiConsole.Write(new Text(deploymentObj.ResourceGroup, new Style(Color.Blue)));
        AnsiConsole.WriteLine();
        AnsiConsole.Write("Portal Link: ");
        AnsiConsole.Write(new Text(deploymentObj.PortalLink, new Style(Color.Blue)));
        AnsiConsole.WriteLine();
        AnsiConsole.Write("Correlation ID: ");
        AnsiConsole.Write(new Text(deploymentObj.CorrelationID, new Style(Color.Blue)));
        AnsiConsole.WriteLine("\n");

        //create dummy deployment objects
        var d1 = new Deployments("networkInterfaceName", "Microsoft.Network/networkInterfaces", "Creating");
        var d2 = new Deployments("networkSecurityGroup", "Microsoft.Network/networkSecurityGroups", "Created");
        var d3 = new Deployments("virtualNetworkName", "Microsoft.Network/virtualNetworks", "Creating");
        var d4 = new Deployments("vnetname1/subnetname1", "Microsoft.Network/virtualNetworks/subnets", "Creating");
        var d5 = new Deployments("subnetName2netName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName", "Microsoft.Network/virtualNetworks/subnets", "Created");
        var d6 = new Deployments("subnetName3netName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetNamenetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName1subnetName", "Microsoft.Network/virtualNetworks/subnets", "Creating");
        var d7 = new Deployments("subnetName4subnetName4subnetName4", "Microsoft.Network/virtualNetworks/subnets", "Created");
        var d8 = new Deployments("vmExtension2", "Microsoft.Compute/virtualMachines/extensions", "Creating");
        var d9 = new Deployments("publicIpAddressName2", "Microsoft.Compute/virtualMachines/extensions", "Created");
        var d10 = new Deployments("publicIpAddressName1", "Microsoft.Network/PublicIPAddresses", "Created");
        var d11 = new Deployments("vmName1", "Microsoft.Compute/virtualMachines", "Creating");
        var d12 = new Deployments("vmName2", "Microsoft.Compute/virtualMachines", "Creating");

        //group in a list
        var deploymentList = new List<Deployments>() { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12 };

        //create grid
        var grid = new Grid();
        // Add columns 
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();

        // Add header row 
        grid.AddRow(new string[] { "Name", "Resource type", "Status" });

        //group by resource type
        var resourceGroups = deploymentList.GroupBy(d => d.ResourceType);


        //await AnsiConsole.Live(grid)
        //    .AutoClear(false)   // Do not remove when done
        //    .Overflow(VerticalOverflow.Ellipsis) // Show ellipsis when overflowing
        //    .Cropping(VerticalOverflowCropping.Top) // Crop overflow at top
        //    .StartAsync(async ctx =>
        //    {
        //        foreach (var group in resourceGroups)
        //        {
        //            foreach (var item in group)
        //            {
        //                grid.AddRow(item.Name, item.ResourceType, item.Status);
        //                ctx.Refresh();
        //                await Task.Delay(250);
        //            }
        //            grid.AddEmptyRow();
        //            ctx.Refresh();
        //            await Task.Delay(500);
        //        }

        //        //rerender if any changes
        //        while (true)
        //        {
        //            //refresh entire grid
        //            grid = new Grid();
        //            grid.AddColumn();
        //            grid.AddColumn();
        //            grid.AddColumn();

        //            grid.AddRow(new string[] { "Name", "Resource type", "Status" });

        //            //change some values
        //            d1.Status = "Created";
        //            d4.Status = "Created";

        //            foreach (var group in resourceGroups)
        //            {
        //                foreach (var item in group)
        //                {
        //                    grid.AddRow(item.Name, item.ResourceType, item.Status);
        //                }
        //                grid.AddEmptyRow();
        //            }
        //            ctx.UpdateTarget(grid);
        //            await Task.Delay(1000);
        //        }
        //    });

        var table = new Table().LeftAligned();
        table.Border = TableBorder.SimpleHeavy;
        table.AddColumn("Name").LeftAligned();
        table.AddColumn("Resource Type").LeftAligned();
        table.AddColumn("Status").LeftAligned();
        await AnsiConsole.Live(table)
        .StartAsync(async ctx =>
        {
            foreach (var group in resourceGroups)
            {
                foreach (var item in group)
                {
                    table.AddRow(item.Name, item.ResourceType, item.Status).LeftAligned();
                    ctx.Refresh();
                    await Task.Delay(250);
                }
                grid.AddEmptyRow().LeftAligned(); //not visually working for table in terminal
                ctx.Refresh();
                await Task.Delay(500);
            }
            while (true)
            {
                //try to change some value to update live
                //d1.Status = "Created";
                //d4.Status = "Created";
                //table.Rows.Update(1,2, table);
                table.UpdateCell(1, 2, "Created");
                table.UpdateCell(4, 2, "Created");
                ctx.Refresh();
                //ctx.UpdateTarget(table);
                await Task.Delay(1000);
            }

        });
    }
}
