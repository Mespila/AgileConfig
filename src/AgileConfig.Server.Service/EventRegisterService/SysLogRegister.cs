using System;
using System.Threading.Tasks;
using AgileConfig.Server.Common;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.IService;

namespace AgileConfig.Server.Service.EventRegisterService;

internal class SysLogRegister : IEventRegister
{
    private readonly ISysLogService _sysLogService;

    public SysLogRegister(ISysLogService sysLogService)
    {
        _sysLogService = sysLogService;
    }

    public void Register()
    {
        TinyEventBus.Instance.Register(EventKeys.USER_LOGIN_SUCCESS, (param) =>
        {
            dynamic param_dy = param as dynamic;
            string userName = param_dy.userName;
            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"{userName} logged in successfully"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.INIT_SUPERADMIN_PASSWORD_SUCCESS, (parm) =>
        {
            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"Super administrator password initialization successful"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.RESET_USER_PASSWORD_SUCCESS, (param) =>
        {
            dynamic param_dy = param as dynamic;
            User user = param_dy.user;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"User {userName} resets the password of {user.UserName} to the default password"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            })
            .ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    foreach (var ex in x.Exception?.InnerExceptions ?? new(Array.Empty<Exception>()))
                    {
                        throw ex;
                    }
                }
            })
            ;
        });

        TinyEventBus.Instance.Register(EventKeys.CHANGE_USER_PASSWORD_SUCCESS, (param) =>
        {
            dynamic param_dy = param as dynamic;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"Password modification of user {userName} successful"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.ADD_APP_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            App app = param_dy.app;
            string userName = param_dy.userName;
            if (app != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    LogText = $"User: {userName} New application [AppId: {app.Id}] [AppName: {app.Name}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        // app
        TinyEventBus.Instance.Register(EventKeys.EDIT_APP_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            App app = param_dy.app;
            string userName = param_dy.userName;
            if (app != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    LogText = $"User: {userName} Edit application [AppId: {app.Id}] [AppName: {app.Name}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        TinyEventBus.Instance.Register(EventKeys.DISABLE_OR_ENABLE_APP_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            App app = param_dy.app;
            string userName = param_dy.userName;
            if (app != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    LogText = $"User: {userName} {(app.Enabled ? "Enabled" : "Disabled")} Application [AppId:{app.Id}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        TinyEventBus.Instance.Register(EventKeys.DELETE_APP_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            App app = param_dy.app;
            string userName = param_dy.userName;
            if (app != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Warn,
                    LogText = $"User: {userName} Delete application [AppId: {app.Id}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        //config
        TinyEventBus.Instance.Register(EventKeys.ADD_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            Config config = param_dy.config;
            string userName = param_dy.userName;

            if (config != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    AppId = config.AppId,
                    LogText =
                        $"User: {userName} New configuration [Group: {config.Group}] [Key: {config.Key}] [AppId: {config.AppId}] [Env: {config.Env}] [To be released]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });
        TinyEventBus.Instance.Register(EventKeys.EDIT_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            Config config = param_dy.config;
            string userName = param_dy.userName;

            if (config != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    AppId = config.AppId,
                    LogText =
                        $"User: {userName} Edit configuration [Group: {config.Group}] [Key: {config.Key}] [AppId: {config.AppId}] [Env: {config.Env}] [To be released]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        TinyEventBus.Instance.Register(EventKeys.DELETE_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            Config config = param_dy.config;
            string userName = param_dy.userName;

            if (config != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Warn,
                    AppId = config.AppId,
                    LogText =
                        $"User: {userName} delete configuration [Group: {config.Group}] [Key: {config.Key}] [AppId: {config.AppId}] [Env: {config.Env}] [To be released]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });
        TinyEventBus.Instance.Register(EventKeys.DELETE_CONFIG_SOME_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            string userName = param_dy.userName;
            string appId = param_dy.appId;
            string env = param_dy.env;
            if (appId != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Warn,
                    AppId = appId,
                    LogText = $"User: {userName} Batch delete configuration [Env: {env}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });

        TinyEventBus.Instance.Register(EventKeys.PUBLISH_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            PublishTimeline node = param_dy.publishTimelineNode;
            string userName = param_dy.userName;
            string env = param_dy.env;
            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                AppId = node.AppId,
                LogText =
                    $"User: {userName} Publish configuration [AppId: {node.AppId}] [Env: {env}] [Version: {node.PublishTime.Value:yyyyMMddHHmmss}]"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });
        TinyEventBus.Instance.Register(EventKeys.ROLLBACK_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            string userName = param_dy.userName;
            PublishTimeline timelineNode = param_dy.timelineNode;
            string env = param_dy.env;

            if (timelineNode != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Warn,
                    AppId = timelineNode.AppId,
                    LogText =
                        $"{userName} rolls back the application [{timelineNode.AppId}] [Env: {env}] to the release version [{timelineNode.PublishTime.Value:yyyyMMddHHmmss}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });
        TinyEventBus.Instance.Register(EventKeys.CANCEL_EDIT_CONFIG_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            string userName = param_dy.userName;
            Config config = param_dy.config;

            if (config != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    AppId = config.AppId,
                    LogText =
                        $"{userName} Undoes the configuration of the editing state [Group: {config.Group}] [Key: {config.Key}] [AppId: {config.AppId}] [Env: {config.Env}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });
        TinyEventBus.Instance.Register(EventKeys.CANCEL_EDIT_CONFIG_SOME_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            string userName = param_dy.userName;
            string appId = param_dy.appId;
            string env = param_dy.env;

            if (appId != null)
            {
                var log = new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    AppId = appId,
                    LogText = $"{userName} batch undo configuration of editing status [Env: {env}]"
                };
                Task.Run(async () =>
                {
                    await _sysLogService.AddSysLogAsync(log);
                });
            }
        });
        TinyEventBus.Instance.Register(EventKeys.ADD_NODE_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            ServerNode node = param_dy.node;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"User: {userName} Add node: {node.Id}"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.DELETE_NODE_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            ServerNode node = param_dy.node;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Warn,
                LogText = $"User: {userName} Delete node: {node.Id}"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.ADD_USER_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            User user = param_dy.user;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"User: {userName} Add user: {user.UserName} Success"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.EDIT_USER_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            User user = param_dy.user;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"User: {userName} Edit user: {user.UserName} Success"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.DELETE_USER_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            User user = param_dy.user;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Warn,
                LogText = $"User: {userName} Delete user: {user.UserName} Successfully"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        TinyEventBus.Instance.Register(EventKeys.DISCONNECT_CLIENT_SUCCESS, (param) =>
        {
            dynamic param_dy = param;
            string clientId = param_dy.clientId;
            string userName = param_dy.userName;

            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Warn,
                LogText = $"User: {userName} disconnected client {clientId} successfully"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });

        //service info envets
        TinyEventBus.Instance.Register(EventKeys.REGISTER_A_SERVICE, (param) =>
        {
            dynamic param_dy = param;
            string serviceId = param_dy.ServiceId;
            string serviceName = param_dy.ServiceName;
            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"Service: [{serviceId}] [{serviceName}] Registration successful"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });
        TinyEventBus.Instance.Register(EventKeys.UNREGISTER_A_SERVICE, (param) =>
        {
            dynamic param_dy = param;
            string serviceId = param_dy.ServiceId;
            string serviceName = param_dy.ServiceName;
            var log = new SysLog
            {
                LogTime = DateTime.Now,
                LogType = SysLogType.Normal,
                LogText = $"Service: [{serviceId}] [{serviceName}] Uninstalled successfully"
            };
            Task.Run(async () =>
            {
                await _sysLogService.AddSysLogAsync(log);
            });
        });
    }
}