// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Globalization;
using Azure.Deployments.Core.Instrumentation;

namespace Bicep.Core.Emit
{
    public class BicepDeploymentsInterop : IDeploymentsInterop
    {
        private BicepDeploymentsInterop()
        {
        }

        public int DeploymentNameLengthLimit => throw new NotImplementedException();

        public static void Initialize()
            => DeploymentsInterop.Initialize(new BicepDeploymentsInterop());

        public CultureInfo GetLocalizationCultureInfo() => CultureInfo.CurrentCulture;
    }
}