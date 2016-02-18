﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Services.Models.Rookery
{
    public class DisturbanceModel
    {
        public int DisturbanceId { get; set; }

        public int Quantity { get; set; }

        public int DurationMinutes { get; set; }

        public string Behavior { get; set; }

        public int DisturbanceTypeId { get; set; }
    }
}