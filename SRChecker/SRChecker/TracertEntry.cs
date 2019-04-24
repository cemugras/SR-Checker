using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
namespace SRChecker
{
    public class TracertEntry
    {
        /// <summary>
        /// The hop id. Represents the number of the hop.
        /// </summary>
        public int HopID { get; set; }

        /// <summary>
        /// The IP address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The hostname
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The reply time it took for the host to receive and reply to the request in milliseconds.
        /// </summary>
        public long ReplyTime { get; set; }

        /// <summary>
        /// The reply status of the request.
        /// </summary>
        public IPStatus ReplyStatus { get; set; }

        public override string ToString()
        {
            return string.Format("{0} | {1} | {2} | {3}",
                HopID,
                string.IsNullOrEmpty(Hostname) ? Address : Hostname + "[" + Address + "]",
                ReplyStatus == IPStatus.TimedOut ? "Request Timed Out." : ReplyTime.ToString() + " ms", ReplyStatus.ToString()
                );
        }
    }
}