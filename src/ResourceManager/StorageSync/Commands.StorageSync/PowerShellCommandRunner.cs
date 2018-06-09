﻿namespace Microsoft.Azure.Commands.StorageSync.Evaluation
{
    using Interfaces;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    public class PowerShellCommandRunner : IPowershellCommandRunner
    {
        #region Fields and Properties
        private readonly Runspace _runspace;
        private readonly PowerShell _powerShell;
        #endregion

        #region Constructors
        public PowerShellCommandRunner(string computerName, PSCredential credential)
        {
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo();

            if (computerName != null && !computerName.Equals("localhost", System.StringComparison.OrdinalIgnoreCase))
            {
                connectionInfo.ComputerName = computerName;
            }

            if (credential != null)
            {
                connectionInfo.Credential = credential;
            }

            _runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            _runspace.Open();

            _powerShell = PowerShell.Create();
            _powerShell.Runspace = _runspace;
        }

        ~PowerShellCommandRunner()
        {
            _powerShell?.Dispose();
            _runspace?.Close();
        }
        #endregion

        #region Public methods
        public void AddScript(string script)
        {
            _powerShell.AddScript(script);
        }

        public Collection<PSObject> Invoke()
        {
            return _powerShell.Invoke();
        }

        public PSDataCollection<ErrorRecord> Errors()
        {
            return _powerShell.Streams.Error;
        }
        #endregion
    }
}