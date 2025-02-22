using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Dtos
{
    public record Result<T>(T data, bool isSuccess, string errorMessage);

    public record NoData();
}
