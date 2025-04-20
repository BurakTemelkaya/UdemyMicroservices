using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;

    public BasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddBasketItemAsync(BasketItemViewModel basketItemViewModel)
    {
        BasketViewModel basket = await GetAsync();

        if (basket != null)
        {
            if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
            {
                basket.BasketItems.Add(basketItemViewModel);
            }
        }
        else
        {
            basket = new()
            {
                BasketItems =
                [
                    basketItemViewModel
                ]
            };

            await SaveorUpdateAsync(basket);
        }     
    }

    public Task<bool> ApplyDiscountAsync(string discountCode)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CancelApplyDiscountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync()
    {
        HttpResponseMessage result = await _httpClient.DeleteAsync("baskets");

        return result.IsSuccessStatusCode;
    }

    public async Task<BasketViewModel> GetAsync()
    {
        var response = await _httpClient.GetAsync("baskets");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<BasketViewModel>? basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();

        return basketViewModel.Data;
    }

    public async Task<bool> RemoveBasketItemAsync(string courseId)
    {
        BasketViewModel basket = await GetAsync();

        if (basket == null)
        {
            return false;
        }

        BasketItemViewModel deleteBasketItem = basket.BasketItems.First(x => x.CourseId == courseId);

        bool deleteResult = basket.BasketItems.Remove(deleteBasketItem);

        if (!deleteResult)
        {
            return false;
        }

        if (!basket.BasketItems.Any())
        {
            basket.DiscountCode = null;
        }

        await SaveorUpdateAsync(basket);

        return true;
    }

    public async Task<bool> SaveorUpdateAsync(BasketViewModel basketViewModel)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);

        return response.IsSuccessStatusCode;
    }
}
