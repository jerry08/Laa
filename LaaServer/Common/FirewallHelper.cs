using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;

namespace LaaServer
{
    public class FirewallHelper
    {
        //See reference: https://stackoverflow.com/a/8505066
        public static void Test1()
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Description = "Used to block all internet access.";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "Block Internet";

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
        }

        public static void AddOutboundRule()
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "Used to allow port 9091.";
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallRule.Enabled = true;
            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallRule.LocalPorts = "9091";
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = "9091 test";

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
        }

        //public static void OpenPort(int port)
        //{
        //    var powershell = PowerShell.Create();
        //    var psCommand = $"New-NetFirewallRule -DisplayName \"<rule description>\" -Direction Inbound -LocalPort {port} -Protocol TCP -Action Allow";
        //    powershell.Commands.AddScript(psCommand);
        //    powershell.Invoke();
        //}

        public static void RemoveRule(string fwRuleName)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            List<INetFwRule> firewallRules = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name.Contains(fwRuleName)).ToList();

            foreach (INetFwRule rule in firewallRules)
            {
                firewallPolicy.Rules.Remove(rule.Name);
            }
        }

        public static bool IsPortOpen(int fwPort)
        {
            string port = fwPort.ToString();

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            List<INetFwRule> firewallRules = firewallPolicy.Rules.OfType<INetFwRule>()
                .Where(x => !string.IsNullOrEmpty(x.LocalPorts) && x.LocalPorts.Contains(port)
                && x.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW && x.Enabled == true).ToList();

            return firewallRules.Count > 0;
        }
    }
}
