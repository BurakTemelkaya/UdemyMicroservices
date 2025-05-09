﻿using FreeCourse.Gateway.DelegateHandlers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName}.json").AddEnvironmentVariables();

builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.Audience = "resource_gateway";
    options.RequireHttpsMetadata = false;
    options.MapInboundClaims = false;
});

builder.Services.AddHttpClient<TokenExchangeDelegateHandler>();

builder.Services.AddOcelot().AddDelegatingHandler<TokenExchangeDelegateHandler>();

var app = builder.Build();

await app.UseOcelot();

app.Run();