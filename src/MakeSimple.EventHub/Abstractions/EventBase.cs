using System;
using System.Collections.Generic;

namespace MakeSimple.EventHub.Abstractions
{
    public class EventBase
    {
        /// <summary>
        /// Provider support send notification for all device with identity id
        /// </summary>
        public IEnumerable<string> UserIds { get; set; }

        public Guid EventId { get; set; }

        /// <summary>
        /// Message for service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Use set name module for tracking
        /// </summary>
        public string Name { get; set; }

        public IEnumerable<string> IncludedSegemnts { get; set; }

        public IEnumerable<string> ExcludedSegemnts { get; set; }

        public string TemplateId { get; set; }
    }
}