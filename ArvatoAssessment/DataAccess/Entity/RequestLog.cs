using System;
using System.Collections.Generic;
using System.Text;

namespace ArvatoAssessment.DataAcess.Entity
{
    public class RequestLog
    {
        public int Id { get; set; }

        public string RequestUrl { get; set; }

        public DateTime RequestTime { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }
    }

}
