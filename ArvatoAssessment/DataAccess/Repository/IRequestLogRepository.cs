using ArvatoAssessment.DataAcess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArvatoAssessment.DataAccess.Repository
{
    public interface IRequestLogRepository
    {
        RequestLog Add(RequestLog requestLog);

        IEnumerable<RequestLog> GetAll();

        RequestLog Get(int id);
    }
}
