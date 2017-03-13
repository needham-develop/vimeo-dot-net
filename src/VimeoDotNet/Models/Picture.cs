using System;
using System.Collections.Generic;

namespace VimeoDotNet.Models {
    /// <summary>
    /// Picture
    /// </summary>
    [Serializable]
    public class Picture {
        /// <summary>
        /// Active
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// URI
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Sizes
        /// </summary>
        public List<Size> sizes { get; set; }
        /// <summary>
        /// link
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// Resource key
        /// </summary>
        public string resource_key { get; set; }
    }
}