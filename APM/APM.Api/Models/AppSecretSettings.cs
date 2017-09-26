using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Api.Models
{
    //Set the data for this class using 'Manager User Secrets'. Follow the guide here: https://tahirnaushad.com/2017/08/31/asp-net-core-2-0-secret-manager/
    public class AppSecretSettings
    {
        public string TableStorageConnectionString { get; set; }
    }
}
