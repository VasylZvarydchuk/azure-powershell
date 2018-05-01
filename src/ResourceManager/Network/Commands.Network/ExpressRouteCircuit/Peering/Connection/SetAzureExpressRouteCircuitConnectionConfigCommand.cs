﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Network.Models;
using System;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Network
{
    [Cmdlet(VerbsCommon.Set, "AzureRmExpressRouteCircuitConnectionConfig", DefaultParameterSetName = "SetByResource", SupportsShouldProcess = true), OutputType(typeof(PSExpressRouteCircuit))]
    public class SetAzureExpressRouteCircuitConnectionConfigCommand : AzureExpressRouteCircuitConnectionConfigBase
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the Circuit Connection")]
        [ValidateNotNullOrEmpty]
        public override string Name { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Express Route Circuit Peering intiating connection")]
        [ValidateNotNullOrEmpty]
        public PSExpressRouteCircuit ExpressRouteCircuit { get; set; }

        public override void Execute()
        {
            base.Execute();

            var peering =
                    this.ExpressRouteCircuit.Peerings.First(
                        resource =>
                            string.Equals(resource.Name, "AzurePrivatePeering", System.StringComparison.CurrentCultureIgnoreCase));

            if (peering == null)
            {
                throw new ArgumentException("Private Peering needs to be configured on the Express Route Circuit");
            }

            var connection = peering.Connections.SingleOrDefault(resource => string.Equals(resource.Name, this.Name, StringComparison.CurrentCultureIgnoreCase));

            if (connection == null)
            {
                throw new ArgumentException("Circuit Connection with the specified name does not exist");
            }

            connection.Name = this.Name;
            connection.AddressPrefix = this.AddressPrefix;

            connection.ExpressRouteCircuitPeering = new PSResourceId();
            connection.ExpressRouteCircuitPeering.Id = peering.Id;

            connection.PeerExpressRouteCircuitPeering = new PSResourceId();
            connection.PeerExpressRouteCircuitPeering.Id = this.PeerExpressRouteCircuitPeering;

            if (!string.IsNullOrWhiteSpace(this.AuthorizationKey))
            {
                connection.AuthorizationKey = this.AuthorizationKey;
            }

            WriteObject(peering);
            WriteObject(this.ExpressRouteCircuit);
        }
    }
}