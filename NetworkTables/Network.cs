using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using mDNS;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using System.Diagnostics;

namespace NetworkTables
{
    class Network : imDNSClient
    {

        mDNSManager mDNS;
        ObservableCollection<mDNSHostInfo> hosts = new ObservableCollection<mDNSHostInfo>();

        public Network ()
        {
            mDNS = new mDNSManager(this);
            mDNS.StartFind();
        }

        public void HostFound(mDNSHostInfo result)
        {
            hosts.Add(result);
        }

        public void mDNSStatusChange(DeviceWatcherStatus newStatus)
        {
            
        }

        public mDNSHostInfo GetInfo(string id)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            while (s.Elapsed < TimeSpan.FromSeconds(30))
            {
                mDNSHostInfo[] copyOfHosts = new mDNSHostInfo[hosts.Count+10];
                hosts.CopyTo(copyOfHosts, 0);
                foreach (mDNSHostInfo host in copyOfHosts)
                {
                    if (host != null && host.Id == id)
                    {
                        s.Stop();
                        return host;
                    }
                }
            }
            s.Stop();
            return null;
        }
    }
}
