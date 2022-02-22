using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimeLog.Models
{
    public class EmployeeTimeLogDTO
    {
        public Employee Employee { get; set; }
        public List<TimeLog> TimeLogs { get; set; }
    }
}
