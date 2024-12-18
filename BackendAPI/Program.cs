using BackendAPI.Data;
using BackendAPI.Services;
using BackendAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BackendAPI {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers to the container
            builder.Services.AddControllers();

            // Add DbContext to the container
            builder.Services.AddDbContext<AppDbContext>(options => {
                options.UseMySQL(builder.Configuration.GetConnectionString("TheForum_MySQL")!);
            });

            // Add Repositories to the container
            builder.Services.AddScoped<PostRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<CommentRepository>();
            builder.Services.AddScoped<CommentReactionRepository>();

            // Add Services to the container
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<CommentService>();
            builder.Services.AddScoped<CommentReactionService>();
            builder.Services.AddScoped<ImageService>();

            // JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings!.Key);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Reduce time skew
                };
            });

            builder.Services.AddAuthorization();

            // Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "The Forum API",
                    Version = "v1",
                    Description = "API documentation for The Forum project"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' [space] and then your token in the text input below.\nExample: 'Bearer 12345abcdef'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Ensure Images folder exists
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(uploadsFolder)) {
                Directory.CreateDirectory(uploadsFolder);
            }
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(uploadsFolder),
                RequestPath = "/images"
            });

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            
        }
    }
}
