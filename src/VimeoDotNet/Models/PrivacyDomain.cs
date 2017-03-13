using System;

namespace VimeoDotNet.Models
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class PrivacyDomain
    {
        /// <summary>
        /// Domain
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// Active
        /// </summary>
        public int allow_hd { get; set; }
        /// <summary>
        /// URI
        /// </summary>
        public string uri { get; set; }
    }
}