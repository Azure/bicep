// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.ConsoleExperiment;
using Spectre.Console;

public static class Example
{
    public static string Truncate(this string value, int maxLength, string truncationSuffix = "...")
    {
        if (value.Length > maxLength && maxLength > 3)
        {
            int middle = maxLength / 2;
            var trunc_str = value.Substring(0, middle) + "..." + value.Substring(value.Length - middle + 1);
            return trunc_str;
        }
        return value;
    }
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
        var deploymentList = new List<Deployments>() { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12 }; //original
        var w = Console.WindowWidth;
        //fill up to 800
        var truncatedList = new List<Deployments>(); //modified
        for (int i = 0; i < 800; i++)
        {
            truncatedList.Add(deploymentList[i % 12]);
            truncatedList[i].Name = Truncate(truncatedList[i].Name, w / 3);
        }

        //group by resource type
        var resourceGroups = truncatedList.GroupBy(d => d.ResourceType);

        var table = new Table().LeftAligned();
        table.Border = TableBorder.Simple;
        table.Collapse();
        table.Width(w);
        table.AddColumn("Name").LeftAligned();
        table.AddColumn("Resource Type");// column=>column.Width(60).Alignment(Justify.Right));
        table.AddColumn("Status").LeftAligned();
        //table.Columns
        await AnsiConsole.Live(table)
        //.Overflow(VerticalOverflow.Ellipsis)
        //.Cropping(VerticalOverflowCropping.)
        .StartAsync(async ctx =>
        {
            foreach (var group in resourceGroups)
            {
                foreach (var item in group)
                {
                    if (item.Status.Equals("Creating"))
                    {
                        table.AddRow($"[yellow]{item.Name}[/]", item.ResourceType, $"[blue]{item.Status}[/]").LeftAligned();
                    }
                    else
                    {
                        table.AddRow($"[yellow]{item.Name}[/]", item.ResourceType, $"[green]{item.Status}[/]").LeftAligned();
                    }
                    ctx.Refresh();
                    await Task.Delay(50);
                }
            } 
                table.UpdateCell(0, 2, "[green]Created[/]");
                d1.Status = "Created";
                table.UpdateCell(5, 2, "[green]Created[/]");
                d6.Status = "Created";
                await Task.Delay(2000);
                ctx.Refresh();
                table.UpdateCell(2, 2, "[green]Created[/]");
                d3.Status = "Created";
                table.UpdateCell(3, 2, "[green]Created[/]");
                d4.Status = "Created";
                await Task.Delay(2000);
                ctx.Refresh();
                table.UpdateCell(7, 2, "[green]Created[/]");
                d8.Status = "Created";
                table.UpdateCell(10, 2, "[green]Created[/]");
                d11.Status = "Created";
                await Task.Delay(2000);
                ctx.Refresh();
                table.UpdateCell(11, 2, "[green]Created[/]");
                d12.Status = "Created";
                await Task.Delay(2000);
                ctx.Refresh();
        });

        while (true)
        {
            //w = Console.WindowWidth;
            if (w != Console.WindowWidth)
            {
                w = Console.WindowWidth;
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

                for (int i = 0; i < 800; i++)
                {
                    //truncatedList.Add(deploymentList[i % 12]);
                    truncatedList[i].Name = Truncate(deploymentList[i % 12].Name, w / 3);
                }

                //group by resource type
                resourceGroups = truncatedList.GroupBy(d => d.ResourceType);

                table = new Table().LeftAligned();
                table.Border = TableBorder.Simple;
                table.Collapse();
                table.Width(w);
                table.AddColumn("Name").LeftAligned();
                table.AddColumn("Resource Type");// column=>column.Width(60).Alignment(Justify.Right));
                table.AddColumn("Status").LeftAligned();
                //table.Columns
                await AnsiConsole.Live(table)
                .StartAsync(async ctx =>
                {
                    foreach (var group in resourceGroups)
                    {
                        foreach (var item in group)
                        {
                            if (item.Status.Equals("Creating"))
                            {
                                table.AddRow($"[yellow]{item.Name}[/]", item.ResourceType, $"[blue]{item.Status}[/]").LeftAligned();
                            }
                            else
                            {
                                table.AddRow($"[yellow]{item.Name}[/]", item.ResourceType, $"[green]{item.Status}[/]").LeftAligned();
                            }
                        }
                    }
                    ctx.Refresh();
                    await Task.Delay(50);
                });
            }
        }
    }
}
