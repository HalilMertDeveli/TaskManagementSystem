using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberPagesListRequestHandler :IRequestHandler<MemberListPagedRequest,PagedResult<MemberListDto>>
    {
        private readonly IUserRepository repository;

        public MemberPagesListRequestHandler(IUserRepository repository)
        {
            this.repository = repository;
        }


        public async Task<PagedResult<MemberListDto>> Handle(MemberListPagedRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetAllAsync(request.ActivePage, request.S, 10);

            var mappedData = result.Data.Select(x => new MemberListDto(x.Id, x.Name, x.Surname, x.UserName)).ToList();

            return new PagedResult<MemberListDto>(mappedData, request.ActivePage, result.PageSize, result.TotalPages);
        }
    }
}
