using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Api.Models
{
    public class AppSettings
    {
        public string TableStorageConnectionString { get; set; }
        public string TableStorageContainerName { get; set; }
        public string TableStoragePartitionKey { get; set; }
    }
}
