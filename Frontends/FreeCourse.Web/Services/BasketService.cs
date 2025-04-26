using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.Discount;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    private readonly IDiscountService _discountService;

    public BasketService(HttpClient httpClient, IDiscountService discountService)
    {
        _httpClient = httpClient;
        _discountService = discountService;
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
            basket = new();
            basket.BasketItems.Add(basketItemViewModel);
        }

        await SaveorUpdateAsync(basket);
    }

    public async Task<bool> ApplyDiscountAsync(string discountCode)
    {
        await CancelApplyDiscountAsync();

        BasketViewModel basket = await GetAsync();

        if (basket == null)
        {
            return false;
        }

        DiscountViewModel discount = await _discountService.GetDiscountAsync(discountCode);

        if (discount == null)
        {
            return false;
        }

        basket.ApplyDiscount(discount.Code, discount.Rate);

        return await SaveorUpdateAsync(basket);
    }

    public async Task<bool> CancelApplyDiscountAsync()
    {
        BasketViewModel basket = await GetAsync();

        if (basket == null || !basket.HasDiscount)
        {
            return false;
        }

        basket.CancelDiscount();

        bool result = await SaveorUpdateAsync(basket);

        return result;
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

        if (basket.BasketItems.Count == 0)
        {
            basket.DiscountCode = null;
        }

        return await SaveorUpdateAsync(basket);
    }

    public async Task<bool> SaveorUpdateAsync(BasketViewModel basketViewModel)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);

        return response.IsSuccessStatusCode;
    }
}
