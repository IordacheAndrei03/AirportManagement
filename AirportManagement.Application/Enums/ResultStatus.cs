using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Enums
{
    public enum ResultStatus
    {
        Ok = 0,
        Invalid = 1,
        NotFound = 2,
        Conflict = 3,
        Forbidden = 4
    }
}
