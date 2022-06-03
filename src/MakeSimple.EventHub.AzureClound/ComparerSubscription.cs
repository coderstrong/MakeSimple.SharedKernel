using System.Collections.Generic;

namespace MakeSimple.EventHub.AzureClound
{
    public class ComparerSubscription : IEqualityComparer<SettingAttribute>
    {
        public bool Equals(SettingAttribute x, SettingAttribute y)
        {
            return x.ProjectId == y.ProjectId && x.SubscriptionId == y.SubscriptionId;
        }

        public int GetHashCode(SettingAttribute obj)
        {
            return string.Concat(obj.ProjectId, obj.SubscriptionId).GetHashCode();
        }
    }
}
