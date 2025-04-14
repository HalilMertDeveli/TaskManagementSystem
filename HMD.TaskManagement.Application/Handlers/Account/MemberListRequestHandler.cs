using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportRequestHandler:IRequestHandler<MemberListRequest,Result<List<MemberListDto>>>
    {
        private readonly IUserRepository userRepository;

        public TaskReportRequestHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<List<MemberListDto>>> Handle(MemberListRequest request, CancellationToken cancellationToken)
        {
           var result = await this.userRepository.GetAllByFilterAsync(x => x.AppRoleId == (int)RoleType.Member, false);

           return new Result<List<MemberListDto>>(result.ToMap(),true,null,null);
        }
    }
}
