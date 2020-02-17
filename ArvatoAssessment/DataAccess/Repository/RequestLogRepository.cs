using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvatoAssessment.DataAcess.Entity;

namespace ArvatoAssessment.DataAccess.Repository
{
    public class RequestLogRepository : IRequestLogRepository
    {
        public RequestLog Add(RequestLog requestLog)
        {
            using(var conn = new RequestContext())
            {
                conn.RequestLogs.Add(requestLog);

                conn.SaveChanges();

                return requestLog;
            }
        }

        public RequestLog Get(int id)
        {
            using (var conn = new RequestContext())
            {
                return conn.RequestLogs.Find(id);
            }
        }

        public IEnumerable<RequestLog> GetAll()
        {
            using (var conn = new RequestContext())
            {
                return conn.RequestLogs.ToList();
            }
        }
    }
}
