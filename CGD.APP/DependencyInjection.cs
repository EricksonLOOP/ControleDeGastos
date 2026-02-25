using CGD.APP.Services.Auth;
using CGD.APP.Services.Categories;
using CGD.APP.Services.Expenses;
using CGD.APP.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CGD.APP;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
        services.AddScoped<Services.Groups.IGroupService, Services.Groups.GroupService>();


        return services;
    }
}