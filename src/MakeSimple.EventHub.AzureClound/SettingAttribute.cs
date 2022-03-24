using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.AzureClound
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class SettingAttribute : Attribute
    {
        public string EventHubName { get; set; }
        public string Suffix { get; set; }
    }
}
