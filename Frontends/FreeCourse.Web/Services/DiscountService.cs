using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Discount;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class DiscountService : IDiscountService
{

    private readonly HttpClient _httpClient;

    public DiscountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DiscountViewModel> GetDiscountAsync(string discountCode)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"discount/GetByCode/{discountCode}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<DiscountViewModel>? discount = await response.Content.ReadFromJsonAsync<Response<DiscountViewModel>>();

        return discount.Data;
    }
}
