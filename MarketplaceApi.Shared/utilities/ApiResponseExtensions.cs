using Microsoft.AspNetCore.Mvc;

namespace MarketplaceApi.Shared.utilities;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response, ControllerBase controller)
    {
        if (!response.Success)
        {
            return response.Errors?.Any() == true
                ? controller.BadRequest(response)
                : controller.NotFound(response);
        }
        
        return controller.Ok(response);
    }
}