﻿using Elysium.Authentication.Components;
using Elysium.Client.Services;
using Elysium.Components.Components;
using Elysium.Components.Components.Admin;
using Elysium.Exceptions;
using Elysium.Hosting.Services;
using Elysium.Services;
using Haondt.Web.Assets;
using Haondt.Web.Core.Components;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Core.Services;
using Haondt.Web.Services;

namespace Elysium.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElysiumServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ErrorSettings>(configuration.GetSection(nameof(ErrorSettings)));
            services.AddScoped<IEventHandler, ElysiumPublishActivityEventHandler>();
            services.AddSingleton<ISingletonPageComponentFactory, SingletonPageComponentFactory>();
            services.AddScoped<IEventHandler, AuthenticationEventHandler>();
            services.AddSingleton<IExceptionActionResultFactory, ElysiumExceptionActionResultFactory>();
            services.AddScoped<IComponentHandler, ElysiumComponentHandler>();
            services.Configure<AdminSettings>(configuration.GetSection(nameof(AdminSettings)));
            services.AddScoped<IClientStartupParticipant, RoleRegisterer>();
            services.AddScoped<IClientStartupParticipant, DefaultAdminAccountRegisterer>();
            services.Configure<RegistrationSettings>(configuration.GetSection(nameof(RegistrationSettings)));

            var assemblyPrefix = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            services.AddScoped<IHeadEntryDescriptor>(sp => new IconDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.icon.ico"
            });
            services.AddScoped<IHeadEntryDescriptor>(sp => new StyleSheetDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.bulma-custom.css"
            });
            services.AddScoped<IHeadEntryDescriptor>(sp => new ScriptDescriptor
            {
                Uri = "https://kit.fontawesome.com/afd44816da.js",
                CrossOrigin = "anonymous"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new MetaDescriptor
            {
                Name = "htmx-config",
                Content = @"{
                    ""responseHandling"": [
                        { ""code"": ""204"", ""swap"": false },
                        { ""code"": "".*"", ""swap"": true }
                    ]
                }",
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = "https://unpkg.com/@microsoft/signalr@8.0.7/dist/browser/signalr.js"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = $"/_asset/{assemblyPrefix}.wwwroot.hx-signalr.js"
            });
            services.AddScoped<IHeadEntryDescriptor>(_ => new ScriptDescriptor
            {
                Uri = "https://unpkg.com/htmx.org@1.9.12/dist/ext/ws.js"
            });
            return services;
        }

        public static IServiceCollection AddElysiumComponents(this IServiceCollection services)
        {
            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthorizationComponentDescriptor<ShadeSelectorModel>(async (cf, rd) =>
            {
                var session = sp.GetRequiredService<IUserSessionService>();
                var username = await session.GetLocalizedUsernameAsync();
                if (!username.IsSuccessful)
                    throw new ComponentException($"Failed to retrieve username. reason: {username.Reason}");
                var iri = await session.GetIriAsync();
                if (!iri.IsSuccessful)
                    throw new ComponentException($"Failed to retrieve iri. reason: {iri.Reason}");
                var shades = await session.GetShadesAsync();
                if (!shades.IsSuccessful)
                    throw new ComponentException($"Failed to load user shades. reason: {shades.Reason}");
                var activeShadeOptional = session.GetActiveShade();
                var activeShade = activeShadeOptional.HasValue ? activeShadeOptional.Value : iri.Value;

                var model = new ShadeSelectorModel
                {
                    ShadeSelections = new List<ShadeSelection>
                    {
                        new ()
                        {
                            IsPrimary = true,
                            Text = $"@{username.Value}",
                            IsActive = activeShade == iri.Value
                        }
                    }
                };

                var elysiumService = sp.GetRequiredService<IElysiumService>();
                foreach (var shade in shades.Value)
                {
                    model.ShadeSelections.Add(new ShadeSelection
                    {
                        IsActive = activeShade == shade,
                        IsPrimary = false,
                        Text = elysiumService.GetShadeNameFromLocalIri(iri.Value, shade)
                    });
                }

                return model;
            })
            {
                ViewPath = "~/Components/ShadeSelector.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated]
            });

            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthorizationComponentDescriptor<HomePageModel>(async (cf) =>
            {
                var feed = await cf.GetComponent<FeedModel>();
                var shadeSelector = await cf.GetComponent<ShadeSelectorModel>();
                return new HomePageModel
                {
                    ShadeSelector = shadeSelector,
                    Feed = feed
                };
            })
            {
                ViewPath = "~/Components/HomePage.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated]
            });

            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<FeedModel>(async cf =>
            {
                var elysiumService = sp.GetRequiredService<IElysiumService>();
                var creations = await elysiumService.GetPublicCreations();
                var mediaModels = creations.Creations.ToList();
                var mediaComponents = await Task.WhenAll(creations.Creations.Select(c => cf.GetComponent(c)));

                return new FeedModel
                {
                    Media = mediaComponents.ToList()
                };
            })
            {
                ViewPath = "~/Components/Feed.cshtml",
            });

            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<MediaModel>
            {
                ViewPath = $"~/Components/Media.cshtml",
            });

            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<ErrorModel>((componentFactory, requestData) =>
            {
                var errorCode = requestData.Query.GetValue<int>("errorCode");
                var message = requestData.Query.GetValue<string>("message");
                var title = requestData.Query.TryGetValue<string>("title");
                var details = requestData.Query.TryGetValue<string>("details");
                return new ErrorModel
                {
                    ErrorCode = errorCode,
                    Message = message,
                    Title = title,
                    Details = details,
                };
            })
            {
                ViewPath = "~/Components/Error.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<LoginModel>(() =>
            {
                var hostingService = sp.GetRequiredService<IHostingService>();
                var host = hostingService.Host;

                return new LoginModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/Login.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/identity/login")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new ComponentDescriptor<CloseModalModel>(new CloseModalModel())
            {
                ViewPath = "~/Components/CloseModal.cshtml"
            });
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<RegisterModalModel>(() =>
            {
                var hostingService = sp.GetRequiredService<IHostingService>();
                var host = hostingService.Host;

                return new RegisterModalModel
                {
                    Host = host
                };
            })
            {
                ViewPath = "~/Components/RegisterModal.cshtml",
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReSwap("none")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<DefaultLayoutModel>
            {
                ViewPath = $"~/Components/DefaultLayout.cshtml",
            });


            // temporary message model
            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthorizationComponentDescriptor<TemporaryMessageComponentLayoutModel>(() => new TemporaryMessageComponentLayoutModel
            {
                Messages = []
            })
            {
                ViewPath = "~/Components/TemporaryMessageComponentLayout.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#fill-content")
                    .ReSwap("innerHTML")
                    .PushUrl("/messages")
                    .Build())
            });
            services.AddScoped<IComponentDescriptor>(_ => new NeedsAuthorizationComponentDescriptor<TemporaryMessageComponentUpdateModel>()
            {
                ViewPath = "~/Components/TemporaryMessageComponentUpdate.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
            });
            // end temporary message model

            services.AddScoped<IComponentDescriptor>(sp => new NeedsAuthorizationComponentDescriptor<CreatePostModalModel>(() => new())
            {
                ViewPath = "~/Components/CreatePostModal.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAuthenticated],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                .ReSwap("none")
                .Build())
            });

            services.AddScoped<IComponentDescriptor>(sp => new ComponentDescriptor<InvitedRegisterLayoutModel>((cf, rd) =>
            {
                var hostingService = sp.GetRequiredService<IHostingService>();
                var host = hostingService.Host;

                var inviteId = rd.Query.TryGetValue<string>("inviteId");

                return new InvitedRegisterLayoutModel
                {
                    Host = host,
                    InviteId = inviteId.HasValue ? inviteId.Value : ""
                };
            })
            {
                ViewPath = "~/Components/InvitedRegisterLayout.cshtml",
            });

            services.AddAdminComponents();

            return services;
        }

        public static IServiceCollection AddAdminComponents(this IServiceCollection services)
        {

            services.AddScoped<IComponentDescriptor>(_ => new NeedsAuthorizationComponentDescriptor<AdminPanelLayoutModel>(async (componentFactory, requestData) =>
            {
                return new AdminPanelLayoutModel
                {
                    ActivePage = await componentFactory.GetPlainComponent<AdminPanelLandingModel>(),
                };
            })
            {
                ViewPath = "~/Components/Admin/AdminPanelLayout.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#content")
                    .ReSwap("innerHTML")
                    .PushUrl("/admin")
                    .Build())
            });

            services.AddScoped<IComponentDescriptor>(_ => new NeedsAuthorizationComponentDescriptor<AdminPanelLandingModel>(new AdminPanelLandingModel())
            {
                ViewPath = "~/Components/Admin/AdminPanelLanding.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
            });

            services.AddScoped<IComponentDescriptor>(_ => new NeedsAuthorizationComponentDescriptor<GenerateInviteModel>(new GenerateInviteModel())
            {
                ViewPath = "~/Components/Admin/GenerateInvite.cshtml",
                AuthorizationChecks = [ComponentAuthorizationCheck.IsAdministrator],
                ConfigureResponse = new(m => m.ConfigureHeadersAction = new HxHeaderBuilder()
                    .ReTarget("#admin-panel-page")
                    .ReSwap("innerHTML")
                    .Build())

            });

            return services;
        }

        public static IServiceCollection AddElysiumAssetSources(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;
            services.AddSingleton<IAssetSource>(sp => new ManifestAssetSource(assembly));
            return services;
        }
    }
}
