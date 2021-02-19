﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Helpers.Common;
using CoreJob.Web.Dashboard.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CoreJob.Framework;

namespace CoreJob.Web.Dashboard.Services.Command.Executer
{
    public class SaveExecuterCmd : IRequest<JsonEntityBase>
    {
        public ExecuterViewModel VM { get; set; }

        public class SaveExecuterCmdRequest : IRequestHandler<SaveExecuterCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            private readonly IValidationNotificationContext _validationNotificationContext;

            public SaveExecuterCmdRequest(JobDbContext dbContext,
                IValidationNotificationContext validationNotificationContext)
            {
                _dbContext = dbContext;
                _validationNotificationContext = validationNotificationContext;
            }

            public async Task<JsonEntityBase> Handle(SaveExecuterCmd request, CancellationToken cancellationToken)
            {
                if (_validationNotificationContext.HasErrorNotifications)
                {
                    return _validationNotificationContext.GetErrorNotifications().Error();
                }

                var model = request.VM;
                JobExecuter executer = null;
                if (model.IsNew)
                {
                    // add
                    executer = new JobExecuter()
                    {
                        RegistryKey = model.RegistryKey,
                        InTime = DateTime.Now,
                        Name = model.ExecuterName,
                        UpdateTime = DateTime.Now,
                        Auto = model.Auto
                    };

                    await _dbContext.JobExecuter.AddAsync(executer);

                    if (!model.Auto)
                    {
                        var registryHosts = model.RegistryHosts?.Where(x => x.Url.NotNullOrEmpty()).Select(x => new RegistryHost()
                        {
                            ExecuterId = executer.Id,
                            Host = x.Url,
                            Order = 0
                        });

                        if (registryHosts != null && registryHosts.Any())
                        {
                            await _dbContext.RegistryHost.BulkInsertAsync(registryHosts, options => {
                                options.InsertIfNotExists = true;
                                options.ColumnPrimaryKeyExpression = x => x.Host;
                            });
                        }
                    }
                    else
                    {
                        var regList = await _dbContext.RegistryInfo.OrderBy(x => x.InTime)
                                          .Where(x => x.Name == model.RegistryKey).ToListAsync();

                        if (regList != null && executer != null)
                        {
                            var registryHosts = regList.Select(x => new RegistryHost()
                            {
                                ExecuterId = executer.Id,
                                Host = x.Host,
                                Order = 0
                            });
                            await _dbContext.RegistryHost.BulkInsertAsync(registryHosts, options => {
                                options.InsertIfNotExists = true;
                                options.ColumnPrimaryKeyExpression = x => x.Host;
                            });
                        }
                    }

                }
                else
                {
                    executer = await _dbContext.JobExecuter.Include(x => x.RegistryHosts).FirstOrDefaultAsync(x => x.Id == model.ExecuterId);
                    executer.RegistryKey = model.RegistryKey;
                    executer.Name = model.ExecuterName;
                    executer.UpdateTime = DateTime.Now;
                    executer.Auto = model.Auto;

                    if (!model.Auto && model.RegistryHosts != null)
                    {
                        var registryHosts = model.RegistryHosts.Where(x => x.Url.NotNullOrEmpty())
                            .Select(x => new RegistryHost()
                        {
                            ExecuterId = executer.Id,
                            Host = x.Url,
                            Order = 0,
                            Id = x.Id
                        });

                        if (registryHosts != null && registryHosts.Any())
                        {
                            await _dbContext.RegistryHost.BulkMergeAsync(registryHosts, options => {
                                options.InsertIfNotExists = true;
                                options.ColumnPrimaryKeyExpression = x => x.Host;
                            });
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();

                return new { }.Success();
            }
        }

        public class SaveExecuterCmdValidator : AbstractValidator<SaveExecuterCmd>
        {
            public SaveExecuterCmdValidator()
            {
                RuleFor(x => x.VM.ExecuterName).NotEmpty().WithMessage("处理器名称不能为空");
                RuleFor(x => x.VM.RegistryKey).NotEmpty().WithMessage("注册名称不能为空");
                RuleFor(x => x.VM.RegistryHosts).NotEmpty().When(x => !x.VM.Auto).WithMessage("注册服务器不能为空");
            }
        }
    }
}
